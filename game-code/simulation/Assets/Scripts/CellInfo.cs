using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CellInfo
{

    public int state = 0;
    public int x = 0;
    public int y = 0;
    public string description = "Desert Land";
    //Celsius
    public float temperature = 20f;
    // 0 -100 Scale
    public float humidity = 10;
    int animals = 0;
    //Only grows during the forest scale
    public int age = 0;
    public void UpdateState()
    {
        checkNeighborsInRange(1);

        if (temperature > 40)
        {
            temperature += 0.25f;
            TryToDevolve();
        }
        else if (temperature > 25)
        {
            humidity *= 0.95f;
        }
        else if (temperature < 15)
        {
            humidity *= 0.95f;
            temperature--;
        }
        else if (temperature < 5)
        {
            TryToDevolve();
            temperature -= 0.5f;
        }
        if (humidity > 90)
        {
            if (state == 0)
            {
                state = 1;
                animals = 0;
                age = 0;
            }
        }
        else if (humidity > 40)
        {
            if (temperature > 25)
            {
                temperature--;
            }
            if (state == 0)
            {
                state = 2;
            }
        }
        //Modify qualities based on states
        if (state == 1)
        {
            temperature -= 0.75f;
            humidity *= 1.1f;
            humidity = Mathf.Clamp(humidity, 0f, 100f);
        }
        else if (state == 2)
        {
            temperature += 0.25f;
            if (temperature < 25 && temperature > 15)
            {
                if (Random.Range(0, 4) == 3)
                {
                    animals += 1;
                }
            }
        }
        else if (state == 0)
        {
            temperature += 0.75f;
            if (temperature < 25 && temperature > 15)
            {
                if (Random.Range(0, 8) == 7)
                {
                    animals += 1;
                }
            }
        }
        else if (state == 3)
        {
            temperature -= 0.25f;
            if (age < 5 && temperature < 30)
            {
                if (Random.Range(0, 5) == 3)
                {
                    age += 1;
                }
            }
            for (int i = 0; i < age; i++)
            {
                if (Random.Range(0, 4) == 1)
                {
                    humidity += 0.5f;
                    humidity = Mathf.Clamp(humidity, 0f, 100f);
                }
            }
            if (animals > 50)
            {
                if (age > 0)
                {
                    age -= 1;
                }
                else
                {
                    age = 0;
                    state = 2;
                }
            }
            else
            {
                if (Random.Range(0, 2) == 1)
                {
                    animals += 1;
                }
                else if (Random.Range(0, 16) == 1)
                {
                    animals *= 2;
                }
            }
        }
        else if (state == 4)
        {
            humidity *= 1.05f;
            humidity = Mathf.Clamp(humidity, 0f, 100f);
            if (Random.Range(0, 16) == 1)
            {
                temperature += 5f;
            }
        }
        EnsureName();
    }
    void checkNeighborsInRange(int Distance)
    {
        float tempHumid = humidity;
        float tempClimate = temperature;
        int neighborNumber = 0;
        int forestNeighbors = 0;
        int minX = Mathf.Clamp(x - Distance, 0, GridManager.instance.width - 1);
        int minY = Mathf.Clamp(y - Distance, 0, GridManager.instance.height - 1);
        int maxX = Mathf.Clamp(x + Distance, 0, GridManager.instance.width - 1);
        int maxY = Mathf.Clamp(y + Distance, 0, GridManager.instance.height - 1);
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {

                if (i ==x && j == y)
                {
                    continue;
                }
                CellInfo neighbor = GridManager.instance.grid[i, j].State.Copy();
                if (Distance == 1)
                {
                    neighborNumber++;
                    tempHumid += neighbor.humidity;
                    tempClimate += neighbor.temperature;
                    if (state == 2 || state == 0)
                    {
                        if (neighbor.state == 3)
                        {
                            if (neighbor.age >= 5)
                            {
                                if (Random.Range(0, 4) == 3)
                                {
                                    if (state == 2)
                                    {
                                        state = 3;
                                        age = 1;
                                        if (temperature > 30)
                                        {
                                            tempClimate -= 10;
                                        }
                                    }
                                }
                            }
                            forestNeighbors++;
                        }
                    }
                }

            }
        }
        if (tempHumid != humidity)
        {
            tempHumid /= (neighborNumber + 1);
            humidity = tempHumid;
        }
        if (tempClimate != temperature)
        {
            tempClimate /= (neighborNumber + 1);
            temperature = tempClimate;
        }
        if ((state == 0 || state == 2) && forestNeighbors == neighborNumber)
        {
            SetState(4, x, y);
        }
    }
    void TryToDevolve()
    {
        if (animals > 0)
        {
            animals = Mathf.RoundToInt(animals * 0.5f);
        }
        humidity *= 0.7f;
        if (state != 0)
        {
            if (state == 1)
            {
                if (humidity < 40)
                {
                    state = 0;
                }
            }
            else if (state == 2)
            {
                if (humidity < 20 && animals < 5)
                {
                    state = 0;
                    animals = 0;
                }
            }
            else if (state == 3)
            {
                if (age > 1)
                {
                    age--;
                }
                else
                {
                    age = 0;
                    state = 2;
                }
            }
        }
    }

    void EnsureName()
    {
        if (state == 0)
        {
            description = "Desert Land";
        }
        else if (state == 1)
        {
            description = "Water";
        }
        else if (state == 2)
        {
            description = "Fertile Land";
        }
        else if (state == 3)
        {
            description = "Forest";
        }
        else if (state == 4)
        {
            description = "Volcano";
        }
    }

    public void SetState(int newState, int x, int y)
    {
        this.x = x; this.y = y;
        if (newState == 0)
        {
            if (state != 1)
            {
                state = 0;
                temperature += 1;
            }
            if (animals > 25)
            {
                animals = Mathf.FloorToInt(animals / 2);
            }
        }
        else if (newState == 1)
        {
            state = 1;
            humidity = 80f;
        }
        else if (newState == 2)
        {
            if (state == 0)
            {
                state = 2;
            }
        }
        else if (newState == 3)
        {
            if (state == 2)
            {
                state = 3;
                age = 1;
                if (temperature > 30)
                {
                    temperature -= 10;
                }
            }
        }
        else if (newState == 4)
        {
            state = 4;
            temperature += 15f;
        }
        EnsureName();
    }
    public ArrayList GetSummary()
    {
        ArrayList summary = new ArrayList() { description, humidity, temperature, animals };
        return summary;
    }

    public CellInfo Copy()
    {
        return new CellInfo()
        {
            state = this.state,
            description = this.description,
            humidity = this.humidity,
            temperature = this.temperature,
            animals = this.animals,
            age = this.age,
            x = this.x,
            y = this.y
        };
    }
}
