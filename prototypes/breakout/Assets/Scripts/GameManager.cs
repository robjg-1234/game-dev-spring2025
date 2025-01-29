using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Action speedUpBall;
    public Action weirdPaddle;
    static public GameManager instance;
    int score = 0;
    int remainingBalls = 5;
    int[] randomSquares = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    int layer = 0;
    int hitCount = 0;
    public int level = 1;
    int currentPower = 4;
    int pointmultiplier = 1;
    int ballsOnPlay = 0;
    BrickScript powerBrick;
    BrickScript debuffBrick;
    paddleScript paddleController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        buildBoard();
        paddleController = paddle.GetComponent<paddleScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ballsOnPlay == 0 && remainingBalls > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(ball, Vector3.zero, Quaternion.identity);
                ballsOnPlay++;
                remainingBalls--;
                updateBalls();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z) && ballsOnPlay > 0)
            {
                activatePower();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                activateDebuff();
            }
            if (Input.GetKeyDown(KeyCode.R) && remainingBalls==0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    public void stopBallPlay()
    {
        ballsOnPlay--;
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
            powerBrick.setColor(8);
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
    private void selectDebuffBlock()
    {
        randomizer();
        if (debuffBrick != null)
        {
            debuffBrick.setColor(8);
            debuffBrick = null;
        }
        bool found = false;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < randomSquares.Length; j++)
            {
                if (!found)
                {
                    if (bricks[i, randomSquares[j]] != null && bricks[i, randomSquares[j]] != powerBrick)
                    {
                        debuffBrick = bricks[i, randomSquares[j]];
                        debuffBrick.setColor(7);
                        found = true;
                    }
                }
            }
        }
    }
    public void hitCounter()
    {
        hitCount++;
        checkBoard();
        if (hitCount / 5 >= 2)
        {
            selectPowerBlock();
            selectDebuffBlock();
            hitCount = 0;
        }
        else if (hitCount % 5 == 0)
        {
            selectPowerBlock();
        }
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
            paddleController.speed = 25 + 2 * level - 1;
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
        else if (powerNumber == 4)
        {
            //The Pinballer
            powerUp.color = Color.magenta;
        }
        currentPower = powerNumber;
    }
    void activatePower()
    {
        if (currentPower < 5)
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
            else if (currentPower == 4)
            {
                //Triple Ball
                StartCoroutine(summonTwoBalls());
            }
            currentPower = 5;
            powerUp.color = Color.white;
        }
    }
    //Power Ups
    IEnumerator biggerPaddle()
    {
        paddle.transform.localScale = new Vector3(6f, 0.5f, 1f);
        paddleController.changeColor(Color.red);
        yield return new WaitForSeconds(10);
        paddle.transform.localScale = new Vector3(5f, 0.5f, 1f);
        paddleController.changeColor(Color.white);
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
    IEnumerator summonTwoBalls()
    {
        Instantiate(ball, Vector3.zero, Quaternion.identity);
        ballsOnPlay++;
        yield return new WaitForSeconds(1f);
        Instantiate(ball, Vector3.zero, Quaternion.identity);
        ballsOnPlay++;
    }
    //Debuffs
    public void activateDebuff()
    {
        int debuff = Random.Range(0, 4);

        if (debuff == 0)
        {
            //Smaller Paddle
            StartCoroutine(smallerPaddle());
        }
        else if (debuff == 1)
        {
            //Faster Ball
            speedUpBall();
        }
        else if (debuff == 2)
        {
            //Slower Paddle
            StartCoroutine(slowThePaddle());
        }
        else if (debuff == 3)
        {
            //Inverse Controls
            StartCoroutine(invertController());
        }
        else if (debuff == 4)
        {
            //Unpredictable Bounces
            weirdPaddle();
            StartCoroutine(weirdMovement());
        }
    }
    IEnumerator smallerPaddle()
    {
        paddle.transform.localScale = new Vector3(4f, 0.5f, 1f);
        paddleController.changeColor(Color.blue);
        yield return new WaitForSeconds(10);
        paddle.transform.localScale = new Vector3(5f, 0.5f, 1f);
        paddleController.changeColor(Color.white);
    }
    IEnumerator invertController()
    {
        paddleController.changeController(-1);
        paddleController.changeColor(Color.green);
        yield return new WaitForSeconds(10);
        paddleController.changeController(1);
        paddleController.changeColor(Color.white);
    }
    IEnumerator slowThePaddle()
    {
        paddleController.speed -= 10;
        paddleController.changeColor(Color.cyan);
        yield return new WaitForSeconds(7.5f);
        paddleController.speed += 10;
        paddleController.changeColor(Color.white);
    }
    IEnumerator weirdMovement()
    {
        paddleController.changeColor(Color.yellow);
        yield return new WaitForSeconds(10);
        paddleController.changeColor(Color.white);
    }
}