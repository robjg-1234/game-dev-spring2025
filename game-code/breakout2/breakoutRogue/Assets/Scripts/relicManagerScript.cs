using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class relicManagerScript : MonoBehaviour
{
    [SerializeField] GameObject option1;
    [SerializeField] GameObject option2;
    [SerializeField] GameObject option3;
    [SerializeField] Image relicSpot1;
    [SerializeField] Image relicSpot2;
    [SerializeField] roundManager rm;
    int[] availableRelics = new int[] { 0, 1, 2, 3, 4, 5 };
    int selectionNumber = 0;
    gameManager gm;
    private void Start()
    {
        gm = gameManager.instance;
    }
    public void RelicChosen(int relicNum, Sprite placeHolder)
    {
        availableRelics[relicNum] = -1;
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        if (selectionNumber == 0)
        {
            relicSpot1.sprite = placeHolder;
            relicSpot1.color = Color.white;
            selectionNumber++;
        }
        else
        {
            relicSpot2.sprite = placeHolder;
            relicSpot2.color = Color.white;
        }
        rm.startRound();
    }
    public void ShowRelics()
    {
        int passes = 0;
        int[] tempRelics = availableRelics;
        tempRelics = gm.shuffleArray(tempRelics);
        for (int i = 0; i < tempRelics.Length; i++)
        {
            if (tempRelics[i] >=0)
            {
                if (passes ==0)
                {
                    option1.GetComponent<relicPageScript>().chooseRelic(tempRelics[i]);
                    passes++;
                }
                else if (passes ==1)
                {
                    option2.GetComponent<relicPageScript>().chooseRelic(tempRelics[i]);
                    passes++;
                }
                else if (passes == 2)
                {
                    option3.GetComponent<relicPageScript>().chooseRelic(tempRelics[i]);
                    passes++;
                }
            }
        }
        Debug.Log(tempRelics[0]);
        Debug.Log(tempRelics[1]);
        Debug.Log(tempRelics[2]);
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
    }
    public void skipRelic()
    {
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        rm.startRound();
    }
}
