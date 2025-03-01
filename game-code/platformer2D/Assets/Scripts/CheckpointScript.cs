using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public void SelectCheckpoint()
    {
        gameManager.SetNewRespawnPosition(this);
        transform.gameObject.SetActive(false);
    }
    public void Unselect()
    {
        transform.gameObject.SetActive(true);
    }
}
