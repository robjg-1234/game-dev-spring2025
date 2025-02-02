using UnityEngine;

public class paddleScriptVertical : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float yAxis = Input.GetAxisRaw("Vertical");
        if ((transform.position.y > 4 && yAxis > 0) || (transform.position.y < -4 && yAxis < 0))
        {
            transform.position = transform.position;
        }
        else
        {
            transform.position += Vector3.up * yAxis * 15 * Time.deltaTime;
        }
    }
}
