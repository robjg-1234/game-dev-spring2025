using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [SerializeField] GameObject decoy;
    GameManager gm;
    private void Start()
    {
        gm = GameManager.instance;
        gm.playerJump += ToggleActivty;
    }
    private void OnDestroy()
    {
        gm.playerJump -= ToggleActivty;
    }
    public void ToggleActivty()
    {
        if (decoy.activeSelf)
        {
            decoy.SetActive(false);
        }
        else
        {
            decoy.SetActive(true);
        }
    }
}
