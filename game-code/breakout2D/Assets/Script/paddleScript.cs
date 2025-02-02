using UnityEngine;

public class paddleScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        if ((transform.position.x > 7 && hAxis > 0) || (transform.position.x < -7 && hAxis < 0))
        {
            transform.position = transform.position;
        }
        else
        {
            transform.position += Vector3.right * hAxis * 15 * Time.deltaTime;
        }
    }
}
