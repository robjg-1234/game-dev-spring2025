using System;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class GameManager : MonoBehaviour
{
    public PlayerController PCPlayer;
    public static GameManager instance;
    [SerializeField] Animator doorAnimator;
    public Action finale;
    int gameState = 0;
    //broken bookshelf by Justin Randall[CC - BY] via Poly Pizza
    //Standing Desk by Zsky [CC-BY] via Poly Pizza
    //Diamond Ring by mehreen1919[CC - BY] via Poly Pizza
    //Rose by Zsky[CC - BY] via Poly Pizza
    //Desert marigold by Poly by Google[CC - BY] via Poly Pizza
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }
    public void changeGameState()
    {
        gameState++;
        if (gameState == 1 )
        {
            doorAnimator.SetBool("Door Open", true);
            DialogueManager.DialogueAction.Invoke();
            DialogueManager.scc.questState = "Q1T2";
        }
        else if (gameState == 2 )
        {
            if (finale != null)
            {
                finale.Invoke();
            }
        }
    }
}
