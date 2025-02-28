using System.Collections;
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
    Rigidbody2D rb;
    Color defaultShieldColor;
    float yVelocity = 0;
    float gravity = 10f;
    float jumpBuffer = 0f;
    float velocity = 0;
    bool shielding = false;
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
    ArrayList usedDir = new ArrayList() { };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultShieldColor = shieldrend.color;
        speed = 4f;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {

        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = 0.2f;
        }
        if (Input.GetKeyUp(KeyCode.Space) && (usedDir.Count == 0))
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
    // Update is called once per frame
    private void FixedUpdate()
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
            fallTime += Time.deltaTime;
            if (fallTime <= coyoteTime && !jumped && jumpBuffer > 0)
            {
                jumped = true;
                yVelocity = 5.5f;
                jumpBuffer = 0;
            }
            speed = 6f;
            if (yVelocity > 3 && jumpStopBuffer)
            {
                yVelocity = 0;
                jumpStopBuffer = false;
            }
            if (yVelocity > 0 && HeadHitter())
            {
                yVelocity = 0;
            }
            yVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            jumped = false;
            fallTime = 0;
            jumpStopBuffer = false;
            yVelocity = 0;
            if (jumpBuffer > 0)
            {
                jumped = true;
                yVelocity = 5.5f;
                jumpBuffer = 0;
            }
        }
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
                    velocity += xAxis * 16f * Time.deltaTime;
                    velocity = Mathf.Clamp(velocity, speed * -1, speed);
                }
                else
                {
                    speed = 4f;
                    velocity = xAxis * speed;
                }
            }
            else
            {
                velocity = 0;
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

    public bool IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, ground))
        {
            usedDir.Clear();
            return true;

        }
        else
        {
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
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("spike"))
        {
            inSpike = false;
        }
    }
}
