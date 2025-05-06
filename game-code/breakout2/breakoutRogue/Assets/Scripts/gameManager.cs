using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    [SerializeField] TMP_Text introTimer;
    [SerializeField] GameObject topThree;
    [SerializeField] TMP_Text brickOne;
    [SerializeField] TMP_Text brickTwo;
    [SerializeField] TMP_Text brickThree;
    [SerializeField] Image brickOneimage;
    [SerializeField] Image brickTwoimage;
    [SerializeField] Image brickThreeimage;
    [SerializeField] TMP_Text costText;
    [SerializeField] GameObject confirmation;
    [SerializeField] TMP_Text confirmationText;
    [SerializeField] GameObject wavePage;
    [SerializeField] GameObject wheelPage;
    [SerializeField] GameObject bonusDraft;
    public static gameManager instance;
    public brickScript[,] board = new brickScript[9, 5];
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text round;
    [SerializeField] TMP_Text wave;
    [SerializeField] roundManager rm;
    [SerializeField] GameObject ball;
    [SerializeField] TMP_Text rerolls;
    [SerializeField] GameObject paddle;
    [SerializeField] TMP_Text paddleText;
    public bool closeable = false;
    int cost = 1;
    int availableRerolls = 12;
    int currentScore = 0;
    int scoreToBeat = 1000;
    choiceScript currentSelection = null;
    int roundNumber = 0;
    int waveNumber = 1;
    int options = 3;
    bool canGameStart = false;
    bool firstStart = true;
    bool hasExtra = false;
    bool hasExtraConfirmation = false;
    public bool OnWheel = false;
    public bool gameRunning = false;
    public int paddleHits = 15;
    public int defaultPaddle = 15;
    public int desperadoMult = 1;
    public bool greedyHand = false;
    public bool utilityBelt = false;
    public bool bigDonut = false;
    public bool faultyEquipment = false;
    public int timesSpun = 0;
    string[] wheelEffects = new string[] { "extra", "lThreeRoll", "lOneRoll", "threeRoll", "lOneRoll", "threeRoll", "oneRoll", "wheelBricks", "oneRoll", "half", "oneRoll", "lThreeRoll", "lOneRoll", "oneRoll", "lOneRoll", "half", "oneRoll", "threeRoll", "lOneRoll", "wheelBricks" };
    Dictionary<int, float> bricksVal = new Dictionary<int, float>();
    ArrayList activeRelics = new ArrayList();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
        updateScore(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (canGameStart && !wheelPage.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
                
                //Make a timer to release the ball
                StartCoroutine(ReleaseTheBall());
                canGameStart = false;
                gameRunning = true;
            }
        }               
        if (!bonusDraft.activeSelf && !hasExtraConfirmation)
        {
            hasExtra = false;
        }
        else
        {
            hasExtra = true;
        }
    }
    public void updateScore(float value, int type)
    {
        if (bricksVal.ContainsKey(type))
        {
            bricksVal[type] += value;
        }
        else
        {
            bricksVal[type] = value;
        }
        
        currentScore += Mathf.RoundToInt(value);
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
            int[] rareChoice = new int[] { 9, 10, 13 };
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
        costText.text = "Cost: 1 Reroll";
        cost = 1;
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
                        updateScore(0, -1);
                    }
                    else if (waveNumber == 2)
                    {
                        waveNumber++;
                        wave.text = "Wave " + waveNumber;
                        scoreToBeat = 50000;
                        currentScore = 0;
                        updateScore(0, -1);
                    }
                    gainRerolls(12);
                    paddle.SetActive(true);
                    paddleHits = defaultPaddle;
                    updatePaddleText();
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
            updatePaddleText();
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
        wavePage.SetActive(true);
        StartCoroutine(roundCoroutine());
    }
    public IEnumerator roundCoroutine()
    {
        rm.SilenceTheWheel();
        //Waiting for the numbers to dacay
        bool counting = true;
        int existingCells = board.GetLength(0)* board.GetLength(1);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                board[i, j].gameObject.SetActive(true);
                board[i, j].checkRoundEndAbility();
            }
        }
        while (counting)
        {
            int currentActive = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (board[i, j].childActive)
                    {
                        currentActive++;
                    }
                }
            }
            if (currentActive == 0)
            {
                counting = false;
            }
            yield return null;
        }
        if (!firstStart)
        {
            ShowTopThreeBricks();
        }
        else
        {
            firstStart = false;
        }
        startRound();
        while (!canGameStart)
        {
            yield return null;
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
            paddleHits = 0;
        }
        updatePaddleText();
    }
    public void fixPaddle()
    {
        paddleHits = defaultPaddle;
        updatePaddleText();
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
    void updatePaddleText()
    {
        paddleText.text = "Paddle Bounces: " + paddleHits;
    }
    public void ActivateWheelEffect(int choice)
    {
        string effect = wheelEffects[choice];
        timesSpun++;
        switch (effect)
        {
            case "extra":
                //Activate the extra draft and leave the wheel page
                canGameStart = false;
                confirmationText.text = "Jackpot! You got an extra brick draft!";
                options++;
                hasExtra = true;
                hasExtraConfirmation = true;
                bonusDraft.GetComponent<choiceScript>().changeOption(selectRandomBrick(-1));
                StartCoroutine(ShowBonus());
                break;
            case "oneRoll":
                confirmationText.text = "+2 Rerolls";
                gainRerolls(2);
                break;
            case "lOneRoll":
                confirmationText.text = "-1 Reroll";
                tryToUseReroll();
                break;
            case "threeRoll":
                confirmationText.text = "+4 Rerolls";
                gainRerolls(4);
                break;
            case "lThreeRoll":
                confirmationText.text = "-3 Rerolls";
                for (int i = 0; i < 3; i++)
                {
                    tryToUseReroll();
                }
                break;
            case "half":
                confirmationText.text = "Oh no! You lost half your rerolls!";
                int halfRerolls = Mathf.FloorToInt(availableRerolls / 2);
                for (int i =0; i < halfRerolls; i++)
                {
                    tryToUseReroll();
                }
                break;
            case "wheelBricks":
                //Double it and give it to the next person
                confirmationText.text = "Fortune favors the bold! Your Bricks of Fortune score has been doubled.";
                timesSpun *= 2;
                break;
        }
        confirmation.SetActive(true);
    }
    public void CloseReward()
    {
        if (hasExtraConfirmation)
        {
            confirmation.SetActive(false);
            ReturnToMain();
            hasExtraConfirmation = false;
        }
        else
        {
            confirmation.SetActive(false);
        }
    }
    public void OpenWheelPage()
    {
        if (!hasExtra && closeable)
        {
            currentSelection?.unSelect(0);
            currentSelection = null;
            OnWheel = true;
            wavePage.SetActive(false);
            wheelPage.SetActive(true);
        }
    }
    public void ReturnToMain()
    {
        if (!wheelPage.GetComponent<WheelScript>().spinning && !confirmation.activeSelf)
        {
            OnWheel = false;
            wavePage.SetActive(true);
            wheelPage.SetActive(false);
        }
    }
    public bool UseWheel()
    {
        if (cost <= availableRerolls && !confirmation.activeSelf)
        {
            for (int i = 0; i < cost; i++)
            {
                tryToUseReroll();
            }
            cost++;
            costText.text = "Cost: " + cost + " Rerolls";
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator ShowBonus()
    {
        bonusDraft.SetActive(true);
        while (bonusDraft.transform.localScale.x < 1)
        {
            bonusDraft.transform.localScale = new Vector3(bonusDraft.transform.localScale.x + 5f * Time.deltaTime, bonusDraft.transform.localScale.y + 5f * Time.deltaTime, bonusDraft.transform.localScale.z + 5f * Time.deltaTime);
            yield return null;
        }
        bonusDraft.transform.localScale = new Vector3(1, 1, 1);
    }

    ArrayList FindTopThree()
    {
        int passes = 0;
        ArrayList highestBricks = new ArrayList();
        while (passes < 3)
        {
            float highest = 0;
            int newKey = -1;
            foreach (int key in bricksVal.Keys)
            {
                if (bricksVal[key] > highest && !highestBricks.Contains(key))
                {
                    highest = bricksVal[key];
                    newKey = key;
                }
            }
            highestBricks.Add(newKey);
            passes++;
        }
        return highestBricks;
    }

    void ShowTopThreeBricks()
    {
        ArrayList bricks = FindTopThree();
        bool basicBrickUsed = false;
        Color color;
        for (int i = 0; i < bricks.Count; i++) 
        {
            int val = (int)bricks[i];
            switch (val)
            {
                case 0:
                    color = Color.cyan;
                    break;
                case 1:
                    color = Color.gray;
                    break;
                case 2:
                    color = Color.green;
                    break;
                case 3:
                    color = new Color(0.4716981f, 0.1995904f, 0f);
                    break;
                case 4:
                    color = Color.yellow;
                    break;
                case 5:
                    color = new Color(0.7169812f, 0.1116056f, 0.3944906f);
                    break;
                case 6:
                    color = new Color(0.6745283f, 1f, 0.9345052f);
                    break;
                case 7:
                    color = new Color(1f, 0.611645f, 0);
                    break;
                case 8:
                    color = Color.blue;
                    break;
                case 9:
                    color = Color.black;
                    break;
                case 10:
                    color = Color.red;
                    break;
                case 11:
                    color = new Color(0.5575659f, 1, 0.504717f);
                    break;
                case 12:
                    color = new Color(0.6303558f, 0, 1);
                    break;
                case 13:
                    color = new Color(0.09878961f, 0.5660378f, 0.4582838f);
                    break;
                default:
                    color = Color.white;
                    break;
            }
            if (i == 0)
            {
                brickOne.text = "Scored: "+ bricksVal[val];
                brickOneimage.color = color;
            }
            else if (i == 1)
            {
                if (basicBrickUsed && val == -1) {
                    brickTwo.text = "";
                    brickTwoimage.color = new Color(1,1,1,0);
                }
                else
                {
                    brickTwo.text = "Scored: " + bricksVal[val];
                    brickTwoimage.color = color;
                }
            }
            else if (i == 2)
            {
                if (basicBrickUsed && val == -1)
                {
                    brickThree.text = "";
                    brickThreeimage.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    brickThree.text = "Scored: " + bricksVal[val];
                    brickThreeimage.color = color;
                }
            }
            if (val == -1)
            {
                basicBrickUsed = true;
            }
        }
        topThree.SetActive(true);
    }

    public void CloseTop()
    {
        topThree.SetActive(false);
    }

    IEnumerator ReleaseTheBall()
    {
        while (!closeable)
        {
            yield return null;
        }
        wavePage.SetActive(false);
        introTimer.text = "3";
        introTimer.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        introTimer.text = "2";
        yield return new WaitForSeconds(1);
        introTimer.text = "1";
        yield return new WaitForSeconds(1);
        introTimer.text = "GO!";
        Instantiate(ball, new Vector3(0, 4, 0), Quaternion.identity);
        while (introTimer.color.a > 0)
        {
            introTimer.color = new Color(introTimer.color.r, introTimer.color.g, introTimer.color.b, introTimer.color.a-2*Time.deltaTime);
            yield return null;
        }
        introTimer.gameObject.SetActive(false);
        introTimer.color = new Color(introTimer.color.r, introTimer.color.g, introTimer.color.b, 1);
    }
}
