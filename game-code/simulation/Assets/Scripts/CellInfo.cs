using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellInfo
{

    public int state = 0;
    public int x = 0;
    public int y = 0;
    public string description = "Water";
    public int tideLevel = 0;
    public bool isShop = false;
    public bool occupied = false;
    public bool traversable = true;
    public bool shopDock = false;
    int highTideTimer = 0;
    int waterActivity = 0;
    bool oddCheck = true;
    bool goingUp = true;
    public void UpdateState()
    {
        if (state > 0)
        {
            if (waterActivity > 0)
            {
                if (oddCheck)
                {
                    waterActivity--;
                }
            }
            if (occupied)
            {
                waterActivity++;
            }
            if (tideLevel < 3)
            {
                int prevTide = tideLevel;
                checkNeighborsInRange(1);
                if (tideLevel == prevTide)
                {
                    if (GridManager.instance.weather == 2)
                    {
                        if (Random.Range(0, 4) == 0)
                        {
                            tideLevel++;
                            goingUp = true;
                        }
                    }
                    else
                    {
                        if (waterActivity > 6)
                        {
                            tideLevel++;
                        }
                        else
                        {
                            if (!goingUp)
                            {
                                if (tideLevel == 1)
                                {
                                    goingUp = true;
                                }
                                else
                                {
                                    if (Random.Range(0, 2) == 0)
                                    {
                                        tideLevel--;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            else
            {

                if (highTideTimer < 3)
                {
                    highTideTimer++;
                }
                else
                {
                    highTideTimer = 0;
                    tideLevel = 2;
                    goingUp = false;
                }
            }

        }
        oddCheck = !oddCheck;
    }
    void checkNeighborsInRange(int Distance)
    {
        bool shopInRange = false;
        int tideRiseChance = 0;
        int minX = Mathf.Clamp(x - Distance, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(y - Distance, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(x + Distance, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(y + Distance, 0, GridManager.instance.height - 1);
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {

                if (i == x && j == y)
                {
                    continue;
                }
                CellInfo neighbor = GridManager.instance.grid[i, j].State.Copy();
                if (Distance == 4)
                {
                    if (neighbor.state == 0)
                    {
                        if (neighbor.isShop)
                        {
                            shopInRange = true;
                        }
                    }
                }
                if (Distance == 1)
                {
                    if (neighbor.state > 0)
                    {
                        if (neighbor.tideLevel > tideLevel)
                        {
                            tideRiseChance++;
                        }
                        if (neighbor.occupied)
                        {
                            waterActivity += 2;
                        }
                    }
                }
            }
        }
        if (!shopInRange)
        {
            if (state == 0)
            {
                isShop = true;
            }
        }
        for (int i = 0; i < tideRiseChance; i++)
        {
            if (Random.Range(0, 5) == 0)
            {
                if (goingUp)
                {
                    tideLevel++;
                    break;
                }
            }
        }

    }

    public ShipScript[] CheckForBoats()
    {
        int minX = Mathf.Clamp(x - 1, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(y - 1, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(x + 1, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(y + 1, 0, GridManager.instance.height - 1);
        List<ShipScript> shipsInRange = new List<ShipScript>();
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {

                if (i == x && j == y)
                {
                    continue;
                }
                CellInfo neighbor = GridManager.instance.grid[i, j].State.Copy();
                if (neighbor.occupied)
                {
                    shipsInRange.Add(GridManager.instance.grid[i, j].GetShip());
                }
            }
        }
        return shipsInRange.ToArray();
    }
    public void PlaceShops()
    {
        checkNeighborsInRange(4);
    }
    public void PreStartUpdates()
    {
        int minX = Mathf.Clamp(x - 1, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(y - 1, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(x + 1, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(y + 1, 0, GridManager.instance.height - 1);
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {

                if (i == x && j == y)
                {
                    continue;
                }
                CellInfo neighbor = GridManager.instance.grid[i, j].State.Copy();
                if (neighbor.state == 0)
                {
                    if (neighbor.isShop)
                    {
                        if (x == neighbor.x || y == neighbor.y)
                        {
                            if (state > 0)
                            {
                                shopDock = true;
                            }
                        }
                    }
                }
            }
        }
    }
    public void OccupationToggle()
    {
        occupied = !occupied;
    }

    public void SetState(int newState)
    {
        state = newState;
        if (state > 0)
        {
            traversable = true;
            description = "Ocean";
            if (state == 1)
            {
                tideLevel = 1;
            }
            else if (state == 2)
            {
                tideLevel = 2;
            }
            else
            {
                tideLevel = 3;
            }
        }
        else
        {
            traversable = false;
            description = "Land";
        }
    }


    public CellInfo Copy()
    {
        return new CellInfo()
        {
            state = this.state,
            x = this.x,
            y = this.y,
            description = this.description,
            tideLevel = this.tideLevel,
            isShop = this.isShop,
            occupied = this.occupied,
            traversable = this.traversable,
            shopDock = this.shopDock,
            highTideTimer = this.highTideTimer,
            waterActivity = this.waterActivity,
            oddCheck = this.oddCheck,
            goingUp = this.goingUp
        };
    }
    public ArrayList GetSummary()
    {
        ArrayList summary = new ArrayList() {description, tideLevel, x, y};
        return summary;
    }
}
