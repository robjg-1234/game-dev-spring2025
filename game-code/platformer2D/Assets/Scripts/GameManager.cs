using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject confirmationMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Image FadeIn; 
    [SerializeField] TMP_Text finalTime;
    [SerializeField] TMP_Text HexaCollected;
    [SerializeField] GameObject gameTimer;
    [SerializeField] GameObject finalSummary;
    [SerializeField] TMP_Text timer;
    [SerializeField] GameObject player;
    [SerializeField] TMP_Text deaths;
    [SerializeField] GameObject tutorial;
    public static GameManager instance;
    CheckpointScript lastCheckpoint;
    [SerializeField] Vector3 respawnPosition = new Vector3(-10f, 0.7f, 0);
    [SerializeField] int levelNum;
    [SerializeField] CameraScript oldCamera;
    public PlayerScript currentPlayerIteration;
    public Action playerDeath;
    public int CoinCollected =0;
    public int maxHexaCoins;
    public bool isPaused = false;
    int confirmationState = 0;
    bool started = false;
    bool gameWon = false;
    int second =0;
    int minute = 0;
    int hour = 0;
    float counter = 0f;
    int deathCounter = 0;
    bool cantUnpause = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        StartCoroutine(FadeInGame());
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
            if (Input.GetKeyDown(KeyCode.P) && !cantUnpause)
            {
                if (isPaused)
                {
                    isPaused = false;
                    PauseMenu.SetActive(false);
                }
                else
                {
                    isPaused = true;
                    PauseMenu.SetActive(true);
                }
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
                if (playerDeath != null)
                {
                    playerDeath();
                }
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
            tutorial.SetActive(false);
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
        HexaCollected.text = CoinCollected + "/"+maxHexaCoins;
        finalSummary.SetActive(true);
        deaths.text = "Deaths: " + deathCounter.ToString();
        gameWon = true;
    }
    public void ReturnToMainMenu()
    {
        PauseMenu.SetActive(false);
        cantUnpause = true;
        StartCoroutine(CheckConfirmationForQuitToMenu());
    }
    public void restartLevel()
    {
        PauseMenu.SetActive(false);
        cantUnpause = true;
        StartCoroutine(CheckConfirmationForRestart());
    }
    public void unPause()
    {
        isPaused = false;
        PauseMenu.SetActive(false);
    }
    IEnumerator FadeInGame()
    {
        while (FadeIn.color.a > 0)
        {
            FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, FadeIn.color.a - (1f * Time.deltaTime));
            yield return null;
        }
        FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, 0);
        FadeIn.gameObject.SetActive(false);
    }
    IEnumerator FadeOutGame()
    {
        FadeIn.gameObject.SetActive(true);
        while (FadeIn.color.a < 1)
        {
            FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, FadeIn.color.a + (1f * Time.deltaTime));
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
    IEnumerator CheckConfirmationForRestart()
    {
        confirmationMenu.SetActive(true);
        while (confirmationState == 0)
        {
            yield return null;
        }
        if (confirmationState == 1)
        {
            SceneManager.LoadScene(levelNum+1);
        }
        else
        {
            PauseMenu.SetActive(true);
            confirmationMenu.SetActive(false);
            cantUnpause = false;
            confirmationState = 0;
        }
    }

    IEnumerator CheckConfirmationForQuitToMenu()
    {
        confirmationMenu.SetActive(true);
        while (confirmationState == 0)
        {
            yield return null;
        }
        if (confirmationState == 1)
        {
            float finalTime = hour;
            finalTime += second / 3600;
            finalTime += minute / 60;
            if (gameWon)
            {
                if (levelNum == 1)
                {
                    if (PlayerPrefs.GetFloat("levelOneTime") > finalTime)
                    {
                        PlayerPrefs.SetString("levelOne", timer.text);
                        PlayerPrefs.SetFloat("levelOneTime", finalTime);
                    }
                }
                else if (levelNum == 2)
                {
                    if (PlayerPrefs.GetFloat("levelTwoTime") > finalTime)
                    {
                        PlayerPrefs.SetString("levelTwo", timer.text);
                        PlayerPrefs.SetFloat("levelTwoeTime", finalTime);
                    }
                }
                else if (levelNum == 3)
                {
                    if (PlayerPrefs.GetFloat("levelThreeTime") > finalTime)
                    {
                        PlayerPrefs.SetString("levelThree", timer.text);
                        PlayerPrefs.SetFloat("levelThreeTime", finalTime);
                    }
                }
                PlayerPrefs.Save();
            }
            StartCoroutine(FadeOutGame());
        }
        else
        {
            PauseMenu.SetActive(true);
            confirmationMenu.SetActive(false);
            cantUnpause = false;
            confirmationState = 0;
        }
    }
    public void RetryCheckpoint()
    {
        Destroy(currentPlayerIteration.gameObject);
        unPause();
    }
    public void Confirm()
    {
        confirmationState = 1;
    }
    public void Deny()
    {
        confirmationState = 2;
    }
    public void FinishLevel()
    {
        float finalTime = hour;
        finalTime += second / 3600;
        finalTime += minute / 60;
        if (gameWon)
        {
            if (levelNum == 1)
            {
                if (PlayerPrefs.GetFloat("levelOneTime") > finalTime)
                {
                    PlayerPrefs.SetString("levelOne", timer.text);
                    PlayerPrefs.SetFloat("levelOneTime", finalTime);
                }
            }
            else if (levelNum == 2)
            {
                if (PlayerPrefs.GetFloat("levelTwoTime") > finalTime)
                {
                    PlayerPrefs.SetString("levelTwo", timer.text);
                    PlayerPrefs.SetFloat("levelTwoeTime", finalTime);
                }
            }
            else if (levelNum == 3)
            {
                if (PlayerPrefs.GetFloat("levelThreeTime") > finalTime)
                {
                    PlayerPrefs.SetString("levelThree", timer.text);
                    PlayerPrefs.SetFloat("levelThreeTime", finalTime);
                }
            }
            PlayerPrefs.Save();
        }
        StartCoroutine(FadeOutGame());
    }
}
