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
        
        for (int i = 0; i < availableRelics.Length; i++)
        {
            if (availableRelics[i] == relicNum)
            {
                availableRelics[i] = -1;
            }
        }
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
        StartCoroutine(MinimizeAllWindows());
    }
    public void ShowRelics()
    {
        int passes = 0;
        int[] tempRelics = availableRelics;
        tempRelics = gm.shuffleArray(tempRelics);
        for (int i = 0; i < tempRelics.Length; i++)
        {
            if (tempRelics[i] >= 0)
            {
                if (passes == 0)
                {
                    option1.GetComponent<relicPageScript>().chooseRelic(tempRelics[i]);
                    passes++;
                }
                else if (passes == 1)
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
        StartCoroutine(MaximizeAllWindows());
    }
    public void skipRelic()
    {
        StartCoroutine(MinimizeAllWindows());
    }
    IEnumerator MinimizeAllWindows()
    {
        while (option1.transform.localScale.x > 0)
        {
            option1.transform.localScale = new Vector3(option1.transform.localScale.x - 0.01f, option1.transform.localScale.y - 0.01f, option1.transform.localScale.z - 0.01f);
            option2.transform.localScale = new Vector3(option2.transform.localScale.x - 0.01f, option2.transform.localScale.y - 0.01f, option2.transform.localScale.z - 0.01f);
            option3.transform.localScale = new Vector3(option3.transform.localScale.x - 0.01f, option3.transform.localScale.y - 0.01f, option3.transform.localScale.z - 0.01f);
            yield return null;
        }
        option1.transform.localScale = Vector3.zero;
        option2.transform.localScale = Vector3.zero;
        option3.transform.localScale = Vector3.zero;
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        rm.startRound();
    }

    IEnumerator MaximizeAllWindows()
    {
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
        while (option1.transform.localScale.x < 1)
        {
            option1.transform.localScale = new Vector3(option1.transform.localScale.x + 0.01f, option1.transform.localScale.y + 0.01f, option1.transform.localScale.z + 0.01f);
            option2.transform.localScale = new Vector3(option2.transform.localScale.x + 0.01f, option2.transform.localScale.y + 0.01f, option2.transform.localScale.z + 0.01f);
            option3.transform.localScale = new Vector3(option3.transform.localScale.x + 0.01f, option3.transform.localScale.y + 0.01f, option3.transform.localScale.z + 0.01f);
            yield return null;
        }
        option1.transform.localScale = new Vector3(1, 1, 1);
        option2.transform.localScale = new Vector3(1, 1, 1);
        option3.transform.localScale = new Vector3(1, 1, 1);
    }
}
