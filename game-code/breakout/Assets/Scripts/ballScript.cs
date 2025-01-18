using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ballScript : MonoBehaviour
{
    public Rigidbody rb;
    GameManager manager;
    int currentDifficulty = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameManager.instance;
        currentDifficulty = 2 + manager.level - 1;
        rb.linearVelocity = new Vector3(4 * (Random.Range(0, 2) * 2 - 1), -5 * currentDifficulty, 0);
        manager.callBall += freeze;
    }
    private void OnDestroy()
    {
        manager.stopBallPlay();
        manager.callBall -= freeze; 
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            rb.linearVelocity = new Vector3(4, -5 * currentDifficulty, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (collision.gameObject.transform.GetComponent<BrickScript>().difficulty > currentDifficulty)
            {
                currentDifficulty = collision.gameObject.transform.GetComponent<BrickScript>().difficulty;
                if (rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -8f, 8f), 5 * currentDifficulty, 0);
                }
                else if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -8f, 8f), -5 * currentDifficulty, 0);
                }
            }
            else
            {
                if (rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -8f, 8f), 5 * currentDifficulty, 0);
                }
                else if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -8f, 8f), -5 * currentDifficulty, 0);
                }
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("paddle"))
        {
            ContactPoint moment = collision.GetContact(0);
            if (moment.point.x > (collision.transform.position.x + 1.25))
            {
                rb.linearVelocity = new Vector3(30, rb.linearVelocity.y, 0);
            }
            else if (moment.point.x < (collision.transform.position.x - 1.25))
            {
                rb.linearVelocity = new Vector3(-30, rb.linearVelocity.y, 0);
            }
            else 
            {
                rb.linearVelocity = new Vector3(Mathf.Clamp(rb.linearVelocity.x, -6f,6f), 5 *currentDifficulty, 0);
            }
            manager.hitCounter();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End Screen"))
        {
            Destroy(gameObject);
        }
    }
    void freeze()
    {
        StartCoroutine(freezeBall());
    }
    public IEnumerator freezeBall()
    {
        float velY = rb.linearVelocity.y;
        float velX = rb.linearVelocity.x;
        float velZ = rb.linearVelocity.z;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(3);
        rb.linearVelocity = new Vector3(velX, velY, velZ);
    }
}
