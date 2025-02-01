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
    public int paddleHits = 15;
    public int defaultPaddle = 15;
    public int desperadoMult = 1;
    public bool greedyHand = false;
    public bool utilityBelt = false;
    public bool bigDonut = false;
    public bool faultyEquipment = false;
    ArrayList activeRelics = new ArrayList();
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
        currentScore += Mathf.RoundToInt(value * desperadoMult);
        score.text = currentScore + "/" + scoreToBeat;
    }
    public int selectRandomBrick(int currentValue)
    {
        int randomChoice = Random.Range(0, 20);
        if (randomChoice < 10)
        {
            //Common Brick
            int[] commonChoice = new int[] { 0, 1, 2, 3, 4 };
            commonChoice = shuffleArray(commonChoice);
            for (int i = 0; i < commonChoice.Length; i++)
            {
                if (commonChoice[i] != currentValue)
                {
                    randomChoice = commonChoice[i];
                }
            }
        }
        else if (randomChoice < 16)
        {
            //Uncommon Brick
            int[] uncommonChoice = new int[] { 5, 6, 7, 8 };
            uncommonChoice = shuffleArray(uncommonChoice);
            for (int i = 0; i < uncommonChoice.Length; i++)
            {
                if (uncommonChoice[i] != currentValue)
                {
                    randomChoice = uncommonChoice[i];
                }
            }
        }
        else if (randomChoice < 19)
        {
            //Rare Brick
            int[] rareChoice = new int[] { 9, 10 };
            rareChoice = shuffleArray(rareChoice);
            for (int i = 0; i < rareChoice.Length; i++)
            {
                if (rareChoice[i] != currentValue)
                {
                    randomChoice = rareChoice[i];
                }
            }
        }
        else
        {
            //Legendary Brick
            int[] legendaryChoice = new int[] { 11, 12 };
            legendaryChoice = shuffleArray(legendaryChoice);
            for (int i = 0; i < legendaryChoice.Length; i++)
            {
                if (legendaryChoice[i] != currentValue)
                {
                    randomChoice = legendaryChoice[i];
                }
            }
        }
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
        if (roundNumber == 3)
        {
            if (currentScore >= scoreToBeat)
            {
                if (waveNumber < 3)
                {
                    rm.openSummary(currentScore, true, false, scoreToBeat, false);
                    if (waveNumber == 1)
                    {
                        waveNumber++;
                        wave.text = "Wave " + waveNumber;
                        scoreToBeat = 15000;
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
                    paddleHits = defaultPaddle;
                    roundNumber = 1;
                    round.text = "Round " + roundNumber;
                    paddle.transform.position = new Vector3(0, 0.2246f, 0);
                }
                else
                {
                    rm.openSummary(currentScore, true, false, scoreToBeat, true);
                }
            }
            else
            {
                rm.openSummary(currentScore, true, true, scoreToBeat, false);
            }
        }
        else
        {
            paddle.SetActive(true);
            paddleHits = defaultPaddle;
            roundNumber++;
            round.text = "Round " + roundNumber;
            paddle.transform.position = new Vector3(0, 0.2246f, 0);
            rm.openSummary(currentScore, false, false, scoreToBeat, false);
        }
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
        if (!greedyHand)
        {
            if (numRerolls == 32)
            {
                greedyHand = true;
            }
            if (utilityBelt)
            {
                availableRerolls += 2;
            }
            availableRerolls += numRerolls;
            rerolls.text = "Rerolls: " + availableRerolls;
        }
        if (numRerolls == 3)
        {
            options--;
            if (options == 0)
            {
                canGameStart = true;
            }
        }
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
                board[i, j].checkRoundEndAbility();
            }
        }
        startRound();
        while (!canGameStart)
        {
            yield return null;
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                board[i, j].newRoundRefresh();
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
        if (faultyEquipment)
        {
            int chance = Random.Range(0, 2);
            paddleHits -= 2 * chance;
        }
        else
        {
            paddleHits--;
        }

        if (paddleHits <= 0)
        {
            paddle.SetActive(false);
        }
    }
    public void fixPaddle()
    {
        paddleHits = defaultPaddle;
        paddle.SetActive(true);
    }
    public int[] shuffleArray(int[] target)
    {
        int[] tempArray = target;
        for (int i = 0; i < target.Length; i++)
        {
            int temp = Random.Range(0, target.Length);
            int tempVal = tempArray[i];
            tempArray[i] = tempArray[temp];
            tempArray[temp] = tempVal;
        }
        return tempArray;
    }
    public void FullMetalJacketChange()
    {
        paddle.GetComponent<paddleScript>().speed *= 2;
    }
}
