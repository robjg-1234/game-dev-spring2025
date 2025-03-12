using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Vector2 boxSize;
    [SerializeField] LayerMask ground;
    [SerializeField] float castDistance;
    [SerializeField] Vector2 shieldLocation;
    [SerializeField] float shieldDistance;
    [SerializeField] LayerMask shieldInteractable;
    [SerializeField] Vector2 verticalShieldLocation;
    [SerializeField] float verticalShieldDistance;
    [SerializeField] GameObject shield;
    [SerializeField] SpriteRenderer shieldrend;
    [SerializeField] LayerMask passableGround;
    GameManager gameManager;
    Rigidbody2D rb;
    Color defaultShieldColor;
    public float yVelocity = 0;
    float gravity = 10f;
    float jumpBuffer = 0f;
    float velocity = 0;
    bool jumped = false;
    float speed;
    float yAxis = 0;
    float xAxis = 0;
    float directionFlipTimer = 0f;
    bool jumpStopBuffer = false;
    bool inSpike = false;
    float deathBuffer = 0.01f;
    float direction = 0;
    float coyoteTime = 0.1f;
    float fallTime = 0f;
    float jumpingTimer = 0f;
    ArrayList usedDir = new ArrayList() { };
    bool transitioning = false;
    bool respawning = true;
    bool inSpring = false;
    bool unableToJumpStop = false;
    public bool onSolidGround = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.instance;
        defaultShieldColor = shieldrend.color;
        speed = 4f;
        rb = GetComponent<Rigidbody2D>();
        gameManager.currentPlayerIteration = this;
    }
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.RespawnPlayer();
        }
    }

    private void Update()
    {
        if (!gameManager.isPaused)
        {
            xAxis = Input.GetAxisRaw("Horizontal");
            yAxis = Input.GetAxisRaw("Vertical");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpBuffer = 0.2f;
            }
            if (Input.GetKeyUp(KeyCode.Space) && (usedDir.Count == 0) && !unableToJumpStop && jumped)
            {
                jumpStopBuffer = true;
            }
            if (Input.GetKey(KeyCode.C))
            {
                HoldShield();
            }
            else
            {
                shield.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space) && (usedDir.Count == 0) && !unableToJumpStop && jumped)
            {
                jumpStopBuffer = true;
            }
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!gameManager.isPaused)
        {
            if (jumpBuffer > 0)
            {
                jumpBuffer -= Time.deltaTime;
            }
            else
            {
                jumpBuffer = 0;
            }
            if (!IsGrounded())
            {
                if (jumpingTimer > 0)
                {
                    jumpingTimer -= Time.deltaTime;
                }
                else
                {
                    jumpingTimer = 0;
                }
                fallTime += Time.deltaTime;
                if (fallTime <= coyoteTime && !jumped && jumpBuffer > 0)
                {
                    gameManager.Jump();
                    jumped = true;
                    yVelocity = 6f;
                    jumpBuffer = 0;
                }
                if (speed > 4.75f)
                {
                    speed -= 8f * Time.deltaTime;
                }
                else
                {
                    speed = 4.75f;
                }

                if (yVelocity > 0 && jumpStopBuffer && jumpingTimer <= 0)
                {
                    yVelocity = 0;
                    jumpStopBuffer = false;
                }
                if (yVelocity > 0 && HeadHitter())
                {
                    yVelocity = 0;
                }
                if (yVelocity < 0)
                {
                    unableToJumpStop = true;
                }
                yVelocity -= gravity * Time.deltaTime;
            }
            else
            {
                respawning = false;
                jumped = false;
                fallTime = 0;
                jumpStopBuffer = false;
                unableToJumpStop = false;
                if (!inSpring)
                {
                    yVelocity = 0;
                }
                if (jumpBuffer > 0)
                {
                    gameManager.Jump();
                    jumpingTimer = 0.1f;
                    jumped = true;
                    yVelocity = 6f;
                    jumpBuffer = 0;
                }
            }
            if (transitioning)
            {
                velocity = direction * speed;
            }
            else
            {
                if (directionFlipTimer > 0f)
                {
                    directionFlipTimer -= Time.deltaTime;
                    velocity = direction * speed;
                }
                else
                {
                    if (xAxis != 0)
                    {
                        if (!IsGrounded())
                        {
                            if (velocity > speed * 1.2f)
                            {
                                velocity -= 12f * Time.deltaTime;
                            }
                            else if (velocity < speed * 1.2f * -1)
                            {
                                velocity += 12f * Time.deltaTime;
                            }
                            else
                            {
                                velocity += speed * 1.4f * xAxis * Time.deltaTime;
                            }
                        }
                        else
                        {
                            if (speed > 4f)
                            {
                                speed -= 100f * Time.deltaTime;
                                velocity = speed * xAxis;
                            }
                            else
                            {
                                speed = 4f;
                                velocity = xAxis * speed;
                            }
                        }
                    }
                    else
                    {
                        velocity = 0;
                    }
                }
            }
            rb.linearVelocity = new Vector2(velocity, yVelocity);
            if (inSpike)
            {
                if (deathBuffer > 0)
                {
                    deathBuffer -= Time.deltaTime;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                deathBuffer = 0.005f;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2 (0, 0);
        }

    }

    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, ground))
        {
            usedDir.Clear();
            onSolidGround = true;
            return true;

        }
        else if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, passableGround))
        {
            usedDir.Clear();
            onSolidGround = false;
            return true;
        }
        else
        {
            onSolidGround = false;
            return false;
        }
    }
    public bool HeadHitter()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, transform.up, castDistance, ground))
        {
            return true;

        }
        else
        {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * shieldDistance, shieldLocation);
        Gizmos.DrawWireCube(transform.position + transform.right * verticalShieldDistance, verticalShieldLocation);
    }
    public void HoldShield()
    {
        int dir = -1;
        // down = 1 / 0.1 (-0.55) =0
        // up = 1 / 0.1 (0.55) = 1
        // left = 0.1/ 1 (-0.55) = 2
        // right = 0.1/ 1 (0.55) = 3
        if (yAxis != 0)
        {
            if (yAxis > 0)
            {
                dir = 1;
                shield.transform.localPosition = new Vector3(0, 0.5f, 0);
                shield.transform.localScale = new Vector3(1, 0.2f, 1);
            }
            else
            {
                shield.transform.localPosition = new Vector3(0, -0.5f, 0);
                shield.transform.localScale = new Vector3(1, 0.2f, 1);
                dir = 0;
            }
        }
        else if (xAxis != 0)
        {
            if (xAxis > 0)
            {
                shield.transform.localPosition = new Vector3(0.5f, 0f, 0);
                shield.transform.localScale = new Vector3(0.2f, 1, 1);
                dir = 3;
            }
            else
            {
                shield.transform.localPosition = new Vector3(-0.5f, 0f, 0);
                shield.transform.localScale = new Vector3(0.2f, 1, 1);
                dir = 2;
            }
        }
        if (usedDir.Contains(dir))
        {
            shieldrend.color = Color.gray;
        }
        else
        {
            shieldrend.color = defaultShieldColor;
        }
        shield.SetActive(true);
        if (!usedDir.Contains(dir))
        {
            if (yAxis != 0)
            {
                if (Physics2D.BoxCast(transform.position, shieldLocation, 0, transform.up, shieldDistance * yAxis, shieldInteractable))
                {
                    if (yAxis > 0)
                    {
                        usedDir.Add(1);
                    }
                    else
                    {
                        usedDir.Add(0);
                    }
                    yVelocity *= -1;
                    jumpStopBuffer = false;
                }
            }
            else if (xAxis != 0)
            {
                if (Physics2D.BoxCast(transform.position, verticalShieldLocation, 0, transform.right, verticalShieldDistance * xAxis, shieldInteractable))
                {
                    if (xAxis > 0)
                    {
                        usedDir.Add(3);
                    }
                    else
                    {
                        usedDir.Add(2);
                    }
                    directionFlipTimer = 0.25f;
                    direction = xAxis * -1;
                    speed += Mathf.Abs(yVelocity);
                    yVelocity = yVelocity / 2;
                    jumpStopBuffer = false;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("spike"))
        {
            inSpike = true;
        }
        else if (collision.transform.CompareTag("Checkpoint"))
        {
            collision.transform.GetComponent<CheckpointScript>().SelectCheckpoint();
        }
        else if (collision.transform.CompareTag("room"))
        {
            gameManager.changeCamera(collision.transform.GetComponent<CameraScript>(), respawning);
        }
        else if (collision.transform.CompareTag("Lava"))
        {
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Replenisher"))
        {
            if (usedDir.Count > 0)
            {
                ReplenisherScript hitReplenisher = collision.gameObject.GetComponent<ReplenisherScript>();
                if (!hitReplenisher.isHit)
                {
                    usedDir.Clear();
                    hitReplenisher.StartCooldown();
                }
            }
        }
        else if (collision.transform.CompareTag("HexaPiece"))
        {
            HexaPiece grabbedPiece = collision.gameObject.GetComponent<HexaPiece>();
            grabbedPiece.SelectTarget(this.gameObject);
        }
        else if (collision.transform.CompareTag("SecretRoom"))
        {
            StartCoroutine(collision.gameObject.GetComponent<SecretRooms>().revealRoom());
        }
        else if (collision.transform.CompareTag("Finish"))
        {
            gameManager.winGame();
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
        else if (collision.transform.CompareTag("HTransition"))
        {
            if (speed < 4f)
            {
                speed = 4f;
            }
            direction = xAxis;
            transitioning = true;
        }
        else if (collision.transform.CompareTag("Temporary"))
        {
            collision.gameObject.GetComponent<TempPlatform>().SteppedOn();
        }


    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("spike"))
        {
            inSpike = false;
        }
        if (collision.transform.CompareTag("Spring"))
        {
            inSpring = false;
        }
        else if (collision.transform.CompareTag("HTransition"))
        {
            transitioning = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("spike"))
        {
            inSpike = true;
        }
        if (collision.transform.CompareTag("Spring"))
        {
            inSpring = true;
            unableToJumpStop = true;
            yVelocity = 12f;
        }
        else if (collision.transform.CompareTag("Transition"))
        {
            unableToJumpStop = true;
            yVelocity = 5.5f;
        }
        else if (collision.transform.CompareTag("HTransition"))
        {
            transitioning = true;
        }
        else if (collision.transform.CompareTag("Temporary"))
        {
            collision.gameObject.GetComponent<TempPlatform>().SteppedOn();
            onSolidGround = false;
        }
    }
}
