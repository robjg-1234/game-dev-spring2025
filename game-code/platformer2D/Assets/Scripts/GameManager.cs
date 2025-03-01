using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text finalTime;
    [SerializeField] TMP_Text HexaCollected;
    [SerializeField] GameObject gameTimer;
    [SerializeField] GameObject finalSummary;
    [SerializeField] TMP_Text timer;
    [SerializeField] GameObject player;
    [SerializeField] TMP_Text deaths;
    public static GameManager instance;
    CheckpointScript lastCheckpoint;
    Vector3 respawnPosition = new Vector3(-10f, 0.7f, 0);
    [SerializeField] CameraScript oldCamera;
    public PlayerScript currentPlayerIteration;
    public Action playerDeath;
    public int CoinCollected =0;
    bool started = false;
    bool gameWon = false;
    int second =0;
    int minute = 0;
    int hour = 0;
    float counter = 0f;
    int deathCounter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (!started && !gameWon)
        {
            if (Input.anyKey)
            {
                started = true;
            }
        }
        else if (!gameWon)
        {
            if (counter > 1)
            {
                counter = 0;
                second++;
                if (second > 59)
                {
                    second = 0;
                    minute++;
                    if (minute > 59)
                    {
                        minute = 0;
                        hour++;
                    }
                }
                updateTimer();
            }
            else
            {
                counter += Time.deltaTime;
            }
            
        }
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    public void RespawnPlayer()
    {
        if (started && !gameWon)
        {
            if (this != null)
            {
                playerDeath();
            }
            StartCoroutine(spawnPlayer());
        }
    }

    public void SetNewRespawnPosition(CheckpointScript newCheckpoint)
    {
        if (lastCheckpoint != null)
        {
            lastCheckpoint.Unselect();
        }
        respawnPosition = newCheckpoint.transform.position;
        lastCheckpoint = newCheckpoint;
    }

    IEnumerator spawnPlayer()
    {
        deathCounter++;
        yield return new WaitForSeconds(0.2f);
        Instantiate(player, respawnPosition, Quaternion.identity);
    }

    public void changeCamera(CameraScript newCamera, bool respawnFlag)
    {
        if (respawnFlag)
        {
            if (newCamera != oldCamera)
            {
                oldCamera.toggleCamera();
                newCamera.toggleCamera();
            }
            oldCamera = newCamera;
        }
        else
        {
            if (oldCamera != null)
            {
                oldCamera.toggleCamera();
            }
            newCamera.toggleCamera();
            oldCamera = newCamera;
        }
    }
    void updateTimer()
    {
        String newTime = "";
        newTime += hour.ToString() + ":";
        if (minute < 10)
        {
            newTime += "0"+minute.ToString()+":";
        }
        else
        {
            newTime += minute.ToString()+":";
        }
        if (second < 10)
        {
            newTime+="0"+second.ToString();
        }
        else
        {
            newTime += second.ToString();
        }
        timer.text = newTime;
    }
    public void winGame()
    {
        finalTime.text = timer.text;
        gameTimer.SetActive(false);
        HexaCollected.text = CoinCollected + "/7";
        finalSummary.SetActive(true);
        deaths.text = "Deaths: " + deathCounter.ToString();
        gameWon = true;
    }
}
