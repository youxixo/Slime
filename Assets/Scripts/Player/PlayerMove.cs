using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// 此腳本為角色控制
/// 截止到10/22 以下為待加的功能
/// 角色畫面上的轉向 (要根據移動的方向)
/// 衝刺方向(目前只有左右衝刺)
/// 跳躍會被吸回去(已修復, rather then movePosition back to hit point everytime it goes out of collision, use raycast to determine surface nomrmal in update and move along that, only move back when to far or contacting new obj)
/// 
/// 11/5
/// 修復跳躍吸具體看上面
/// 目前出現新bug, 從天上掉到圓弧角的編編時可能會angle有問題 大概是raycast沒檢測好
/// 離開牆體應該加回gravity
/// </summary>

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float movementSpeedBase = 5; //移动速度
    [SerializeField] float jumpForce = 5; //跳跃强度
    private float originalGravityScale; //存正常的gravity力

    public Rigidbody2D rb;
    //Vector2 movementDirection = new Vector2(); //移动方向

    private bool jumpClicked = false; //按下跳跃
    private bool allowToMove = true; //能否移动
    private bool isGrounded = false; //检查是否在地上
    private Vector2 velocity; //角色目前移动速度&方向

    [Header("贴墙移动")]
    private Vector2 surfaceNormal; //角色目前所在地板的法线（用于贴墙移动）
    float r;
    [SerializeField] LayerMask groundLayer; //地板的layerMask
    [SerializeField] float raycastDistance = 0.5f; //檢測地面的raycast長度
    [SerializeField] Transform wallDetect; //檢測地面的raycast位置
    Vector2 movementAxis = new Vector2();
    private bool releaseMove = true; //是否鬆開移動鍵
    private float angleWhenMove; //開始移動的角度

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashForce = 25;
    [SerializeField] private float dashDuration = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction pauseAction;
    private InputAction dashAction;

    [Header("Events")]
    public static UnityEvent pauseGame = new UnityEvent();

    void Start()
    {
        InitInput();

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
        if (dashAction.IsPressed() && canDash)//Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            //StartCoroutine(DashA());
        }
        Jump();

        if(pauseAction.IsPressed())
        {
            pauseGame.Invoke();
        }
    }

    private void InitInput()
    {
        var playerActionMap = inputActions.FindActionMap("Player");

        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        pauseAction = playerActionMap.FindAction("Pause");
        dashAction = playerActionMap.FindAction("Sprint");
    }

    //按下a/d之後檢測所在地型角度 如果不松 變換角度將依照按下時的角度判斷前後
    private Vector2 DetermineMovementAxis(float angle)
    {
        int moveType;
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
                _movementAxis = -_movementAxis;
            }
            else if ((angleWhenMove > 90 && angleWhenMove < 270) && moveType != 1)
            {
                _movementAxis = -_movementAxis;
            }
        }
        return _movementAxis;
    }

    //change direction facing base on standing angle and input
    private void ChangeFaceDir(float angle, float input)
    {
        Debug.Log("changing dir");
        int moveType = -1;
        if ((angle >= 0 && angle <= 90) || angle >= 270)
        {
            moveType = 0;
        }
        else if ((angle > 90 && angle < 270))
        {
            moveType = 1;
        }

        if (moveType == 0)
        {
            if(input > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if(input < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (input > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (input < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private GameObject contactingObj;
    //移动
    private void Move()
    {
        float horizontalInput = moveAction.ReadValue<Vector2>().x;
        if (!collisionEnter) //如果移動過程碰到新表面, 優先沿著新表面走
        {
            //用raycast得到法線 計算移動方向
            Vector2 rayDirection = -transform.up * raycastDistance;
            var hit = Physics2D.Raycast(wallDetect.position, rayDirection.normalized, 0.5f, groundLayer);
            if (hit.collider != null)
            {
                surfaceNormal = hit.normal;
                isGrounded = true;
                if (contactingObj != hit.collider.gameObject)
                {
                    rb.MovePosition(hit.point);
                }
                contactingObj = hit.collider.gameObject;
            }
        }

        // 根據地面朝向決定移動方向
        float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
        float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);

        if (isGrounded)
        {
            //如再出問題改成visual跟rb分開
            //角度小的話緩慢旋轉
            if (Mathf.Abs(convertedAngleZ - ConvertTo360Base((angle - 90f))) < 60)
            {
                //Debug.Log("smol");
                float rotateAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, angle - 90f, ref r, 0.1f);
                transform.localRotation = Quaternion.Euler(0, 0, rotateAngle);
            }
            else //角度大的話直接旋轉
            {
                transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            //just start moving
            if (horizontalInput != 0 && releaseMove == true)
            {
                ChangeFaceDir(convertedAngleZ, horizontalInput);
                releaseMove = false;
                angleWhenMove = ConvertTo360Base(transform.localEulerAngles.z);
            }
            else if (horizontalInput == 0 && releaseMove == false)
            {
                releaseMove = true;
                angleWhenMove = float.NaN;
            }
            movementAxis = DetermineMovementAxis(convertedAngleZ); //決定移動方向
            convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);

            velocity = horizontalInput * movementSpeedBase * movementAxis;

            rb.linearVelocity = new Vector2(velocity.x, velocity.y);
        }
        else
        {
            // 在空中也可以控制方向
            if (horizontalInput != 0)
            {
                ChangeFaceDir(convertedAngleZ, horizontalInput);
                rb.linearVelocity = new Vector2(horizontalInput * movementSpeedBase, rb.linearVelocity.y);
            }
        }

        if(collisionEnter)
        {
            collisionEnter = false;
        }
    }

    //***待調整
    //跳跃 - 根據當前角度朝不同方向跳
    private void Jump()
    {
        if (jumpAction.triggered && isGrounded && jumpClicked == false)// Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            contactingObj = null;
            float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
            jumpClicked = true;
            isGrounded = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            angleWhenMove = float.NaN;

            if (Mathf.Abs(convertedAngleZ - 270) < 0.2f)
            {
                rb.linearVelocity = new Vector2(jumpForce, jumpForce);
            }
            else if (convertedAngleZ >= 270)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            else if (Mathf.Abs(convertedAngleZ - 90) < 0.2f)
            {
                rb.linearVelocity = new Vector2(-jumpForce, jumpForce);
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

    //*需優化
    //冲刺-朝着朝向
    private IEnumerator DashA()
    {
        contactingObj = null;
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2 (Mathf.RoundToInt(Input.GetAxis("Horizontal")) * dashForce, Mathf.RoundToInt(Input.GetAxis("Vertical")) * dashForce*0.5f);
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

    //落地時取得地板的法線
    private void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            collisionEnter = true;
            isGrounded = true;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                surfaceNormal = contact.normal; // Get the surface normal
                rb.gravityScale = 0;
            }
        }
    }

    private bool collisionEnter;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpClicked = false;
        HandleCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            rb.gravityScale = 0;
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            RaycastHit2D hit;
            isGrounded = false;
            contactingObj = null;
            //由用戶控制導致地離開地板 不吸回地面
            if (jumpClicked || isDashing)
            {
                rb.gravityScale = originalGravityScale;
                releaseMove = true;
                return;
            }
            //反之吸回地面
            else
            {
                /*
                Vector2 rayDirection = -transform.up * raycastDistance;
                hit = Physics2D.Raycast(wallDetect.position, rayDirection.normalized, 1f, groundLayer);
                if (hit.collider != null)
                {
                    //Instantiate(debug, hit.point + hit.normal * 0.5f, Quaternion.identity); //這裡之後改移下
                    surfaceNormal = hit.normal;
                    //rb.MovePosition(hit.point + hit.normal * 0.1f);
                    isGrounded = true;
                }
                else
                {
                }*/
                rb.gravityScale = originalGravityScale;
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

