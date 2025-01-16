using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject brick;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text ballCount;
    bool ballOnplay = false;
    BrickScript[,] bricks = new BrickScript[6,9];
    [SerializeField] GameObject ball;
    static public GameManager instance;
    int score = 0;
    int remainingBalls = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        Vector3 objectStartPos = transform.position;
        for (int i = 0; i < 6; i++)
        {

            for (int j = 0; j < 9; j++)
            {
                Vector3 pos = new Vector3(objectStartPos.x+j * 6f, objectStartPos.y + i * 1.5f, 0);
                GameObject tempBrick = Instantiate(brick, pos, Quaternion.identity);
                bricks[i,j] = tempBrick.GetComponent<BrickScript>();
                bricks[i,j].setColor(i);
            }

        }
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
    }
    public void stopBallPlay()
    {
        ballOnplay = false;
    }
    public void updateScore(int scored)
    {
        score += scored;
        scoreText.text = "Score: "+score.ToString();
    }
    void updateBalls()
    {
        ballCount.text = "Remaining Balls: " + remainingBalls;
    }
}
