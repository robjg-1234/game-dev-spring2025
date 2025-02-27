using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class brickScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] int posY;
    [SerializeField] int posX;
    [SerializeField] GameObject tempScoreShower;
    [SerializeField] Camera cam;
    [SerializeField] GameObject canvasObject;
    GameObject child;
    gameManager gm;
    float scoreValue = 25f;
    public int brickType = -1;
    public bool durable = false;
    public bool unbreakable = false;
    public int ballsConsumed = 0;
    int hits = 0;
    float multiplier = 1;
    public float defaultMult = 1;
    int daysActive = 0;
    bool hitOnce = false;
    bool flashing = false;
    float tempVal = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = transform.parent.GetComponent<gameManager>();
        rend.material.color = Color.white;
        gm.board[posX, posY] = this;
    }
    public void updateBrickType(int newType)
    {


        brickType = newType;
        switch (brickType)
        {
            case 0:
                //chained brick
                rend.color = Color.cyan;
                scoreValue = 50f * checkBoard(newType);
                break;
            case 1:
                //Tower
                rend.color = Color.gray;
                scoreValue = 50 * checkSurrounding(-1);
                break;
            case 2:
                //Palm Tree
                rend.color = Color.green;
                scoreValue = 0;
                break;
            case 3:
                //Cardboard Brick
                rend.color = new Color(0.4716981f, 0.1995904f, 0f);
                scoreValue = Mathf.Clamp(500 - 50 * hits, 0, 500);
                break;
            case 4:
                //Bonus Brick
                rend.color = Color.yellow;
                scoreValue = 100;
                break;
            case 5:
                //Piggy Bank
                rend.color = new Color(0.7169812f, 0.1116056f, 0.3944906f);
                unbreakable = true;
                scoreValue = 10 * hits;
                break;
            case 6:
                //Phantasmal Brick
                rend.color = new Color(0.6745283f, 1f, 0.9345052f);
                scoreValue = 100;
                gameObject.GetComponent<Collider2D>().isTrigger = true;
                break;
            case 7:
                //Reinforcements
                rend.color = new Color(1f, 0.611645f, 0);
                scoreValue = 50;
                if (!hitOnce)
                {
                    checkSurrounding(brickType);
                }
                break;
            case 8:
                //Trust Fund
                rend.color = Color.blue;
                defaultMult = 0.5f * daysActive;
                scoreValue = 0f;
                break;
            case 9:
                //Black Hole
                rend.color = Color.black;
                defaultMult = 3f * ballsConsumed;
                scoreValue = 0f;
                gameObject.GetComponent<Collider2D>().isTrigger = true;
                unbreakable = true;
                break;
            case 10:
                //Explosive Brick
                rend.color = Color.red;
                scoreValue = 50f;
                break;
            case 11:
                //Refresher
                rend.color = new Color(0.5575659f, 1, 0.504717f);
                scoreValue = 25f;
                break;
            case 12:
                //The Eye
                rend.color = new Color(0.6303558f, 0, 1);
                scoreValue = 0;
                break;
            default:
                //Basic Brick
                rend.color = Color.white;
                scoreValue = 25f;
                break;
        }
        if (brickType == -1)
        {
            daysActive = 0;
        }
    }
    int checkBoard(int type)
    {
        int similar = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (gm.board[i, j].brickType == brickType)
                {
                    similar++;
                }
            }
        }
        return similar;
    }
    private void OnMouseDown()
    {
        if (brickType < 0)
        {
            updateBrickType(gm.getNewType(posX, posY));
        }
        else
        {
            if (!flashing)
            {
                StartCoroutine(FlashRed());
            }
        }
    }
    IEnumerator FlashRed()
    {
        Color temp = rend.color;
        rend.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        rend.color = temp;
        yield return new WaitForSeconds(0.5f);
        rend.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        rend.color = temp;
        flashing = false;
    }

    int checkSurrounding(int typeCheck)
    {
        int numberOfBlocks = 0;
        int maxX = Mathf.Clamp(posX + 1, 0, 8) + 1;
        int minX = Mathf.Clamp(posX - 1, 0, 8);
        int maxY = Mathf.Clamp(posY + 1, 0, 4) + 1;
        int minY = Mathf.Clamp(posY - 1, 0, 4);
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if (gm.board[i, j].brickType == typeCheck && gm.board[i, j] != this)
                {
                    numberOfBlocks++;
                }
                if (typeCheck == 7)
                {
                    gm.board[i, j].durable = true;
                }
            }
        }
        return numberOfBlocks;
    }
    void activateSurrounding()
    {
        int maxX = Mathf.Clamp(posX + 1, 0, 8) + 1;
        int minX = Mathf.Clamp(posX - 1, 0, 8);
        int maxY = Mathf.Clamp(posY + 1, 0, 4) + 1;
        int minY = Mathf.Clamp(posY - 1, 0, 4);
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if (gm.board[i, j].brickType != 10)
                {
                    gm.board[i, j].tryToBreak();
                }
            }
        }
    }

    public void checkRoundEndAbility()
    {
        switch (brickType)
        {
            case 2:
                if (!hitOnce)
                {
                    createTempScore(150f * multiplier * gm.desperadoMult);
                    gm.updateScore(150f * multiplier * gm.desperadoMult);
                }
                break;
            case 12:
                if (!hitOnce)
                {
                    gm.gainRerolls(4);
                }
                break;
        }
        hitOnce = false;
    }
    void refreshSelf()
    {
        gameObject.SetActive(true);
    }
    public void newRoundRefresh()
    {
        daysActive += 1;
        refreshSelf();
        updateBrickType(brickType);
    }
    public void checkForMultipliers()
    {
        if (brickType != 8 && brickType != 9)
        {
            multiplier = defaultMult;
            int maxX = Mathf.Clamp(posX + 1, 0, 8) + 1;
            int minX = Mathf.Clamp(posX - 1, 0, 8);
            int maxY = Mathf.Clamp(posY + 1, 0, 4) + 1;
            int minY = Mathf.Clamp(posY - 1, 0, 4);
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (gm.board[i, j] != this)
                    {
                        if (gm.board[i, j].brickType == 8)
                        {
                            multiplier *= gm.board[i, j].defaultMult;
                        }
                        else if (gm.board[i, j].brickType == 9)
                        {
                            multiplier *= gm.board[i, j].defaultMult;
                        }
                    }
                }
            }
        }
    }
    public void tryToBreak()
    {
        if (gameObject.activeSelf)
        {
            if (brickType >= 0)
            {
                hits++;
                if (brickType == 10)
                {
                    if (durable)
                    {
                        durable = false;
                        updateBrickType(brickType);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    activateSurrounding();
                }
                else if (brickType == 11)
                {
                    gm.fixPaddle();
                    if (!hitOnce)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                gm.board[i, j].gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
            hitOnce = true;
            createTempScore(scoreValue * multiplier * gm.desperadoMult);
            gm.updateScore(scoreValue * multiplier * gm.desperadoMult);

            if (durable)
            {
                durable = false;
                updateBrickType(brickType);
            }
            else if (unbreakable)
            {
                updateBrickType(brickType);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    void createTempScore(float val)
    {
        tempVal = val;
        if (child == null)
        {
            child = Instantiate(tempScoreShower, cam.WorldToScreenPoint(transform.position), Quaternion.identity);
            child.transform.SetParent(canvasObject.transform);
            child.GetComponent<ScoreScript>().SetScoreAndStart(Mathf.RoundToInt(tempVal));
        }
        else
        {
            child.GetComponent<ScoreScript>().newTarget(Mathf.RoundToInt(tempVal));
        }
    }
}
