using System.Collections;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] TMP_Text currentMonth;
    [SerializeField] GameObject pauseImage;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text cellDesc;
    [SerializeField] TMP_Text humidity;
    [SerializeField] TMP_Text temperature;
    [SerializeField] TMP_Text animalPop;
    Vector3 target;
    public static GridManager instance;
    [SerializeField] GameObject cellPrefab;
    public CellScript[,] grid;
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] int waterSpots;
    CellScript currentCell;
    CellScript selectedCell;
    [SerializeField] float rotationSpeed = 20f;
    bool paused = true;
    bool freedomUnits = false;
    int month = 1;
    float stepTimer = 5f;
    float angle = 0;
    float distance;
    float offSetX;
    float offSetZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Volcano by Poly by Google [CC-BY] via Poly Pizza

    // polar coordinates

    void Start()
    {
        target = new Vector3(width / 2f, 1f, height / 2f);
        offSetX = target.x;
        offSetZ = target.z;
        angle = 180f - Mathf.Abs(_camera.transform.rotation.eulerAngles.y) - 90;
        distance = Mathf.Sqrt(Mathf.Pow(_camera.transform.position.x, 2) + Mathf.Pow(_camera.transform.position.y, 2));
        instance = this;
        grid = new CellScript[width, height];
        InitializeGrid();
    }
    private void Update()
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
                if (selectedCell != null)
                {
                    ShowSummary(selectedCell.State);
                }
                stepTimer = 5f;
                month += 1;
                currentMonth.text = "Month " + month.ToString();
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
                    ShowSummary(selectedCell.State);
                }
            }
            else
            {
                selectedCell = currentCell;
                selectedCell.SelectCell();
                ShowSummary(selectedCell.State);
            }
            if (Input.GetMouseButtonDown(0))
            {
                CellInfo tempCheck = selectedCell.State;
                tempCheck.SetState(3, tempCheck.x, tempCheck.y);
                selectedCell.State = tempCheck;
                ShowSummary(selectedCell.State);

            }
            if (Input.GetMouseButtonDown(1))
            {
                CellInfo tempCheck = selectedCell.State;
                tempCheck.SetState(0, tempCheck.x, tempCheck.y);
                selectedCell.State = tempCheck;
                ShowSummary(selectedCell.State);
            }
            panel.SetActive(true);
        }
        else
        {
            if (selectedCell != null)
            {
                selectedCell.UnSelect();
                selectedCell = null;
                currentCell = null;
            }
            panel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }
    // Update is called once per frame
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
                if (waterSpots > 0)
                {
                    if (Random.Range(0, 4) == 1)
                    {
                        CellInfo tempCell = new CellInfo();
                        tempCell.SetState(1, i, j);
                        grid[i, j].State = tempCell;
                        waterSpots--;
                    }
                }
            }
        }
    }

    void ShowSummary(CellInfo target)
    {
        ArrayList tempShow = target.GetSummary();
        cellDesc.text = tempShow[0].ToString();
        humidity.text = "Humidity: " + ((float)tempShow[1]).ToString("0.00") + "%";
        if (freedomUnits)
        {
            float fahrenheitTemp = (float)tempShow[2];
            fahrenheitTemp = (fahrenheitTemp * 9f / 5f) + 32;
            temperature.text = "Temperature: " + fahrenheitTemp.ToString("0.00") + "°F";
        }
        else
        {
            temperature.text = "Temperature: " + ((float)tempShow[2]).ToString("0.00") + "°C";
        }
        animalPop.text = "Animal Population: " + tempShow[3].ToString();
    }
    public void ToggleFreedomUnits(bool val)
    {
        freedomUnits = val;
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
}
