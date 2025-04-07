using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NPCScript : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text dialogue;
    [SerializeField] string NPC_name;
    [SerializeField] Animator NPCController;
    [SerializeField] bool rats = false;
    public bool interactable = true;
    PlayerController pc;
    Vector3 originalRotation;
    [SerializeField] Vector3 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalRotation = this.transform.eulerAngles;
        if (rats)
        {
            NPCController.SetBool("Rats", rats);
        }
        GameManager.instance.finale += StartDancing;
        DialogueManager.DialogueAction += MoveToPosition;
    }
    private void OnDestroy()
    {
        GameManager.instance.finale -= StartDancing;
        DialogueManager.DialogueAction -= MoveToPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!dialoguePanel.activeSelf && interactable)
        {
            transform.eulerAngles = originalRotation;
        }
    }
    public void TalkToEm()
    {
        title.text = NPC_name;
        dialogue.text = DialogueManager.scc.getSCCLine(NPC_name);
        transform.eulerAngles = new Vector3(originalRotation.x, GameManager.instance.PCPlayer.transform.eulerAngles.y - 180, originalRotation.z);
        dialoguePanel.SetActive(true);
        if (NPC_name.Equals("Tom"))
        {
            if (DialogueManager.scc.checkCondition("doorOpen", "equals", true) && DialogueManager.scc.questState.Equals("Q1T1"))
            {
                GameManager.instance.changeGameState();
            }
            else if (DialogueManager.scc.checkCondition("diamondGrabbed", "equals", true) && DialogueManager.scc.questState.Equals("Q1T2"))
            {
                GameManager.instance.changeGameState();
            }
        }
    }
    public void StartDancing()
    {
        NPCController.SetBool("Dancing", true);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(targetPosition, 1f);
    }

    public void MoveToPosition()
    {
        if (DialogueManager.scc.checkCondition("doorOpen", "equals", true) && DialogueManager.scc.questState.Equals("Q1T1") && NPC_name.Equals("Tom"))
        {
            StartCoroutine(Walking());
        }
    }
    IEnumerator Walking()
    {
        while (GameManager.instance.PCPlayer.interacting && interactable)
        {
            yield return null;
        }
        NPCController.SetBool("Walking", true);
        interactable = false;
        originalRotation = new Vector3(0, 42.354f, 0);
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            agent.SetDestination(targetPosition);
            yield return null;
        }
        interactable = true;
        NPCController.SetBool("Walking", false);
    }
}
