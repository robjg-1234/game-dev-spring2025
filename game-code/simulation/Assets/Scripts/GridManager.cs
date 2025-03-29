using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] GameObject ShopPanel;
    
    [SerializeField] public TMP_Dropdown targetMenu;
    [SerializeField] GameObject ships;
    [SerializeField] Camera _camera;
    [SerializeField] TMP_Text currentMonth;
    [SerializeField] GameObject pauseImage;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject endPanel;
    //General
    [SerializeField] TMP_Text cellDesc;
    [SerializeField] TMP_Text HPDesc;
    [SerializeField] TMP_Text ReputationDesc;
    [SerializeField] TMP_Text locationDesc;
    // Players Stuff
    [SerializeField] TMP_Text playerFirepower;
    [SerializeField] TMP_Text playerHealth;
    [SerializeField] TMP_Text coords;
    [SerializeField] TMP_Text helmsmanLevel;
    [SerializeField] TMP_Text playerTreasure;
    [SerializeField] TMP_Text playerReputation;
    [SerializeField] TMP_Text rumors;
    //
    public float[,] costMap;
    Vector3 target;
    public static GridManager instance;
    [SerializeField] GameObject cellPrefab;
    public CellScript[,] grid;
    [SerializeField] public int width;
    [SerializeField] public int height;
    CellScript currentCell;
    CellScript selectedCell;
    [SerializeField] float rotationSpeed = 20f;
    public Action treasureFound;
    public List<(int, int)> availableDocks = new List<(int, int)>();
    public List<ShipScript> currentShips = new List<ShipScript>();
    ShipScript playerShip;
    public (int, int) treasure = (-1, -1);
    public bool[,] traversableMap;
    public int weather = 0;
    bool paused = true;
    int month = 1;
    float stepTimer = 2f;
    float angle = 0;
    [SerializeField] float distance;
    bool GameDone = false;
    float offSetX;
    float offSetZ;
    // Volcano by Poly by Google [CC-BY] via Poly Pizza
    //Pirate Ship by Braden Brunk[CC - BY] via Poly Pizza

    void Start()
    {
        costMap = new float[width, height];
        target = new Vector3(width / 2f, 4f, height / 2f);
        offSetX = target.x;
        offSetZ = target.z;
        angle = 180f - Mathf.Abs(_camera.transform.rotation.eulerAngles.y) - 90;
        instance = this;
        grid = new CellScript[width, height];
        traversableMap = new bool[width, height];
        InitializeGrid();
        PlaceBoats();
        UpdateMyShip();
    }
    private void Update()
    {
        if (!GameDone)
        {
            float xAxis = Input.GetAxisRaw("Horizontal");
            angle += xAxis * rotationSpeed * Time.deltaTime;
            while (angle < 0f)
            {
                angle += 360f;
            }
            while (angle >= 360f)
            {
                angle -= 360f;
            }
            CameraPosition();

            if (!paused)
            {
                if (stepTimer > 0)
                {
                    stepTimer -= Time.deltaTime;
                }
                else
                {
                    SimulateStep();
                    stepTimer = 2f;
                    month += 1;
                    currentMonth.text = "Hour " + month.ToString();
                }
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("cell")))
            {
                currentCell = hit.collider.GetComponentInParent<CellScript>();
                if (selectedCell != null)
                {
                    if (currentCell != selectedCell)
                    {
                        selectedCell.UnSelect();
                        selectedCell = currentCell;
                        selectedCell.SelectCell();
                    }
                }
                else
                {
                    selectedCell = currentCell;
                    selectedCell.SelectCell();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    panel.SetActive(true);
                    ShowSummary(selectedCell.State.Copy());
                }
            }
            else
            {
                if (selectedCell != null)
                {
                    selectedCell.UnSelect();
                    selectedCell = null;
                    currentCell = null;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    panel.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TogglePause();
            }
        }
    }
    void SimulateStep()
    {
        CellInfo[,] newGrid = new CellInfo[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newGrid[i, j] = grid[i, j].State.Copy();
                newGrid[i, j].UpdateState();
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j].State = newGrid[i, j];
            }
        }
        for (int i = 0; i < currentShips.Count; i++)
        {
            currentShips[i].simulateNextStep();
        }
        newGrid = null;
        UpdateMyShip();

    }
    void ShowSummary(CellInfo checkState)
    {
        if (checkState.occupied)
        {
            ShipScript cellShip = grid[checkState.x, checkState.y].GetShip();
            ArrayList shipShow = cellShip.GetShipSummary();
            cellDesc.text = "Ship";
            HPDesc.text = "Hull HP: " + shipShow[0].ToString();
            locationDesc.text = shipShow[4].ToString();
            ReputationDesc.text = "Reputation: " + shipShow[7].ToString();
        }
        else
        {
            ArrayList tempShow = checkState.GetSummary();
            cellDesc.text = tempShow[0].ToString();
            if (checkState.traversable)
            {
                HPDesc.text = "Tide Level: " + tempShow[1].ToString();
            }
            else
            {
                HPDesc.text = " ";
            }

            locationDesc.text = "(" + tempShow[2].ToString() + ", " + tempShow[3].ToString() + ")";
            ReputationDesc.text = " ";
        }

    }
    void InitializeGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = new Vector3(i, 0, j);
                grid[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity).GetComponent<CellScript>();
                grid[i, j].SetCoordinates(i, j);
                costMap[i, j] = 9999999999;
                int randomizedState = Mathf.RoundToInt(Mathf.PerlinNoise(i / 6.5f, j / 6.5f) * 5f) - 1;
                CellInfo tempInit = grid[i, j].State.Copy();
                tempInit.SetState(randomizedState);
                traversableMap[i, j] = tempInit.traversable;
                grid[i, j].State = tempInit;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j].State.PlaceShops();
            }
        }
        

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j].State.PreStartUpdates();
                grid[i, j].State = grid[i, j].State.Copy();
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j].State.shopDock)
                {
                    availableDocks.Add((i, j));
                }
            }
        }
        ChooseTreasure();
    }
    void ChooseTreasure()
    {
        (int, int) newSpot = (-1, -1);
        int[] xVal = new int[width];
        int[] yVal = new int[height];
        for (int i = 0; i < width; i++)
        {
            xVal[i] = i;
        }
        for (int i = 0; i < height; i++)
        {
            yVal[i] = i;
        }
        yVal = Randomizer(yVal);
        xVal = Randomizer(xVal);
        for (int i = 0; i < xVal.Length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (traversableMap[xVal[i], yVal[j]] && !availableDocks.Contains((xVal[i], yVal[j])))
                {
                    newSpot = (xVal[i], yVal[j]);
                    break;
                }
            }
        }
        treasure = newSpot;
        Debug.Log(treasure.ToString());
    }
    public void TogglePause()
    {
        paused = !paused;
        pauseImage.SetActive(paused);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(target, new Vector3(0.5f, 0.5f, 0.5f));
    }
    void CameraPosition()
    {
        float changeX;
        float changeZ;
        float posY = _camera.transform.position.y;

        changeX = distance * Mathf.Cos(angle);
        changeZ = distance * Mathf.Sin(angle);
        _camera.transform.position = new Vector3(changeX + offSetX, posY, changeZ + offSetZ);
        _camera.transform.LookAt(target);
    }
    void PlaceBoats()
    {
        bool setPlayer = true;
        int remainingShips = 4;
        (int, int)[] randomPositions = new (int, int)[width * height];
        int counter = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                randomPositions[counter] = (i, j);
                counter++;
            }
        }

        randomPositions = Randomizer(randomPositions);
        for (int i = 0; i < randomPositions.Length; i++)
        {
            if (traversableMap[randomPositions[i].Item1, randomPositions[i].Item2] && remainingShips > 0)
            {
                if (UnityEngine.Random.Range(0, 4) == 0)
                {
                    Vector3 pos = new Vector3(randomPositions[i].Item1, 0, randomPositions[i].Item2);
                    ShipScript ship = Instantiate(ships, pos, Quaternion.identity).GetComponent<ShipScript>();
                    ship.InitializeBoat(randomPositions[i].Item1, randomPositions[i].Item2, setPlayer);
                    currentShips.Add(ship);
                    grid[randomPositions[i].Item1, randomPositions[i].Item2].SetShip(ship);
                    remainingShips--;
                    if (setPlayer)
                    {
                        playerShip = ship;
                    }
                    setPlayer = false;
                }
            }
        }



    }
    static int[] Randomizer(int[] numbers)
    {
        int[] tempArray = numbers;
        for (int i = 0; i < numbers.Length; i++)
        {
            int temp = UnityEngine.Random.Range(0, numbers.Length);
            int tempVal = tempArray[i];
            tempArray[i] = tempArray[temp];
            tempArray[temp] = tempVal;
        }
        return tempArray;
    }
    //Tuple overload
    static (int, int)[] Randomizer((int, int)[] numbers)
    {
        (int, int)[] tempArray = numbers;
        for (int i = 0; i < numbers.Length; i++)
        {
            int temp = UnityEngine.Random.Range(0, numbers.Length);
            (int, int) tempVal = tempArray[i];
            tempArray[i] = tempArray[temp];
            tempArray[temp] = tempVal;
        }
        return tempArray;
    }
    public void GrabbedTreaure()
    {
        if (treasureFound != null)
        {
            ChooseTreasure();
            treasureFound();
        }
    }

    void UpdateMyShip()
    {
        if (playerShip != null)
        {
            ArrayList summary = playerShip.GetShipSummary();
            playerHealth.text = "Hull HP: " + summary[0].ToString() + "/" + summary[1].ToString();
            playerFirepower.text = "Firepower: " + summary[2].ToString() + "/" + summary[3].ToString();
            coords.text = summary[4].ToString();
            helmsmanLevel.text = "Helmsman Skill: " + summary[5].ToString();
            playerTreasure.text = "Treasure: " + summary[6].ToString();
            playerReputation.text = "Reputation: " + summary[7].ToString();
            if (playerShip.treasureSpot == treasure)
            {
                rumors.text = "Treasure Rumors: " + treasure.ToString();
            }
            else
            {
                rumors.text = "Treasure Rumors: " + summary[8].ToString();
            }
        }
    }

    public void EndGame()
    {
        endPanel.SetActive(true);
        GameDone = true;
    }
    public void TargetModifier(int val)
    {
        if (playerShip != null)
        {
            playerShip.SetPriority(val);
        }
    }
    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }
    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }
    public void FixHull()
    {
        if (playerShip != null)
        {
            if (playerShip.hullHP < playerShip.maxHull && playerShip.treasure > 4)
            {
                playerShip.treasure -= 5;
                playerShip.hullHP++;
            }
        }
        UpdateMyShip();
    }
    public void UpgradeHull()
    {
        if (playerShip != null)
        {
            if (5 > playerShip.maxHull && playerShip.treasure >= 25)
            {
                playerShip.treasure -= 25;
                playerShip.maxHull++;
                playerShip.hullHP = playerShip.maxHull;
            }
        }
        UpdateMyShip();
    }
    public void LevelHelmsman()
    {
        if (playerShip != null)
        {
            if (3 > playerShip.helmsmanLevel && playerShip.treasure >= 50)
            {
                playerShip.treasure -= 50;
                playerShip.helmsmanLevel++;
            }
        }
        UpdateMyShip();
    }
    public void MoreFirepower()
    {
        if (playerShip != null)
        {
            if (playerShip.treasure >= 10)
            {
                playerShip.treasure -= 10;
                playerShip.maxFirePower++;
                playerShip.firePower = playerShip.maxFirePower;
            }
        }
        UpdateMyShip();
    }

}
