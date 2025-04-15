using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    [SerializeField] Camera cam;
    float yVelocity = 0f;
    float gravity = -10f;
    float pitch = 0f;
    float yaw = 0f;
    float sensitivityValue = 200f;
    float playerSpeed = 7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Dead Tree Trunk by Zsky[CC - BY] via Poly Pizza
    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!InkStoryManager.instance.knotActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (!cc.isGrounded)
            {
                yVelocity += gravity * Time.deltaTime;
            }
            else
            {
                yVelocity = -2;
            }

            float hAxis = Input.GetAxisRaw("Horizontal");
            float vAxis = Input.GetAxisRaw("Vertical");


            pitch += sensitivityValue * Input.GetAxis("Mouse Y") * -1 *Time.deltaTime;
            yaw += sensitivityValue * Input.GetAxis("Mouse X") * Time.deltaTime;
            //Limit how high or low you can go with the camera
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            //Wrap around if a complete 360 is done (horizontal Axis)
            while (yaw < 0f)
            {
                yaw += 360f;
            }
            while (yaw >= 360f)
            {
                yaw -= 360f;
            }

            gameObject.transform.eulerAngles = new Vector3(0f, yaw, 0f);
            cam.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
            Vector3 amountToMove = Vector3.zero;
            amountToMove += transform.forward.normalized * vAxis;
            amountToMove += transform.right.normalized * hAxis;
            amountToMove.Normalize();
            amountToMove *= playerSpeed;
            amountToMove.y += yVelocity;
            amountToMove *= Time.deltaTime;
            cc.Move(amountToMove);
            amountToMove = new Vector3(0, 0, 0);
            if (Input.GetMouseButtonDown(1))
            {
                Ray mousePositionRay = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(mousePositionRay, out hitInfo, 3f))
                {
                    if (hitInfo.collider.CompareTag("NPC"))
                    {
                        hitInfo.collider.GetComponent<NPCScript>().StartConversation();
                    }
                }
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("coins"))
        {
            Destroy(other.gameObject);
            InkStoryManager.instance.changeMoney(5);
        }
    }
}
