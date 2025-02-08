using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class choiceScript : MonoBehaviour
{
    [SerializeField] TMP_Text brickTitle;
    [SerializeField] TMP_Text brickDescription;
    [SerializeField] Image brickColor;
    [SerializeField] Image panel;
    gameManager gm;
    public int type = -1;
    private void Start()
    {
        gm = gameManager.instance;
    }

    public void changeOption(int option)
    {
        type = option;
        switch (option)
        {
            case 0:
                brickTitle.text = "Chained Brick";
                brickDescription.text = "Gains +50 base score for each Chained Block in the board.";
                brickColor.color = Color.cyan;
                break;
            case 1:
                brickTitle.text = "The Tower";
                brickDescription.text = "Gains +50 base score for each Basic Brick surrounding it.";
                brickColor.color = Color.gray;
                break;
            case 2:
                brickTitle.text = "Palm Tree";
                brickDescription.text = "If this brick is not hit during the round gain 150 score.";
                brickColor.color = Color.green;
                break;
            case 3:
                brickTitle.text = "Cardboard Brick";
                brickDescription.text = "Gives 500 score, loses 50 score per hit.";
                brickColor.color = new Color(0.4716981f, 0.1995904f, 0f);
                break;
            case 4:
                brickTitle.text = "Bonus Brick";
                brickDescription.text = "Gives 100 score when hit.";
                brickColor.color = Color.yellow;
                break;
            case 5:
                brickTitle.text = "Piggy Bank";
                brickDescription.text = "Unbreakable. Gains 10 score each time it gets hit.";
                brickColor.color = new Color(0.7169812f, 0.1116056f, 0.3944906f);
                break;
            case 6:
                brickTitle.text = "Phantasmal Brick";
                brickDescription.text = "Gives 100 score, the ball goes through this block.";
                brickColor.color = new Color(0.6745283f, 1f, 0.9345052f);
                break;
            case 7:
                brickTitle.text = "Reinforcements";
                brickDescription.text = "Gives 50 score. Durable. Makes surrounding blocks durable.";
                brickColor.color = new Color(1f, 0.611645f, 0);
                break;
            case 8:
                brickTitle.text = "Trust Fund";
                brickDescription.text = "Provides no score. Multiplies the score of surrounding bricks by 0.5, gains 0.5 multiplier every round.";
                brickColor.color = Color.blue;
                break;
            case 9:
                brickTitle.text = "Black Hole";
                brickDescription.text = "Provides no score and consumes the ball. Multiplies the score of surrounding bricks by 0, gains 3 multiplier for every ball consumed.";
                brickColor.color = Color.black;
                break;
            case 10:
                brickTitle.text = "Explosive Brick";
                brickDescription.text = "Gives 50 score when hit. Activates surrounding bricks. Cannot activate Explosive Bricks.";
                brickColor.color = Color.red;
                break;
            case 11:
                brickTitle.text = "Refresher";
                brickDescription.text = "Gives 25 score. Replenishes the board and paddle bounces, can only be used once per round.";
                brickColor.color = new Color(0.5575659f,1, 0.504717f);
                break;
            case 12:
                brickTitle.text = "The Eye";
                brickDescription.text = "Provides no score. If it is not hit throughout the round, gain 4 rerolls.";
                brickColor.color = new Color(0.6303558f, 0, 1);
                break;
        }
    }
    public void reroll()
    {
        if (gm.tryToUseReroll())
        {
            changeOption(gm.selectRandomBrick(type));
        }
    }
    public void skip()
    {
        panel.color = Color.white;
        gm.gainRerolls(3);
        StartCoroutine(CloseThisWindow());
    }
    public void selected() 
    {
        gm.changeSelection(this);
        panel.color = new Color(0.9257517f, 0.9433962f, 0.8321466f);
    }
    public void unSelect(int action)
    {
        if (action == 0)
        {
            panel.color = Color.white;
        }
        else
        {
            StartCoroutine(CloseThisWindow());
        }
    }
    IEnumerator CloseThisWindow()
    {
        while(gameObject.transform.localScale.x > 0)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x - 0.01f, gameObject.transform.localScale.y - 0.01f, gameObject.transform.localScale.z - 0.01f);
            yield return null;
        }
        panel.color = Color.white;
        gameObject.SetActive(false);
        gameObject.transform.localScale = Vector3.zero;
    }

    
}
