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
        option1.SetActive(true);
        option1.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
        option2.SetActive(true);
        option2.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
        option3.SetActive(true);
        option3.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick(-1));
    }
    public void closeSummary()
    {
        if (waveLoss)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (waveWin)
        {
            roundSummary.SetActive(false);
            rms.ShowRelics();
            waveWin = false;
        }
        else
        {
            roundSummary.SetActive(false);
            startRound();
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
        roundSummary.SetActive(true);
    }


}
