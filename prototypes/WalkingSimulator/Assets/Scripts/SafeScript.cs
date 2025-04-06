
using System.Collections;
using UnityEngine;

public class SafeScript : MonoBehaviour
{
    public static SafeScript instance;
    [SerializeField] GameObject doorSafe;
    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;
    [SerializeField] GameObject light3;
    [SerializeField] GameObject light4;
    bool waitForResult = false;
    Renderer rendCurrentCell;
    Color defaultColor;
    Collider trigger;
    int code = 9774;
    int tempCode = 0;
    int numPos =0;
    private void Start()
    {
        SafeScript.instance = this;
        trigger = GetComponent<Collider>();
        rendCurrentCell = light1.GetComponent<Renderer>();
        defaultColor = rendCurrentCell.material.color;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeactivatePuzzleScreen();
        }
    }
    public void ActivatePuzzleScreen() { trigger.enabled = false; }
    public void DeactivatePuzzleScreen() { trigger.enabled = true; }

    public void AddToCode(int num)
    {
        if (doorSafe.activeSelf && !waitForResult)
        {
            if (numPos == 0)
            {
                tempCode += num * 1000;
                rendCurrentCell = light1.GetComponent<Renderer>();
                rendCurrentCell.material.color = Color.cyan;
            }
            else if (numPos == 1)
            {
                tempCode += num * 100;
                rendCurrentCell = light2.GetComponent<Renderer>();
                rendCurrentCell.material.color = Color.cyan;
            }
            else if (numPos == 2)
            {
                tempCode += num * 10;
                rendCurrentCell = light3.GetComponent<Renderer>();
                rendCurrentCell.material.color = Color.cyan;
            }
            else if (numPos == 3)
            {
                tempCode += num;
                rendCurrentCell = light4.GetComponent<Renderer>();
                rendCurrentCell.material.color = Color.cyan;
            }
            numPos++;
            if (numPos == 4)
            {
                waitForResult = true;
                TestCode();
            }
        }
    }
    void TestCode()
    {
        if (tempCode == code)
        {
            rendCurrentCell = light1.GetComponent<Renderer>();
            rendCurrentCell.material.color = Color.green;
            rendCurrentCell = light2.GetComponent<Renderer>();
            rendCurrentCell.material.color = Color.green;
            rendCurrentCell = light3.GetComponent<Renderer>();
            rendCurrentCell.material.color = Color.green;
            rendCurrentCell = light4.GetComponent<Renderer>();
            rendCurrentCell.material.color = Color.green;
            doorSafe.SetActive(false);
            DialogueManager.scc.setGameStateValue("safeClosed", "set", "false");
        }
        else
        {
            numPos = 0;
            tempCode = 0;
            StartCoroutine(WrongCode());
        }
    }
    IEnumerator WrongCode()
    {
        rendCurrentCell = light1.GetComponent<Renderer>();
        rendCurrentCell.material.color = Color.red;
        rendCurrentCell = light2.GetComponent<Renderer>();
        rendCurrentCell.material.color = Color.red;
        rendCurrentCell = light3.GetComponent<Renderer>();
        rendCurrentCell.material.color = Color.red;
        rendCurrentCell = light4.GetComponent<Renderer>();
        rendCurrentCell.material.color = Color.red;
        yield return new WaitForSeconds(1f);
        rendCurrentCell = light1.GetComponent<Renderer>();
        rendCurrentCell.material.color = defaultColor;
        rendCurrentCell = light2.GetComponent<Renderer>();
        rendCurrentCell.material.color = defaultColor;
        rendCurrentCell = light3.GetComponent<Renderer>();
        rendCurrentCell.material.color = defaultColor;
        rendCurrentCell = light4.GetComponent<Renderer>();
        rendCurrentCell.material.color = defaultColor;
        waitForResult = false;
    }

}
