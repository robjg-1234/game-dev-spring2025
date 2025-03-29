using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    private static (int, int)[] GetNeighbors(int posX, int posY, bool[,] map, (int, int) direction)
    {
        List<(int, int)> neighbors = new List<(int, int)>();
        (int, int) avoidNode = (posX + direction.Item1 * -1, posY + direction.Item2 * -1);
        if (GridManager.instance.availableDocks.Contains((posX, posY)))
        {
            avoidNode = (-1,-1);
        }
        
        if (posX - 1 >= 0)
        {
            if (map[posX - 1, posY])
            {
                if (avoidNode != (posX - 1, posY))
                {
                    neighbors.Add((posX - 1, posY));
                }

            }
        }
        if (posY - 1 >= 0)
        {
            if (map[posX, posY - 1])
            {
                if (avoidNode != (posX, posY - 1))
                {
                    neighbors.Add((posX, posY - 1));
                }
            }
        }
        if (posX + 1 < map.GetLength(0))
        {
            if (map[posX + 1, posY])
            {
                if (avoidNode != (posX + 1, posY))
                {
                    neighbors.Add((posX + 1, posY));
                }
            }
        }
        if (posY + 1 < map.GetLength(1))
        {
            if (map[posX, posY + 1])
            {
                if (avoidNode != (posX, posY + 1))
                {
                    neighbors.Add((posX, posY + 1));
                }
            }
        }

        if (neighbors.Count > 0)
        {
            return neighbors.ToArray();
        }
        else
        {
            return new (int, int)[] { (-1, -1) };
        }
    }
    static (float, (int, int)[]) BacktrackPath((int, int)[,] relationMap, (int, int) currentSpot, float[,] fScoreMap)
    {
        int maxLengthForPath = 50;
        int currentPathLength = 0;
        float totalCost = 0;
        List<(int, int)> path = new List<(int, int)>();
        (int, int) currentNode = currentSpot;
        while (currentNode != (-1, -1))
        {
            path.Add(currentNode);
            totalCost += fScoreMap[currentNode.Item1, currentNode.Item2];
            currentNode = relationMap[currentNode.Item1, currentNode.Item2];
            currentPathLength++;
            if (currentPathLength == maxLengthForPath)
            {
                path = new List<(int, int)>() { (-1,-1)};
                return (999999, path.ToArray());
            }
        }
        path.Reverse();
        return (totalCost, path.ToArray());
    }
    static float AnalyzeCostFromNeighbors(ShipScript currentShip, (int, int) node, (int,int) expDirection)
    {
        int dir;
        if (expDirection == (0, 1))
        {
            dir = 0;
        }
        else if (expDirection == (1, 0))
        {
            dir = 2;
        }
        else if (expDirection == (0, -1))
        {
            dir = 1;
        }
        else
        {
            dir = 3;
        }
        int minX = Mathf.Clamp(node.Item1 - 2, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(node.Item2 - 2, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(node.Item1 + 2, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(node.Item2 + 2, 0, GridManager.instance.height - 1);
        float expectedCost = 0f;
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (i == node.Item1 && j == node.Item2)
                {
                    continue;
                }
                ShipScript cellInfo = GridManager.instance.grid[i, j].GetShip();
                if (cellInfo != null && cellInfo != currentShip)
                {
                    int distanceToBoatX = node.Item1 - i;
                    int distanceToBoatY = node.Item2 - j;
                    if (cellInfo.direction == 0) //up
                    {
                        if (Mathf.Abs(distanceToBoatX) < 2 && distanceToBoatY < 2)
                        {
                            expectedCost += 5000f;
                        }
                        else if (Mathf.Abs(distanceToBoatX) == 2 && distanceToBoatY == 0)
                        {
                            expectedCost += 10f * (cellInfo.reputation / currentShip.reputation);
                        }
                        else if (distanceToBoatY == 2 && distanceToBoatX == 0)
                        {
                            expectedCost += 15f * (cellInfo.reputation / currentShip.reputation);
                            if (dir == cellInfo.direction)
                            {
                                expectedCost += 100f;
                            }
                            else
                            {
                                if (cellInfo.hullHP < currentShip.firePower)
                                {
                                    expectedCost += (cellInfo.hullHP - currentShip.firePower) * 0.75f * 2 + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                                else
                                {
                                    expectedCost += 20f + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                            }
                        }
                        else
                        {
                            if (currentShip.firePower > 0)
                            {
                                expectedCost += -10f * (currentShip.reputation / cellInfo.reputation);
                            }
                            else
                            {
                                expectedCost += 0f;
                            }
                        }
                    }
                    else if (cellInfo.direction == 1) //down
                    {
                        if (Mathf.Abs(distanceToBoatX) < 2 && distanceToBoatY > -2)
                        {
                            expectedCost += 5000f;
                        }
                        else if(Mathf.Abs(distanceToBoatX) == 2 && distanceToBoatY == 0)
                        {
                            expectedCost += 10f * (cellInfo.reputation / currentShip.reputation);
                        }
                        else if (distanceToBoatY == -2 && distanceToBoatX == 0)
                        {
                            expectedCost += 15f * (cellInfo.reputation / currentShip.reputation);
                            if (dir == cellInfo.direction)
                            {
                                // Assumes it always goes in the same direction Ill change this if i am not lazy
                                expectedCost += 100f;
                            }
                            else
                            {
                                if (cellInfo.hullHP < currentShip.firePower)
                                {
                                    expectedCost += (cellInfo.hullHP - currentShip.firePower) * 0.75f * 2 + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                                else
                                {
                                    expectedCost += 20f + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                            }
                        }
                        else
                        {
                            if (currentShip.firePower > 0)
                            {
                                expectedCost += -10f * (currentShip.reputation / cellInfo.reputation);
                            }
                            else
                            {
                                expectedCost += 0f;
                            }
                        }
                    }
                    else if (cellInfo.direction == 2) //right
                    {
                        if (Mathf.Abs(distanceToBoatY) < 2 && distanceToBoatX < 2)
                        {
                            expectedCost += 5000f;
                        }
                        else if(Mathf.Abs(distanceToBoatY) == 2 && distanceToBoatX == 0)
                        {
                            expectedCost += 10f * (cellInfo.reputation / currentShip.reputation);
                        }
                        else if (distanceToBoatX == 2 && distanceToBoatY == 0)
                        {
                            expectedCost += 15f * (cellInfo.reputation / currentShip.reputation);
                            if (dir == cellInfo.direction)
                            {
                                expectedCost += 100f;
                            }
                            else
                            {
                                if (cellInfo.hullHP < currentShip.firePower)
                                {
                                    expectedCost += (cellInfo.hullHP - currentShip.firePower) * 0.75f * 2 + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                                else
                                {
                                    expectedCost += 20f + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                            }
                        }
                        else
                        {
                            if (currentShip.firePower > 0)
                            {
                                expectedCost += -10f * (currentShip.reputation / cellInfo.reputation);
                            }
                            else
                            {
                                expectedCost += 0f;
                            }
                        }
                    }
                    else if (cellInfo.direction == 3) //left
                    {
                        if (Mathf.Abs(distanceToBoatY) < 2 && distanceToBoatX > -2)
                        {
                            expectedCost += 5000f;
                        }
                        else if(Mathf.Abs(distanceToBoatY) == 2 && distanceToBoatX == 0)
                        {
                            expectedCost += 10f * (cellInfo.reputation / currentShip.reputation);
                        }
                        else if (distanceToBoatX == -2 && distanceToBoatY == 0)
                        {
                            expectedCost += 15f * (cellInfo.reputation / currentShip.reputation);
                            if (dir == cellInfo.direction)
                            {
                                expectedCost += 100f;
                            }
                            else
                            {
                                if (cellInfo.hullHP < currentShip.firePower)
                                {
                                    expectedCost += (cellInfo.hullHP - currentShip.firePower) * 0.75f * 2 + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                                else
                                {
                                    expectedCost += 20f + (currentShip.hullHP - currentShip.heardShots) * 2;
                                }
                            }
                        }
                        else
                        {
                            if (currentShip.firePower > 0)
                            {
                                expectedCost += -10f * (currentShip.reputation / cellInfo.reputation);
                            }
                            else
                            {
                                expectedCost += 0f;
                            }
                        }
                    }
                }
            }
        }
        return expectedCost;
    }
    static float CostCalculation((int, int) currentNode, (int, int) endPoint, ShipScript currentShip)
    {
        (int, int) expectedDirection = (endPoint.Item1 - currentNode.Item1, endPoint.Item2 - currentNode.Item2);
        float calculatedCost = 0f;
        CellScript cell = GridManager.instance.grid[endPoint.Item1, endPoint.Item2];
        calculatedCost += AnalyzeCostFromNeighbors(currentShip, endPoint, expectedDirection);
        CellInfo info = cell.State.Copy();
        if (info.occupied)
        {
            calculatedCost += 30000;
        }
        calculatedCost += info.tideLevel * 20f / currentShip.helmsmanLevel;
        return calculatedCost;
    }
    static float HeuristicFormula((int, int) currentNode, (int, int) endPoint)
    {
        return Mathf.Abs(currentNode.Item1 - endPoint.Item1) + Mathf.Abs(currentNode.Item2 - endPoint.Item2);
    }
    public static (float, (int, int)[]) AStar((int, int) startingPoint, (int, int) goalPoint, bool[,] map, ShipScript myship)
    {
        float totalCost = 99999999f;
        ShipScript shipCalculating = myship;
        (int, int) curDir = shipCalculating.GetDirection();
        int nodesVisits = 0;
        Dictionary<(int, int), float> nodeQueue = new Dictionary<(int, int), float>();
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
        List<(int, int)> nodesVisited = new List<(int, int)>();
        (int, int)[] foundPath = new (int, int)[1] { (-1, -1) };
        float[,] gScore = new float[mapWidth, mapHeight];
        float[,] fScore = new float[mapWidth, mapHeight];
        (int, int)[,] relationMap = new (int, int)[mapWidth, mapHeight];
        relationMap[startingPoint.Item1, startingPoint.Item2] = (-1, -1);
        if (shipCalculating == null)
        {
            return (totalCost, foundPath);
        }
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                gScore[i, j] = float.MaxValue;
                fScore[i, j] = float.MaxValue;
            }
        }
        gScore[startingPoint.Item1, startingPoint.Item2] = 0f;
        fScore[startingPoint.Item1, startingPoint.Item2] = HeuristicFormula(startingPoint, goalPoint);

        nodeQueue.Add(startingPoint, fScore[startingPoint.Item1, startingPoint.Item2]);
        while (nodeQueue.Count > 0)
        {
            nodesVisits++;
            (int, int) currentNode = (-1, -1);
            foreach (KeyValuePair<(int, int), float> keys in nodeQueue)
            {
                if (currentNode == (-1, -1))
                {
                    currentNode = keys.Key;
                }
                else
                {
                    if (keys.Value < nodeQueue[currentNode])
                    {
                        currentNode = keys.Key;
                        if (currentNode != startingPoint)
                        {
                            curDir = DirectionRelation(currentNode, relationMap);
                        }
                    }
                }
            }

            if (currentNode == goalPoint)
            {
                (totalCost, foundPath) = BacktrackPath(relationMap, currentNode, fScore);
                if (myship.isPlayer)
                {
                    GridManager.instance.costMap = fScore;
                }
                return (totalCost, foundPath);
            }
            else
            {
                nodesVisited.Add(currentNode);
                nodeQueue.Remove(currentNode);
                foreach ((int, int) i in GetNeighbors(currentNode.Item1, currentNode.Item2, map, curDir))
                {
                    if (i != (-1, -1))
                    {
                        float tentativeGScore = gScore[currentNode.Item1, currentNode.Item2] + CostCalculation(currentNode, i, shipCalculating);
                        if (tentativeGScore < gScore[i.Item1, i.Item2])
                        {
                            relationMap[i.Item1, i.Item2] = currentNode;
                            gScore[i.Item1, i.Item2] = tentativeGScore;
                            fScore[i.Item1, i.Item2] = tentativeGScore + HeuristicFormula(i, goalPoint);
                        }
                        if (!nodesVisited.Contains(i))
                        {
                            if (!nodeQueue.ContainsKey(i))
                            {
                                nodeQueue.Add(i, fScore[i.Item1, i.Item2]);
                            }
                            else
                            {
                                nodeQueue[i] = fScore[i.Item1, i.Item2];
                            }
                        }
                    }
                }
            }
        }


        return (totalCost, foundPath);
    }
    static (int, int) DirectionRelation((int, int) point, (int, int)[,] from)
    {
        int x = point.Item1 - from[point.Item1, point.Item2].Item1;
        int y = point.Item2 - from[point.Item1, point.Item2].Item2;
        return (x, y);
    }
}
