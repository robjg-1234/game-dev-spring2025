using System;
using UnityEngine;

public class managerScript : MonoBehaviour
{
    [SerializeField] GameObject finalScreen;
    [SerializeField] GameObject objective;
    public static managerScript instance;
    public Action gameWon;
    int coinsRemaining = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    public void CoinCollected()
    {
        coinsRemaining--;
        if (coinsRemaining == 0)
        {
            gameWon();
            objective.SetActive(false);
            finalScreen.SetActive(true);
        }
    }

}
