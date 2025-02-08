using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class roundManager : MonoBehaviour
{
    [SerializeField] GameObject option1;
    [SerializeField] GameObject option2;
    [SerializeField] GameObject option3;
    [SerializeField] GameObject roundSummary;
    [SerializeField] TMP_Text summaryTitle;
    [SerializeField] TMP_Text scoreDesc;
    [SerializeField] TMP_Text buttonDesc;
    [SerializeField] relicManagerScript rms;
    gameManager gm;
    bool waveLoss = false;
    bool waveWin = false;
    bool gameStart = true;
    private void Start()
    {
        gm = gameManager.instance;
        gm.announceRoundStart();
    }
    public void startRound()
    {
        option1.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
        option2.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
        option3.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
        StartCoroutine(ShowOptions());
    }
    public void closeSummary()
    {
        if (waveLoss)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(MinimizeWindow());
        }
    }
    public void openSummary(int score, bool isWave, bool lost, int objective, bool win)
    {
        if (!gameStart)
        {
            if (isWave)
            {
                if (lost)
                {
                    summaryTitle.text = "Wave Failed";
                    scoreDesc.text = "Final Score: " + score + "/" + objective;
                    waveLoss = true;
                    buttonDesc.text = "Restart";
                }
                else if (win)
                {
                    summaryTitle.text = "You Win!";
                    scoreDesc.text = "Final Score: " + score + "/" + objective;
                    buttonDesc.text = "Start New Game";
                    waveLoss = true;
                }
                else
                {
                    summaryTitle.text = "Wave Completed!";
                    scoreDesc.text = "Wave Score: " + score + "/" + objective;
                    buttonDesc.text = "Next Wave";
                    waveWin = true;
                }
            }
            else
            {
                summaryTitle.text = "Round Completed";
                scoreDesc.text = "Current Score: " + score + "/" + objective;
                buttonDesc.text = "Next Round";
            }
        }
        else
        {
            summaryTitle.text = "New Game";
            scoreDesc.text = "Current Score: " + score + "/" + objective;
            buttonDesc.text = "Start";
            gameStart = false;
        }
        StartCoroutine(MaximizeWindow());
    }
    IEnumerator MinimizeWindow()
    {
        while (roundSummary.transform.localScale.x > 0)
        {
            roundSummary.transform.localScale = new Vector3(roundSummary.transform.localScale.x-0.01f, roundSummary.transform.localScale.y - 0.01f, roundSummary.transform.localScale.z - 0.01f);
            yield return null;
        }
        roundSummary.SetActive(false);
        roundSummary.transform.localScale = Vector3.zero;
        if (waveWin)
        {
            rms.ShowRelics();
            waveWin = false;
        }
        else
        {
            startRound();
        }
        
    }

    IEnumerator MaximizeWindow()
    {
        roundSummary.SetActive(true);
        while (roundSummary.transform.localScale.x < 1)
        {
            roundSummary.transform.localScale = new Vector3(roundSummary.transform.localScale.x + 0.01f, roundSummary.transform.localScale.y + 0.01f, roundSummary.transform.localScale.z + 0.01f);
            yield return null;
        }
        roundSummary.transform.localScale = new Vector3(1,1,1);
    }

    IEnumerator ShowOptions()
    {
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
        while (option1.transform.localScale.x < 1)
        {
            option1.transform.localScale = new Vector3(option1.transform.localScale.x + 0.01f, option1.transform.localScale.y + 0.01f, option1.transform.localScale.z + 0.01f);
            option2.transform.localScale = new Vector3(option2.transform.localScale.x + 0.01f, option2.transform.localScale.y + 0.01f, option2.transform.localScale.z + 0.01f);
            option3.transform.localScale = new Vector3(option3.transform.localScale.x + 0.01f, option3.transform.localScale.y + 0.01f, option3.transform.localScale.z + 0.01f);
            yield return null;
        }
        option1.transform.localScale = new Vector3(1, 1, 1);
        option2.transform.localScale = new Vector3(1, 1, 1);
        option3.transform.localScale = new Vector3(1, 1, 1);
    }

}
