using UnityEngine;
using System.Collections;
using Unity.Netcode;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;

public class PlayerMovementCA : NetworkBehaviour
{
    [SerializeField] float movementSpeedBase = 5; //移动速度
    [SerializeField] float jumpForce = 5; //跳跃强度
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePos;
    private float originalGravityScale; //存正常的gravity力

    private Rigidbody2D rb;
    public int playerScore;
    Vector2 movementDirection = new Vector2();

    private bool jumpClicked = false; //按下跳跃
    private bool allowToMove = true; //能否移动
    private bool isGrounded = false; //检查是否在地上
    private Vector2 velocity; //角色目前移动速度&方向
    private Vector2 surfaceNormal;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastDistance = 0.5f;
    [SerializeField] GameObject contactPoint;

    [SerializeField] Transform leftDetect;
    [SerializeField] Transform rightDetect;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashForce = 25;
    [SerializeField] private float dashDuration = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        Vector2 rayDirection = -transform.up * raycastDistance;

        Debug.DrawRay(leftDetect.position, -transform.up * raycastDistance, Color.red);
        Debug.DrawRay(rightDetect.position, -transform.up * raycastDistance, Color.yellow);
        //Debug.Log(rb.linearVelocity);
        if (!isDashing && allowToMove)
        {
            Move();
        }
        Attack();
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashA());
        }



        if (isGrounded && rb.gravityScale == 0)
        {
            Vector2 gravityDirection = -transform.up;
            float gravitySpeed = 50f;
            rb.AddForce(gravityDirection * gravitySpeed, ForceMode2D.Force);
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // 根據地面朝向決定移動方向
        Vector2 movementAxis = new Vector2();
        float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
        if (isGrounded)
        {
            if (convertedAngleZ == 270)
            {
                movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            }
            else if(convertedAngleZ == 90)
            {
                movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            }
            else if(convertedAngleZ == 180)
            {
                movementAxis = new Vector2(-surfaceNormal.y, -surfaceNormal.x);
            }
            else if (convertedAngleZ > 270)
            {
                movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            }
            else if (convertedAngleZ > 180)
            {
                movementAxis = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            }
            else if (convertedAngleZ > 90)
            {
                movementAxis = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            }
            else if(convertedAngleZ >= 0)
            {
                movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            }

            velocity = movementAxis * movementSpeedBase * horizontalInput;
            rb.linearVelocity = new Vector2(velocity.x, velocity.y);
        }
        else
        {
            // Apply horizontal movement in the air
            if (horizontalInput != 0)
            {
                rb.linearVelocity = new Vector2(horizontalInput * movementSpeedBase, rb.linearVelocity.y);
            }
        }

        // Flip the character's sprite based on movement direction
        if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Flip left
        }
        else if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Flip right
        }

        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            jumpClicked = true;
            isGrounded = false;
            if(convertedAngleZ == 270 )
            {
                rb.linearVelocity = new Vector2(jumpForce, jumpForce);
            }
            else if(convertedAngleZ >= 270)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            else if(convertedAngleZ == 90)
            {
                rb.linearVelocity = new Vector2(-jumpForce, jumpForce);
            }
            else if (convertedAngleZ >= 180)
            {
                rb.linearVelocity = new Vector2 (rb.linearVelocityX, -jumpForce);
            }
            else if(convertedAngleZ >= 90)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -jumpForce);
            }
            else if(convertedAngleZ >= 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            StartCoroutine(freezeMovement());
            isGrounded = false; 
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnBulletServerRpc(transform.localScale.x);
        }
    }
    
    //改一下 現在離鼠標越遠跑越長
    private IEnumerator DashA()
    {
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerToMouseVector = (mousePosition - transform.position).normalized;
        playerToMouseVector.z = 0;
        rb.linearVelocity = playerToMouseVector * dashForce;
        //Debug.Log(rb.linearVelocity);

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravityScale;
        isDashing = false;

        //cooldown / groundcheck
        canDash = true;
    }

    private IEnumerator freezeMovement()
    {
        allowToMove = false;
        yield return new WaitForSeconds(0.1f);
        allowToMove = true;
    }
    
    [ServerRpc]
    private void SpawnBulletServerRpc(float direction)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, firePos.position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().direction = direction;
        bulletInstance.GetComponent<NetworkObject>().Spawn();
    }

    //落地
    private void HandleCollision(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            surfaceNormal = contact.normal; // Get the surface normal
            rb.gravityScale = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;  // Set grounded status to true
        jumpClicked = false;
        HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(jumpClicked)
        {
            isGrounded = false;
            rb.gravityScale = originalGravityScale;
        }
        else
        {
            if(ConvertTo360Base(transform.eulerAngles.z)<=180)
            {
                if(transform.localScale.x == -1)
                {
                    Vector2 rayDirection = -transform.up * raycastDistance;
                    RaycastHit2D hit = Physics2D.Raycast(rightDetect.position, rayDirection.normalized, 1f, groundLayer);
                    if (hit.collider != null)
                    {
                        surfaceNormal = hit.normal;
                    }
                }
                else if (transform.localScale.x == 1)
                {
                    Vector2 rayDirection = -transform.up * raycastDistance;
                    RaycastHit2D hit = Physics2D.Raycast(rightDetect.position, rayDirection.normalized, 1f, groundLayer);
                    if (hit.collider != null)
                    {
                        surfaceNormal = hit.normal;
                    }
                }
            }
            else
            {
                if(transform.localScale.x == 1)
                {
                    Vector2 rayDirection = -transform.up * raycastDistance;
                    RaycastHit2D hit = Physics2D.Raycast(rightDetect.position, rayDirection.normalized, 1f, groundLayer);
                    if (hit.collider != null)
                    {
                        surfaceNormal = hit.normal;
                    }
                }
                else if (transform.localScale.x == -1)
                {
                    Vector2 rayDirection = -transform.up * raycastDistance;
                    RaycastHit2D hit = Physics2D.Raycast(rightDetect.position, rayDirection.normalized, 1f, groundLayer);
                    if (hit.collider != null)
                    {
                        surfaceNormal = hit.normal;
                    }
                }
            }
        }
    }

    private IEnumerator AppliedGravity()
    {
        Debug.Log("離開");
        yield return new WaitForSeconds(0.5f);

        // Define a gravity velocity based on the character's current rotation
        Vector2 gravityDirection = -transform.up; // Local downward direction
        float gravitySpeed = 5f; // Customize this to control the gravity speed

        // Apply velocity towards the local down direction to simulate gravity
        Debug.Log(gravityDirection);
        rb.linearVelocity += gravityDirection * gravitySpeed ;

        Debug.Log("gravity applied");
    }

    private float ConvertTo360Base(float angle)
    {
        float convertedAngle = angle % 360;
        if (angle < 0)
        {
            convertedAngle = angle + 360;
            angle += 360;
        }
        return angle;
    }

}
