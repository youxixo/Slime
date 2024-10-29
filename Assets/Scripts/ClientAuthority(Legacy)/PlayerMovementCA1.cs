using UnityEngine;
using System.Collections;

public class PlayerMovementCA1: MonoBehaviour
{
    [SerializeField] float movementSpeedBase = 5; //移动速度
    [SerializeField] float jumpForce = 5; //跳跃强度
    private float originalGravityScale; //存正常的gravity力

    public Rigidbody2D rb;
    Vector2 movementDirection = new Vector2(); //移动方向

    private bool jumpClicked = false; //按下跳跃
    private bool allowToMove = true; //能否移动
    private bool isGrounded = false; //检查是否在地上
    private Vector2 velocity; //角色目前移动速度&方向

    [Header("贴墙移动")]
    private Vector2 surfaceNormal; //角色目前所在地板的法线（用于贴墙移动）
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastDistance = 0.5f;
    [SerializeField] Transform wallDetect;
    Vector2 movementAxis = new Vector2();
    private bool releaseMove = true;
    private float angleWhenMove;


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


    void Update()
    {
        Vector2 rayDirection = -transform.up * raycastDistance;

        Debug.DrawRay(wallDetect.position, -transform.up * raycastDistance, Color.red);
        //Debug.Log(rb.linearVelocity);
        if (!isDashing && allowToMove)
        {
            Move();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashA());
        }
    }
    //按下a/d之後檢測所在地型角度 如果不松 變換角度將依照按下時的角度判斷前後
    private Vector2 DetermineMovementAxis(float angle)
    {
        int moveType = -1;
        Vector2 _movementAxis = new Vector2();
        if ((angle >= 0 && angle <= 90) || angle >= 270)
        {
            _movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            moveType = 0;
        }
        else if ((angle > 90 && angle < 270))
        {
            _movementAxis = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            moveType = 1;
        }
        else
        {
            Debug.LogWarning("Error: " + angle);
            moveType = -1;
        }


        if (angleWhenMove != float.NaN || !releaseMove)
        {
            if (((angleWhenMove >= 0 && angleWhenMove <= 90) || angleWhenMove >= 270) && moveType != 0)
            {
                //Debug.Log("reversing move");
                _movementAxis = -_movementAxis;
            }
            else if ((angleWhenMove > 90 && angleWhenMove < 270) && moveType != 1)
            {
                _movementAxis = -_movementAxis;
            }
        }
        return _movementAxis;
    }

    //移动
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        // 根據地面朝向決定移動方向
        float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
        float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);

        if (isGrounded)
        {
            transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
            if (horizontalInput != 0 && releaseMove == true)
            {
                releaseMove = false;
                angleWhenMove = ConvertTo360Base(transform.localEulerAngles.z);
            }
            else if (horizontalInput == 0 && releaseMove == false)
            {
                releaseMove = true;
                angleWhenMove = float.NaN;
            }
            movementAxis = DetermineMovementAxis(convertedAngleZ);

            velocity = horizontalInput * movementSpeedBase * movementAxis;
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

        //跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) //Input.GetAxis("Jump") > 0 
        {
            jumpClicked = true;
            isGrounded = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            angleWhenMove = float.NaN;
            if (convertedAngleZ == 270)
            {
                rb.linearVelocity = new Vector2(jumpForce, jumpForce);
            }
            else if (convertedAngleZ >= 270)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            else if (convertedAngleZ == 90)
            {
                rb.linearVelocity = new Vector2(-jumpForce, jumpForce);
            }
            else if (convertedAngleZ >= 180)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -jumpForce);
            }
            else if (convertedAngleZ >= 90)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -jumpForce);
            }
            else if (convertedAngleZ >= 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            StartCoroutine(freezeMovement());
        }
    }


    //冲刺-朝着朝向
    private IEnumerator DashA()
    {
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2 (transform.localScale.x * dashForce, 0);
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

    //落地
    private void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = true;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                surfaceNormal = contact.normal; // Get the surface normal
                rb.gravityScale = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpClicked = false;
        HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //HandleCollision(collision);
    }
    public GameObject debugObject;
    public int gravityForce = 10;
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            RaycastHit2D hit;
            isGrounded = false;
            if (jumpClicked || isDashing)
            {
                rb.gravityScale = originalGravityScale;
                releaseMove = true;
                return;
            }
            else
            {
                Vector2 rayDirection = -transform.up * raycastDistance;
                hit = Physics2D.Raycast(wallDetect.position, rayDirection.normalized, 1f, groundLayer);
                if (hit.collider != null)
                {
                    surfaceNormal = hit.normal;
                    rb.MovePosition(hit.point);
                    Debug.Log("吸住");
                }
                else
                {
                    Debug.Log("調出去");
                    rb.gravityScale = originalGravityScale;
                    releaseMove = true;
                    return;
                }
            }
        }
    }

    //helper换算角度
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
