using Unity.VisualScripting;
using UnityEngine;

public class HexaPiece : MonoBehaviour
{
    GameManager gm;
    PlayerScript target;
    Vector3 initialPosition;
    float collectionTimer = 1f;
    private void Start()
    {
        gm = GameManager.instance;
        initialPosition = transform.position;
        gm.playerDeath += UnselectTarget;
    }
    private void OnDestroy()
    {
        gm.playerDeath -= UnselectTarget;
        gm.CoinCollected += 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.005f);
            if (target.onSolidGround)
            {
                collectionTimer -= Time.deltaTime;
            }
            if (collectionTimer < 0f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform .position = initialPosition;
            collectionTimer = 1f;
        }
    }
    void UnselectTarget()
    {
        target = null;
    }
    public void SelectTarget(GameObject newTarget)
    {
        target = newTarget.GetComponent<PlayerScript>();
    }
}
