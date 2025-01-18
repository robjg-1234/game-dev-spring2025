using UnityEngine;

public class paddleScript : MonoBehaviour
{
    float speed = 35f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        if (transform.position.x > 23.7f && hAxis > 0)
        {
            transform.position = transform.position;
        }
         else if (transform.position.x < -22.4f && hAxis < 0)
        {
            transform.position = transform.position;
        }
        else
        {
            transform.position = Vector3.right * hAxis * speed * Time.deltaTime + transform.position;
        }
    }
}
