using System.Collections;
using UnityEngine;

public class SecretRooms : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    public IEnumerator revealRoom()
    {
        while (rend.color.a > 0)
        {
            rend.color = new Color(rend.color.a, rend.color.g, rend.color.b, rend.color.a - 1f*Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
