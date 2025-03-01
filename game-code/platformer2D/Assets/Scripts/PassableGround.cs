using UnityEngine;

public class PassableGround : MonoBehaviour
{
    GameManager manager;
    GameObject targetPlayer;
    [SerializeField] GameObject platformDecoy;
    // Update is called once per frame
    private void Start()
    {
        manager = GameManager.instance;
    }
    void Update()
    {
        if (manager.currentPlayerIteration != null)
        {
            if ((manager.currentPlayerIteration.gameObject.transform.position.y - 0.5f > transform.position.y + (transform.localScale.y / 2)) && !(manager.currentPlayerIteration.yVelocity>0))
            {
                platformDecoy.SetActive(true);
            }
            else
            {
                platformDecoy.SetActive(false);
            }
        }
    }
}
