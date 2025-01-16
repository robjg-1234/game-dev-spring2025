using UnityEngine;

public class ballScript : MonoBehaviour
{
    public Rigidbody rb;
    GameManager manager;
    int currentDifficulty = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameManager.instance;
        rb.linearVelocity = new Vector3(4, -10, 0);
    }
    private void OnDestroy()
    {
        manager.stopBallPlay();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (collision.gameObject.transform.GetComponent<BrickScript>().difficulty > currentDifficulty)
            {
                int setDifficulty = collision.gameObject.transform.GetComponent<BrickScript>().difficulty;
                if (rb.linearVelocity.y > 0 && setDifficulty> currentDifficulty)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5*setDifficulty, 0);
                    currentDifficulty = setDifficulty;
                } else if (rb.linearVelocity.y < 0 &&  setDifficulty> currentDifficulty)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, -5 * setDifficulty, 0);
                    currentDifficulty = setDifficulty;
                }
            }
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("paddle"))
        {
            ContactPoint moment = collision.GetContact(0);
            if ((moment.point.x > (collision.transform.position.x +1.25)) || (moment.point.x < (collision.transform.position.x - 1.25)))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x+10, rb.linearVelocity.y*1.25f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End Screen"))
        {
            Destroy(gameObject);
        }
    }
}
