using System.Collections;
using UnityEngine;

public class SecretRooms : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator revealRoom()
    {
        while (rend.color.a > 0)
        {
            if (!gameManager.isPaused)
            {
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, rend.color.a - 1f * Time.deltaTime);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
