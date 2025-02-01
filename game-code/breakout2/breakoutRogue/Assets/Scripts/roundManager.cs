using UnityEngine;

public class roundManager : MonoBehaviour
{
    [SerializeField] GameObject option1;
    [SerializeField] GameObject option2;
    [SerializeField] GameObject option3;
    gameManager gm;
    private void Start()
    {
        gm = gameManager.instance;
        StartCoroutine(gm.roundCoroutine());
    }
    public void startRound()
    {
        option1.SetActive(true);
        option1.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick());
        option2.SetActive(true);
        option2.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick());
        option3.SetActive(true);
        option3.GetComponent<choiceScript>().changeOption(gm.selectRandomBrick());
    }
}
