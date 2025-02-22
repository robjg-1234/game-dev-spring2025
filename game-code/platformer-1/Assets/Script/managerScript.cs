using System;
using UnityEngine;

public class managerScript : MonoBehaviour
{
    public static managerScript instance;
    public Action gameWon;
    int coinsRemaining = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    public void CoinCollected()
    {
        coinsRemaining--;
        if (coinsRemaining == 0 )
        {
            gameWon();
        }
    }

}
