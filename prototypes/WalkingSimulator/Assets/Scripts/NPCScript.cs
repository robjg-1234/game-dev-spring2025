using TMPro;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text dialogue;
    [SerializeField] string NPC_name;
    [SerializeField] Animator NPCController;
    [SerializeField] bool rats = false;
    PlayerController pc;
    Vector3 originalRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalRotation = this.transform.eulerAngles;
        if (rats)
        {
            NPCController.SetBool("Rats", rats);
        }
        GameManager.instance.finale += StartDancing;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!dialoguePanel.activeSelf)
        {
            transform.eulerAngles = originalRotation;
        }
    }
    public void TalkToEm()
    {
        title.text = NPC_name;
        dialogue.text = DialogueManager.scc.getSCCLine(NPC_name);
        transform.eulerAngles = new Vector3(originalRotation.x, GameManager.instance.PCPlayer.transform.eulerAngles.y-180, originalRotation.z);
        dialoguePanel.SetActive(true);
        if (NPC_name.Equals("Tom"))
        {
            if (DialogueManager.scc.checkCondition("doorOpen", "equals", "true") && DialogueManager.scc.questState.Equals("Q1T1"))
            {
                GameManager.instance.changeGameState();
            }
            else if (DialogueManager.scc.checkCondition("diamondGrabbed", "equals", "true") && DialogueManager.scc.questState.Equals("Q1T2"))
            {
                GameManager.instance.changeGameState();
            }
        }
    }
    public void StartDancing()
    {
        NPCController.SetBool("Dancing", true);
    }
}
