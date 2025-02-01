using Unity.VisualScripting;
using UnityEngine;

public class ballScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    gameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int initialX = Random.Range(1, 6);
        rb.linearVelocity = new Vector2(initialX * (Random.Range(0,2)*2-1), -5);
        gm = gameManager.instance;
    }

    private void OnDestroy()
    {
        gm.announceRoundStart();
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocity.y<3 && rb.linearVelocity.y > -3)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -5);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("paddle"))
        {
            
            ContactPoint2D hit = collision.GetContact(0);
            if (hit.point.x < collision.transform.position.x - 0.75)
            {
                rb.linearVelocity = new Vector2(-4, rb.linearVelocity.y);
            }
            else if (hit.point.x > collision.transform.position.x + 0.75)
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
            gm.checkPaddle();
        }
        else if (collision.transform.CompareTag("brick"))
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
            collision.transform.gameObject.GetComponent<brickScript>().tryToBreak();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("deadScreen"))
        {
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("brick"))
        {
            brickScript hitBrick = collision.transform.GetComponent<brickScript>();
            hitBrick.tryToBreak();
            if (hitBrick.brickType == 9)
            {
                hitBrick.ballsConsumed += 1;
                Destroy(gameObject);
            }
        }
    }
    private Vector2 vectorCoverter(Vector3 position)
    {
        Vector2 newVector = new Vector2(position.x, position.y);
        return newVector;
    }
}
