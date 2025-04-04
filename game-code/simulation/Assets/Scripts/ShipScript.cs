
using System.Collections;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    GameObject indicator;
    [SerializeField] GameObject display;
    GridManager gm;
    [SerializeField] GameObject shipDesign;
    public int direction = 0;
    public int hullHP = 3;
    public int reputation = 5;
    public int helmsmanLevel = 1;
    public int heardShots = 0;
    public int treasure = 0;
    public int maxHull = 3;
    public int firePower = 5;
    public int maxFirePower = 5;
    int priority = 1;
    (int, int)[] path = new (int, int)[1] { (-1, -1) };
    public (int, int) currentPos = (0, 0);
    public (int, int) treasureSpot = (-1, -1);
    (int, int) currentTarget = (0, 0);
    public bool dead = false;
    public bool isPlayer = false;
    int pathUpdateFrequency = 5;
    int respawnTimer = 10;
    ShipScript shipTarget = null;
    (int, int) targetedPosition = (-1, -1);
    //directions
    /*
        0 = up (0,1)
        1 = down (0, -1)
        2 = right (1,0)
        3 = left (-1,0)
     */
    /*Priority list
     * 0 = treasure
     * 1 = shipwright /shop
     * 2 = enemy boats
     */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GridManager.instance;
        gm.treasureFound += TreasureFound;
    }
    public void SetPlayer()
    {
        isPlayer = true;
        indicator = Instantiate(display, transform);
        indicator.transform.SetParent(transform);
    }
    private void Update()
    {
        if (!isPlayer)
        {
            if (treasureSpot != (-1, -1))
            {
                priority = 0;
            }
        }
        else
        {
            gm.targetMenu.value = priority;
        }
    }
    private void OnDestroy()
    {
        gm.treasureFound -= TreasureFound;
    }
    public void simulateNextStep()
    {
        if (!dead)
        {
            if (heardShots > 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    heardShots--;
                }
            }
            if (firePower > 0 && !gm.grid[currentPos.Item1, currentPos.Item2].State.shopDock)
            {
                for (int i = 0; i < maxFirePower; i += 5)
                {
                    if (firePower > 0)
                    {
                        ShootBoat();
                    }
                }
            }
            pathUpdateFrequency--;
            if (path[path.Length - 1] == (-1, -1) || pathUpdateFrequency <= 0)
            {
                GetPathToTarget();
                pathUpdateFrequency = 5;
            }
            bool nextStepFound = false;
            for (int i = 0; i < path.Length; i++)
            {
                if (nextStepFound)
                {
                    CellInfo cellInfo = gm.grid[path[i].Item1, path[i].Item2].State.Copy();
                    if (cellInfo.occupied && cellInfo.shopDock)
                    {
                        gm.grid[currentPos.Item1, currentPos.Item2].UnSetShip();
                        GetPathToTarget();
                        pathUpdateFrequency = 5;
                        ModifyDirection(currentPos, path[1]);
                        currentPos = path[1];
                        gm.grid[currentPos.Item1, currentPos.Item2].SetShip(this);
                    }
                    else
                    {
                        ShipScript[] shipsInArea = cellInfo.CheckForBoats();
                        bool found = false;
                        if (shipsInArea != null && shipsInArea.Length > 0)
                        {
                            for (int j = 0; j < shipsInArea.Length; j++)
                            {
                                (int, int)[] boatPath = shipsInArea[j].path;

                                for (int k = 0; k < boatPath.Length; k++)
                                {
                                    if (boatPath[k] == path[i])
                                    {
                                        found = true;
                                    }
                                }
                            }
                            if (found)
                            {
                                gm.grid[currentPos.Item1, currentPos.Item2].UnSetShip();
                                GetPathToTarget();
                                pathUpdateFrequency = 5;
                                if (path.Length > 1)
                                {
                                    ModifyDirection(currentPos, path[1]);
                                    currentPos = path[1];
                                    gm.grid[currentPos.Item1, currentPos.Item2].SetShip(this);
                                }
                                else
                                {
                                    gm.grid[currentPos.Item1, currentPos.Item2].SetShip(this);
                                }
                                break;
                            }
                            else
                            {
                                ModifyDirection(currentPos, path[i]);
                                gm.grid[currentPos.Item1, currentPos.Item2].UnSetShip();
                                currentPos = path[i];
                                gm.grid[currentPos.Item1, currentPos.Item2].SetShip(this);
                            }
                        }
                        else
                        {
                            ModifyDirection(currentPos, path[i]);
                            gm.grid[currentPos.Item1, currentPos.Item2].UnSetShip();
                            currentPos = path[i];
                            gm.grid[currentPos.Item1, currentPos.Item2].SetShip(this);
                        }
                    }

                }
                else
                {
                    if (path[i] == currentPos)
                    {
                        nextStepFound = true;
                    }
                }
            }
            if (isPlayer)
            {
                if (gm.availableDocks.Contains(currentPos))
                {
                    treasureSpot = gm.treasure;
                    firePower = maxFirePower;
                    gm.OpenShop();
                }
                else
                {
                    gm.CloseShop();
                }
            }
            else
            {
                if (gm.availableDocks.Contains(currentPos))
                {
                    GetPathToTarget();
                    pathUpdateFrequency = 5;
                }
            }
            if (currentPos == treasureSpot)
            {
                reputation += 5;
                treasure += Random.Range(25, 36);
                //Debug.Log(treasure);
                gm.GrabbedTreaure();
                GetPathToTarget();
                pathUpdateFrequency = 5;
            }
            transform.position = new Vector3(currentPos.Item1, transform.position.y, currentPos.Item2);
        }
        else
        {
            if (!isPlayer)
            {
                if (respawnTimer > 0)
                {
                    respawnTimer--;
                }
                else
                {
                    for (int i = 0; i < gm.availableDocks.Count; i++)
                    {
                        if (gm.grid[gm.availableDocks[i].Item1, gm.availableDocks[i].Item2].State.occupied)
                        {
                            continue;
                        }
                        else
                        {
                            respawnTimer = 10;
                            dead = false;
                            shipDesign.SetActive(true);
                            hullHP = maxHull;
                            firePower = maxFirePower;
                            gm.grid[gm.availableDocks[i].Item1, gm.availableDocks[i].Item2].SetShip(this);
                            currentPos = gm.availableDocks[i];
                            priority = 1;
                            transform.position = new Vector3(currentPos.Item1, transform.position.y, currentPos.Item2);
                            gm.respawnQueue.Remove(this);
                            gm.currentShips.Add(this);
                            break;
                        }
                    }
                }
            }
        }
    }
    void ModifyDirection((int, int) prev, (int, int) next)
    {
        int dirX = prev.Item1 - next.Item1;
        int dirY = prev.Item2 - next.Item2;
        if ((dirX, dirY) == (0, 1))
        {
            direction = -1;
            shipDesign.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else if ((dirX, dirY) == (-1, 0))
        {
            direction = 2;
            shipDesign.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        else if ((dirX, dirY) == (0, -1))
        {
            direction = 0;
            shipDesign.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            direction = 3;
            shipDesign.transform.localEulerAngles = new Vector3(0, 270, 0);
        }
    }
    void ShootBoat()
    {
        int fakeReputation = reputation;
        int minX = Mathf.Clamp(currentPos.Item1 - 2, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(currentPos.Item2 - 2, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(currentPos.Item1 + 2, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(currentPos.Item2 + 2, 0, GridManager.instance.height - 1);
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (i == currentPos.Item1 && j == currentPos.Item2)
                {
                    continue;
                }
                ShipScript cellInfo = gm.grid[i, j].GetShip();
                if (cellInfo != null && firePower > 0 && cellInfo != this && !gm.grid[i, j].State.shopDock)
                {
                    int distanceToBoatX = i - currentPos.Item1;
                    int distanceToBoatY = j - currentPos.Item2;
                    if (direction == 0) //up
                    {
                        if (Mathf.Abs(distanceToBoatX) < 2 && distanceToBoatY < 2)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                        else if (Mathf.Abs(distanceToBoatX) == 2 && distanceToBoatY == 0)
                        {
                            if (Random.Range(0, 4) == 0)
                            {
                                firePower--;
                                if (cellInfo.GetHit())
                                {
                                    reputation += 5;
                                    treasure += cellInfo.reputation;
                                }
                            }
                            else
                            {
                                firePower--;
                                cellInfo.Missfire();
                            }
                        }
                        else if (distanceToBoatY == 2 && distanceToBoatX == 0)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }

                    }
                    else if (direction == 1) //down
                    {
                        if (Mathf.Abs(distanceToBoatX) < 2 && distanceToBoatY > -2)
                        {

                        }
                        else if (Mathf.Abs(distanceToBoatX) == 2 && distanceToBoatY == 0)
                        {
                            if (Random.Range(0, 4) == 0)
                            {
                                firePower--;
                                if (cellInfo.GetHit())
                                {
                                    reputation += 5;
                                    treasure += cellInfo.reputation;
                                }
                            }
                            else
                            {
                                firePower--;
                                cellInfo.Missfire();
                            }
                        }
                        else if (distanceToBoatY == -2 && distanceToBoatX == 0)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                    }
                    else if (direction == 2) //right
                    {
                        if (Mathf.Abs(distanceToBoatY) < 2 && distanceToBoatX < 2)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                        else if (Mathf.Abs(distanceToBoatY) == 2 && distanceToBoatX == 0)
                        {
                            if (Random.Range(0, 4) == 0)
                            {
                                firePower--;
                                if (cellInfo.GetHit())
                                {
                                    reputation += 5;
                                    treasure += cellInfo.reputation;
                                }
                            }
                            else
                            {
                                firePower--;
                                cellInfo.Missfire();
                            }
                        }
                        else if (distanceToBoatX == 2 && distanceToBoatY == 0)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                    }
                    else if (direction == 3) //left
                    {
                        if (Mathf.Abs(distanceToBoatY) < 2 && distanceToBoatX > -2)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                        else if (Mathf.Abs(distanceToBoatY) == 2 && distanceToBoatX == 0)
                        {
                            if (Random.Range(0, 4) == 0)
                            {
                                firePower--;
                                if (cellInfo.GetHit())
                                {
                                    reputation += 5;
                                    treasure += cellInfo.reputation;
                                }
                            }
                            else
                            {
                                firePower--;
                                cellInfo.Missfire();
                            }
                        }
                        else if (distanceToBoatX == -2 && distanceToBoatY == 0)
                        {
                            firePower--;
                            if (cellInfo.GetHit())
                            {
                                reputation += 5;
                                treasure += cellInfo.reputation;
                            }
                        }
                    }
                }

            }
        }
        if (reputation > fakeReputation)
        {
            GetPathToTarget();
        }
    }
    public void GetPathToTarget()
    {
        if (priority == 0)
        {
            shipTarget = null;
            if (treasureSpot != (-1, -1))
            {
                if (currentPos != treasureSpot)
                {
                    float cost;
                    (cost, path) = AStarAlgorithm.AStar(currentPos, treasureSpot, gm.traversableMap, this);
                }
                else
                {
                    treasureSpot = (-1, -1);
                    priority = 1;
                    //Collect Treasure
                }
            }
            else
            {
                //Re Assign priority to shipwright
                priority = 1;
            }

        }
        else if (priority == 1)
        {
            shipTarget = null;
            if (!gm.availableDocks.Contains(path[path.Length - 1]) && !gm.availableDocks.Contains(currentPos))
            {
                float minCost = 99999999999999;
                priority = 1;
                for (int i = 0; i < gm.availableDocks.Count; i++)
                {
                    if (!gm.grid[gm.availableDocks[i].Item1, gm.availableDocks[i].Item2].State.occupied)
                    {
                        (int, int)[] tempPath = new (int, int)[1];
                        float tempCost = 0;
                        (tempCost, tempPath) = AStarAlgorithm.AStar(currentPos, gm.availableDocks[i], gm.traversableMap, this);
                        if (path[0] != (-1, -1))
                        {
                            if (tempCost < minCost)
                            {
                                path = tempPath;
                                currentTarget = gm.availableDocks[i];
                            }
                        }
                        else
                        {
                            path = tempPath;
                        }
                    }
                }
            }
            else
            {
                if (gm.availableDocks.Contains(currentPos))
                {
                    treasureSpot = gm.treasure;
                    firePower = maxFirePower;
                    if (isPlayer)
                    {
                        //OpenShop
                        gm.OpenShop();
                    }
                    else
                    {
                        if (treasure > 4 && maxHull < hullHP)
                        {
                            treasure -= 5;
                            hullHP++;
                        }
                        if (treasure >= 50 && helmsmanLevel < 3)
                        {
                            helmsmanLevel++;
                            treasure -= 50;
                        }
                        if (treasure >= 25 && maxHull < 5)
                        {
                            treasure -= 25;
                            maxHull++;
                            hullHP++;
                        }
                        if (treasure >= 10)
                        {
                            treasure -= 10;
                            maxFirePower++;
                            firePower = maxFirePower;
                        }
                        //Shop for the bots
                        priority = 0;
                        GetPathToTarget();
                    }
                }
                else
                {
                    float minCost = 99999999999999;
                    priority = 1;
                    for (int i = 0; i < gm.availableDocks.Count; i++)
                    {
                        (int, int)[] tempPath = new (int, int)[1];
                        float tempCost = 0;
                        (tempCost, tempPath) = AStarAlgorithm.AStar(currentPos, gm.availableDocks[i], gm.traversableMap, this);
                        if (path[0] != (-1, -1))
                        {
                            if (tempCost < minCost)
                            {
                                path = tempPath;
                                minCost = tempCost;
                                currentTarget = gm.availableDocks[i];
                            }
                        }
                        else
                        {
                            path = tempPath;
                        }
                    }
                }

            }
        }
        else if (priority == 2)
        {
            ShipScript targetShip = gm.currentShips[0];
            float closestExpectedShip = 999999;
            for (int i = 0; i < gm.currentShips.Count; i++)
            {
                float calcDistance = Mathf.Abs(currentPos.Item1 - gm.currentShips[i].currentPos.Item1) + Mathf.Abs(currentPos.Item2 - gm.currentShips[i].currentPos.Item2);
                if (calcDistance < closestExpectedShip && this != gm.currentShips[i])
                {
                    closestExpectedShip = calcDistance;
                    targetShip = gm.currentShips[i];
                }
            }
            targetedPosition = (targetShip.currentPos.Item1 + targetShip.GetDirection().Item1 * -1, targetShip.currentPos.Item2 + targetShip.GetDirection().Item2 * -1);
            (closestExpectedShip, path) = AStarAlgorithm.AStar(currentPos, targetedPosition, gm.traversableMap, this);
            shipTarget = targetShip;

        }

    }
    public (int, int) GetDirection()
    {
        if (direction == 0)
        {
            return (0, 1);
        }
        else if (direction == 1)
        {
            return (0, -1);
        }
        else if (direction == 2)
        {
            return (1, 0);
        }
        else
        {
            return (-1, 0);
        }
    }
    public bool GetHit()
    {
        heardShots++;
        bool sunk = false;
        hullHP--;
        if (hullHP <= 0)
        {
            dead = true;
            respawnTimer = 10;
            reputation -= 5;
            reputation = Mathf.Clamp(reputation, 5, int.MaxValue);
            treasure = 0;
            sunk = true;
            shipDesign.SetActive(false);
            gm.grid[currentPos.Item1, currentPos.Item2].UnSetShip();
            gm.currentShips.Remove(this);
            if (isPlayer)
            {
                gm.EndGame();
            }
            else
            {
                gm.respawnQueue.Add(this);
            }
        }
        return sunk;
    }
    public void Missfire()
    {
        heardShots++;
    }
    public void TreasureFound()
    {
        if (treasureSpot != (-1, -1))
        {
            treasureSpot = (-1, -1);
            if (priority == 0)
            {
                priority = 1;
            }

        }
    }
    public void InitializeBoat(int x, int y, bool player)
    {
        isPlayer = player;
        currentPos = (x, y);
        if (isPlayer )
        {
            SetPlayer();
        }
    }

    public ArrayList GetShipSummary()
    {
        string knowsAboutTreasure = "No";
        if (treasureSpot != (-1, -1))
        {
            knowsAboutTreasure = "Yes";
        }
        ArrayList summary = new ArrayList() { hullHP, maxHull, firePower, maxFirePower, currentPos, helmsmanLevel, treasure, reputation, knowsAboutTreasure };
        return summary;
    }
    public void SetPriority(int newPriority)
    {
        if (isPlayer)
        {
            priority = newPriority;
            GetPathToTarget();
        }

    }
}
