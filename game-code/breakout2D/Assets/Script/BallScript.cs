using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    private void Start()
    {
        rb.linearVelocity = new Vector3(3 * Random.Range(0, 2) * 2 - 1, -5, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("brick"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("xPaddle"))
        {
            ContactPoint2D hit = collision.GetContact(0);
            if (hit.point.x < collision.transform.position.x - 0.65)
            {
                rb.linearVelocity = new Vector2(-4, rb.linearVelocity.y);
            }
            else if (hit.point.x > collision.transform.position.x + 0.65)
            {
                rb.linearVelocity = new Vector2(4, rb.linearVelocity.y);
            }
            else
            {
                if (rb.linearVelocity.x < 0)
                {
                    if (rb.linearVelocity.y < 0)
                    {
                        rb.linearVelocity = new Vector2(-5, -5);
                    }
                    else if (rb.linearVelocity.y > 0)
                    {
                        rb.linearVelocity = new Vector2(-5, 5);
                    }
                }
                else if (rb.linearVelocity.x > 0)
                {
                    if (rb.linearVelocity.y < 0)
                    {
                        rb.linearVelocity = new Vector2(5, -5);
                    }
                    else if (rb.linearVelocity.y > 0)
                    {
                        rb.linearVelocity = new Vector2(5, 5);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("deadZone"))
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        gameManager.instance.ballAlive = false;
    }
}
