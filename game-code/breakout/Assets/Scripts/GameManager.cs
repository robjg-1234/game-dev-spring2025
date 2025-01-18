using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject brick;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text ballCount;
    [SerializeField] GameObject paddle;
    [SerializeField] GameObject ball;
    [SerializeField] Image powerUp;
    [SerializeField] GameObject lifeline;
    BrickScript[,] bricks = new BrickScript[6, 9];
    public Action callBall;
    static public GameManager instance;
    int score = 0;
    int remainingBalls = 5;
    int[] randomSquares = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    int layer = 0;
    int hitCount = 0;
    public int level = 1;
    int currentPower = 4;
    int pointmultiplier = 1;
    BrickScript powerBrick;
    bool ballOnplay = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        buildBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ballOnplay && remainingBalls > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(ball, Vector3.zero, Quaternion.identity);
                ballOnplay = true;
                remainingBalls--;
                updateBalls();
            }
        }
        else
        {
            if (hitCount > 5)
            {
                selectPowerBlock();
                hitCount = 0;

            }
            if (Input.GetKeyDown(KeyCode.Z) && ballOnplay)
            {
                activatePower();
            }
        }
    }
    public void stopBallPlay()
    {
        ballOnplay = false;
    }
    public void updateScore(int scored)
    {
        score += scored * level * pointmultiplier;
        scoreText.text = "Score: " + score.ToString();
    }
    void updateBalls()
    {
        ballCount.text = "Remaining Balls: " + remainingBalls;
    }

    void randomizer()
    {
        for (int i = 0; i < randomSquares.Length; i++)
        {
            int target = Random.Range(0, 9);
            int tempVal = randomSquares[i];
            randomSquares[i] = randomSquares[target];
            randomSquares[target] = tempVal;
        }
    }
    void selectPowerBlock()
    {
        randomizer();
        if (powerBrick != null)
        {
            powerBrick.setColor(7);
            powerBrick = null;
        }
        bool found = false;
        for (int i = 0; i < randomSquares.Length; i++)
        {
            if (!found)
            {
                if (bricks[layer, randomSquares[i]] != null)
                {
                    powerBrick = bricks[layer, randomSquares[i]];
                    powerBrick.setColor(6);
                    found = true;
                }
            }
        }
    }
    public void hitCounter()
    {
        hitCount++;
        checkBoard();
    }
    void checkBoard()
    {
        int nonHitBlocks = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (bricks[i, j] != null)
                {
                    nonHitBlocks++;
                }
            }
        }
        if (nonHitBlocks == 0)
        {
            buildBoard();
            layer = 0;
            level++;
        }
    }
    void buildBoard()
    {
        Vector3 objectStartPos = transform.position;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Vector3 pos = new Vector3(objectStartPos.x + j * 5.5f, objectStartPos.y + i * 1.5f, 0);
                GameObject tempBrick = Instantiate(brick, pos, Quaternion.identity);
                bricks[i, j] = tempBrick.GetComponent<BrickScript>();
                bricks[i, j].setColor(i);
            }
        }
    }
    public void changeLayer(int newLayer)
    {
        if (newLayer > layer)
        {
            layer = newLayer;
        }
    }
    public void choosePower()
    {
        int powerNumber = Random.Range(0, 4);
        if (powerNumber == 0)
        {
            //Bigger Paddle
            powerUp.color = Color.red;
        }
        else if (powerNumber == 1)
        {
            //Lifeline
            powerUp.color = Color.green;
        }
        else if (powerNumber == 2)
        {
            //Freeze 3 seconds
            powerUp.color = Color.blue;
        }
        else if (powerNumber == 3)
        {
            //Double Points
            powerUp.color = Color.yellow;
        }
        currentPower = powerNumber;

    }
    void activatePower()
    {
        if (currentPower < 4)
        {
            if (currentPower == 0)
            {
                //Bigger Paddle
                StartCoroutine(biggerPaddle());
            }
            else if (currentPower == 1)
            {
                //Lifeline
                StartCoroutine(activateLifeline());
            }
            else if (currentPower == 2)
            {
                //Freeze 3 seconds
                callBall();
            }
            else if (currentPower == 3)
            {
                //Double Points
                StartCoroutine(doublePoints());
            }
            currentPower = 4;
            powerUp.color = Color.white;
        }
    }
    IEnumerator biggerPaddle()
    {
        paddle.transform.localScale = new Vector3(5f, 0.5f, 1f);
        yield return new WaitForSeconds(10);
        paddle.transform.localScale = new Vector3(4f, 0.5f, 1f);
    }
    IEnumerator activateLifeline()
    {
        lifeline.SetActive(true);
        yield return new WaitForSeconds(3);
        lifeline.SetActive(false);
    }
    IEnumerator doublePoints()
    {
        pointmultiplier *= 2;
        yield return new WaitForSeconds(15);
        pointmultiplier = 1;
    }
}
