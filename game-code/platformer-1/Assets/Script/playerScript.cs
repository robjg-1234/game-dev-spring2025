using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class playerScript : MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] GameObject underCam;
    [SerializeField] GameObject mainCam;
    Color defaultColor;
    GameObject lastWall;
    GameObject currentWall;
    float velocity = 0;
    float groundedSpeed = 10f;
    float characterSpeed = 15f;
    float friction = 30f;
    float jumpVelocity = 0;
    float jumpStrength = 10f;
    float gravity = -10f;
    float jumpBuffer = 0;
    CharacterController cc;
    [SerializeField] GameObject cam;
    Vector3 movement = Vector3.zero;
    Vector3 lastDir = Vector3.zero;
    float dashCooldown = 0;
    float lastX = 0;
    float lastZ = 0;
    bool dashing = false;
    float dashSpeed = 35f;
    float dashDuration = 0;
    bool wallJumping = false;
    float wallJumpingState = 0;
    float dashBuffer = 0;
    bool onWall = false;
    float coyoteTime = 0.1f;
    float fallTime = 0;
    bool gameWon = false;
    managerScript gm;
    Vector3 fixedMovement = Vector3.zero;
    void Start()
    {
        gm = managerScript.instance;
        cc = GetComponent<CharacterController>();
        gm.gameWon += stopGame;
        defaultColor = rend.material.color;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.C))
        {
            dashBuffer = 0.05f;
        }
    }
    void FixedUpdate()
    {
        if (!gameWon)
        {
            float xAxis = Input.GetAxisRaw("Horizontal");
            float zAxis = Input.GetAxisRaw("Vertical");
            //jump 
            if (cc.isGrounded)
            {
                lastWall = null;
                fallTime = 0;
                lastX = 0;
                lastZ = 0;
                jumpVelocity = -1f;
                gravity = -10f;
                if (!dashing)
                {
                    if (Input.GetKeyDown(KeyCode.Space) || jumpBuffer > 0)
                    {
                        jumpVelocity = jumpStrength;
                        lastX = xAxis;
                        lastZ = zAxis;
                        jumpBuffer = 0;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) || jumpBuffer > 0)
                {
                    if (fallTime < coyoteTime)
                    {
                        gravity = -10f;
                        jumpVelocity = jumpStrength;
                        lastX = xAxis;
                        lastZ = zAxis;
                        jumpBuffer = 0;
                    }
                }
                fallTime += Time.deltaTime;
                if (!dashing)
                {
                    jumpVelocity += gravity * Time.deltaTime;
                    if (jumpVelocity < 0)
                    {
                        gravity = -30f;
                    }
                    if (jumpBuffer > 0)
                    {
                        jumpBuffer -= Time.deltaTime;
                    }
                    else
                    {
                        jumpBuffer = 0;
                    }
                    if (onWall && velocity > 9)
                    {
                        if (currentWall?.gameObject != lastWall?.gameObject)
                        {
                            if (Input.GetKeyDown(KeyCode.Space) || jumpBuffer > 0)
                            {
                                lastWall = currentWall;
                                gravity = -10f;
                                wallJumpingState = 0.1f;
                                jumpVelocity = jumpStrength;
                                xAxis *= -1;
                                zAxis *= -1;
                                lastX *= -1;
                                lastZ *= -1;
                                jumpBuffer = 0;
                            }
                        }
                    }
                }
            }
            //Ground movement
            if (!dashing)
            {
                if (xAxis != 0 || zAxis != 0)
                {
                    if (!cc.isGrounded)
                    {
                        if (!wallJumping)
                        {
                            if (velocity <= 0)
                            {
                                if (lastX == 0 && lastZ == 0)
                                {
                                    lastX = xAxis;
                                    lastZ = zAxis;
                                }
                                else
                                {
                                    lastX *= -1;
                                    lastZ *= -1;
                                }

                                velocity += characterSpeed * Time.deltaTime;
                            }
                            else if (xAxis == lastX && zAxis == lastZ)
                            {
                                if (velocity < 10f)
                                {
                                    velocity += characterSpeed * Time.deltaTime;
                                }
                                else
                                {
                                    if (velocity > 0)
                                    {
                                        velocity -= characterSpeed * Time.deltaTime;
                                    }
                                }
                            }
                            else
                            {
                                if (velocity > 0)
                                {
                                    velocity -= characterSpeed * Time.deltaTime;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (velocity < 15)
                        {
                            velocity += groundedSpeed * Time.deltaTime;
                        }
                        else
                        {
                            if (velocity > 0)
                            {
                                velocity -= friction * Time.deltaTime;
                            }
                            else
                            {
                                velocity = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (velocity > 5)
                    {
                        velocity = 5;
                    }
                    else if (velocity > 0)
                    {
                        velocity -= friction * Time.deltaTime;
                    }
                    else
                    {
                        velocity = 0;
                    }
                }
                Vector3 fixedRight = cam.transform.right;
                fixedRight.y = 0;
                Vector3 fixedForward = cam.transform.forward;
                fixedForward.y = 0;
                if (wallJumpingState > 0)
                {
                    wallJumpingState -= Time.deltaTime;
                }
                else
                {
                    wallJumping = false;
                    wallJumpingState = 0;
                }
                if (!cc.isGrounded)
                {
                    if ((xAxis == lastX && xAxis != 0) || (zAxis == lastZ && zAxis != 0))
                    {
                        if (velocity < 10)
                        {
                            velocity += characterSpeed * Time.deltaTime;
                        }
                        else
                        {
                            velocity -= characterSpeed * Time.deltaTime;
                        }
                        movement += fixedRight.normalized * xAxis;
                        movement += fixedForward.normalized * zAxis;
                    }
                    else
                    {
                        velocity -= characterSpeed * Time.deltaTime;
                    }
                    if (dashCooldown <= 0)
                    {
                        rend.material.color = defaultColor;
                        dashCooldown = 0;
                    }
                }
                else
                {
                    movement += fixedRight.normalized * xAxis;
                    movement += fixedForward.normalized * zAxis;
                    if (dashCooldown > 0)
                    {
                        dashCooldown -= Time.deltaTime;
                    }
                    else
                    {
                        rend.material.color = defaultColor;
                        dashCooldown = 0;
                    }
                }
                if (dashCooldown <= 0 && !dashing)
                {
                    if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.C) || dashBuffer > 0)
                    {
                        if (xAxis != 0 || zAxis != 0)
                        {
                            rend.material.color = Color.red;
                            movement += fixedRight.normalized * xAxis;
                            movement += fixedForward.normalized * zAxis;
                            lastX = xAxis;
                            lastZ = zAxis;
                            jumpVelocity = 0;
                            velocity = dashSpeed;
                            dashing = true;
                            dashCooldown = 0.005f;
                            dashBuffer = 0;
                        }
                    }
                }
            }
            else
            {
                movement = lastDir;
                if (dashDuration < 0.25f)
                {
                    dashDuration += Time.deltaTime;
                }
                else
                {
                    dashing = false;
                    dashDuration = 0;
                }
            }
            if (dashBuffer > 0)
            {
                dashBuffer -= Time.deltaTime;
            }
            else
            {
                dashBuffer = 0;
            }
            movement.Normalize();
            lastDir = movement;
            movement *= velocity;
            movement.y = jumpVelocity;
            movement *= Time.deltaTime;
            cc.Move(movement);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("wall"))
        {
            onWall = true;
            currentWall = other.gameObject;
        }
        else if (other.transform.CompareTag("Underpass"))
        {
            mainCam.gameObject.SetActive(false);
            underCam.gameObject.SetActive(true);
        }
        else if (other.transform.CompareTag("roof"))
        {
            jumpVelocity = 0;
        }
        else if (other.transform.CompareTag("coin"))
        {
            Destroy(other.gameObject);
            gm.CoinCollected();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("wall"))
        {
            onWall = false;
        }
        else if (other.transform.CompareTag("Underpass"))
        {
            mainCam.gameObject.SetActive(true);
            underCam.gameObject.SetActive(false);
        }
    }
    void stopGame()
    {
        gameWon = true;
    }
}
