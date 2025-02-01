using UnityEngine;

public class paddleScript : MonoBehaviour
{
    gameManager gm;
    float speed = 15f;
    // Update is called once per frame
    private void Start()
    {
        gm = gameManager.instance;
    }
    void Update()
    {
        if (gm.gameRunning)
        {
            float hAxis = Input.GetAxisRaw("Horizontal");
            if ((transform.position.x > 6.7 && hAxis > 0) || (transform.position.x < -6.7 && hAxis < 0))
            {
                transform.position = transform.position;
            }
            else
            {
                transform.position += Vector3.right * hAxis * speed * Time.deltaTime;
            }
        }
    }
}
