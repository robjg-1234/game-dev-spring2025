using TMPro;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text dialogue;
    [SerializeField] string NPC_name;
    [SerializeField] Animator NPCController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void TalkToEm()
    {
        title.text = NPC_name;
        dialogue.text = DialogueManager.scc.getSCCLine(NPC_name);
        dialoguePanel.SetActive(true);
    }

}
