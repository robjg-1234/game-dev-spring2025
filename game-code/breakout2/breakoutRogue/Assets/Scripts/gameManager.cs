using System.Collections;
using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public brickScript[,] board = new brickScript[9, 5];
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text round;
    [SerializeField] TMP_Text wave;
    [SerializeField] roundManager rm;
    [SerializeField] GameObject ball;
    [SerializeField] TMP_Text rerolls;
    [SerializeField] GameObject paddle;
    int availableRerolls = 12;
    int currentScore = 0;
    int scoreToBeat = 1000;
    choiceScript currentSelection = null;
    int roundNumber = 0;
    int waveNumber = 1;
    int options = 3;
    bool canGameStart = false;
    public bool gameRunning = false;
    int paddleHits = 15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
        updateScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (canGameStart)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(ball, new Vector3(0, 4, 0), Quaternion.identity);
                canGameStart = false;
                gameRunning = true;
            }
        }
    }
    public void updateScore(float value)
    {
        Debug.Log(value);
        currentScore += Mathf.RoundToInt(value);
        score.text = currentScore +"/"+scoreToBeat;
    }
    public int selectRandomBrick()
    {
        int randomChoice = Random.Range(0,20);
        if (randomChoice < 10)
        {
            //Common Brick
            randomChoice = Random.Range(0, 5);
        }
        else if (randomChoice <16)
        {
            //Uncommon Brick
            randomChoice = Random.Range(5, 9);
        }
        else if (randomChoice < 19)
        {
            //Rare Brick
            randomChoice = Random.Range(9, 11);
        }
        else
        {
            //Legendary Brick
            randomChoice = 11;
        }
        Debug.Log(randomChoice);
        return randomChoice;
    }
    public int getNewType(int posX, int posY)
    {
        int chosenType = -1;
        if (currentSelection != null)
        {
            chosenType = currentSelection.type;
            options--;
            if (options == 0)
            {
                canGameStart = true;
            }
            currentSelection.unSelect(1);
            currentSelection = null;
        }
        return chosenType;
    }
    public void changeSelection(choiceScript newSelection)
    {
        if (currentSelection != null)
        {
            currentSelection.unSelect(0);
        }
        currentSelection = newSelection;
    }
    void startRound()
    {
        options = 3;
        if (roundNumber==3)
        {
            if (waveNumber==1)
            {
                waveNumber++;
                wave.text = "Wave " + waveNumber;
                scoreToBeat = 25000;
                currentScore = 0;
                updateScore(0);
            }
            else if (waveNumber == 2)
            {
                waveNumber++;
                wave.text = "Wave " + waveNumber;
                scoreToBeat = 50000;
                currentScore = 0;
                updateScore(0);
            }
            gainRerolls(12);
            paddle.SetActive(true);
            paddleHits = 15;
            roundNumber = 1;
            round.text = "Round " + roundNumber;
            paddle.transform.position = new Vector3(0, 0.2246f, 0);
        }
        else
        {
            paddle.SetActive(true);
            paddleHits = 15;
            roundNumber++;
            round.text = "Round " + roundNumber;
            paddle.transform.position = new Vector3(0, 0.2246f, 0);
        }
        rm.startRound();

    }
    public bool tryToUseReroll()
    {
        if (availableRerolls > 0)
        {
            availableRerolls--;
            rerolls.text = "Rerolls: " + availableRerolls;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void gainRerolls(int numRerolls)
    {
        if (numRerolls == 3)
        {
            options--;
            if (options == 0)
            {
                canGameStart = true;
            }
        }
        availableRerolls += numRerolls;
        rerolls.text = "Rerolls: " + availableRerolls;
    }
    public void activateBrick(brickScript reMake)
    {
        reMake.gameObject.SetActive(true);
    }
    public void announceRoundStart()
    {
        gameRunning = false;
        StartCoroutine(roundCoroutine());
    }
    public IEnumerator roundCoroutine()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                board[i, j].gameObject.SetActive(true);
                board[i,j].checkRoundEndAbility();
            }
        }
        startRound();
        while(!canGameStart)
        {
            yield return null;
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j<5; j++)
            {
                board[i,j].newRoundRefresh();
            }
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                board[i, j].checkForMultipliers();
            }
        }
    }
    public void checkPaddle()
    {
        paddleHits--;
        if (paddleHits == 0)
        {
            paddle.SetActive(false);
        }
    }
}
