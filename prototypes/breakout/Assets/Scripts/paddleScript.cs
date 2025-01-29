using UnityEngine;

public class paddleScript : MonoBehaviour
{
    public float speed = 26f;
    [SerializeField] Renderer rend;
    Color defaultColor = Color.white;
    int controllerInversion = 1;
    // Update is called once per frame
    void FixedUpdate()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        if (transform.position.x > 23.2f && hAxis*controllerInversion > 0)
        {
            transform.position = transform.position;
        }
        else if (transform.position.x < -21.9f && hAxis*controllerInversion < 0)
        {
            transform.position = transform.position;
        }
        else if (hAxis != 0)
        {
            transform.position += Vector3.right * hAxis * speed * Time.deltaTime * controllerInversion;
        }
    }
    public void changeColor(Color tempColor)
    {
        rend.material.color = tempColor;
    }
    public void changeController(int direction)
    {
        controllerInversion = direction;
    }
}
