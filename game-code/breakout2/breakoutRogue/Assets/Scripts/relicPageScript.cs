using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class relicPageScript : MonoBehaviour
{
    string[] imgSource = new string[] { "BigDonut", "Desperado", "FaultyEquipment", "FullMetalJacket", "HandOfGreed", "UtilityBelt" };
    [SerializeField] Image spriteHolder;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDescription;
    [SerializeField] relicManagerScript operatorScript;
    gameManager gm;
    int relicNumber;
    private void Start()
    {
        gm = gameManager.instance;
    }
    public void chooseRelic(int relic)
    {
        relicNumber = relic;
        switch(relic)
        {
            case 0:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Big Donut";
                relicDescription.text = "Whenever a ball is consumed, it is counted twice.";
                break;
            case 1:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Desperado";
                relicDescription.text = "Every score gains a 2x multiplier. The paddle has 10 less bounces.";
                break;
            case 2:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Faulty Equipment";
                relicDescription.text = "The ball has a 50% chance of not using a paddle bounce, but each bounce counts as 2.";
                break;
            case 3:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Full Metal Jacket";
                relicDescription.text = "The paddle gains 10 extra bounces. Paddle speed is doubled.";
                break;
            case 4:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Hand Of Greed";
                relicDescription.text = "Gain 32 rerolls, you can no longer get rerolls.";
                break;
            case 5:
                spriteHolder.sprite = Resources.Load<Sprite>(imgSource[relic]);
                relicName.text = "Utility Belt";
                relicDescription.text = "Whenever you get rerolls, gain 2 more.";
                break;
        }
    }
    public void selectRelic()
    {
        switch (relicNumber)
        {
            case 0:
                gm.bigDonut = true;
                break;
            case 1:
                gm.desperadoMult = 2;
                gm.defaultPaddle -= 10;
                gm.fixPaddle();
                break;
            case 2:
                gm.faultyEquipment = true;
                break;
            case 3:
                gm.FullMetalJacketChange();
                gm.defaultPaddle += 10;
                gm.fixPaddle();
                break;
            case 4:
                gm.gainRerolls(32);
                break;
            case 5:
                gm.utilityBelt = true;
                break;
        }
        operatorScript.RelicChosen(relicNumber, Resources.Load<Sprite>(imgSource[relicNumber]));
    }
}
