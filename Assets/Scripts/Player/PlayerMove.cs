using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;
using Unity.VisualScripting;

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
/// 
/// 11/30
/// 加入了吸牆力 - 當在不符合重力的角度時會消耗吸牆力 消耗完掉落, 在平地回復
/// 在垂直牆時只有ws移動 ad無法移動
/// 跳躍變得更加合理
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
    [SerializeField] private float stickPower; //吸牆力
    [SerializeField] private float stickDownPower;
    [SerializeField] private float stickRestorePower;
    private Vector2 surfaceNormal; //角色目前所在地板的法线（用于贴墙移动）
    float r;
    [SerializeField] LayerMask groundLayer; //地板的layerMask
    [SerializeField] float raycastDistance = 100f; //檢測地面的raycast長度
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
    private bool actionsDisabled = false;



    void Start()
    {
        InitInput();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable(); // Disable the entire InputActionAsset
        }
    }

    void Update()
    {
        if(stickPower < 0)
        {
            rb.gravityScale = originalGravityScale;
        }
        Jump();
        if (!isDashing && allowToMove)
        {
            Move();
        }
        if (dashAction.IsPressed() && canDash)
        {
            //StartCoroutine(DashA());
        }

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
        _movementAxis = ConsistentMovement(_movementAxis, moveType);
        return _movementAxis;
    }

    private Vector2 DetermineMovementAxisVertical(float angle)
    {
        int moveType;
        Vector2 _movementAxis = new Vector2();
        if ((angle >= 85 && angle <= 95))
        {
            _movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            moveType = 0;
        }
        else if ((angle >= 265 && angle <= 275))
        {
            _movementAxis = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            moveType = 1;
        }
        else
        {
            Debug.LogWarning("Error: " + angle);
            moveType = -1;
        }
        _movementAxis = ConsistentMovement(_movementAxis, moveType);
        return _movementAxis;
    }

    private Vector2 ConsistentMovement(Vector2 _movementAxis, int moveType)
    {
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

    public float speed = 20f; // Movement speed
    public float minDistance = 0.01f; // Stopping threshold

    private Vector2 targetPosition;
    private bool isMoving = false;

    private void TranslateToPos()
    {
        //用raycast得到法線 計算移動方向
        Vector2 rayDirection = -transform.up * raycastDistance;
        //***change later too long
        var hit = Physics2D.Raycast(wallDetect.position, rayDirection.normalized, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            surfaceNormal = hit.normal;
            isGrounded = true;
            if (contactingObj != hit.collider.gameObject)
            {
                targetPosition = hit.point;
                isMoving = true;
                if (isMoving)
                {
                    Vector2 currentPosition = transform.position;
                    Vector2 direction = (targetPosition - currentPosition).normalized; // Direction to target
                    float distance = Vector2.Distance(currentPosition, targetPosition); // Distance to target
                    float step = speed * Time.deltaTime;

                    if (distance <= minDistance)
                    {
                        isMoving = false;
                        return;
                    }
                    rb.transform.Translate(direction * step, Space.World);
                    //rb.MovePosition(hit.point);
                }
            }
            contactingObj = hit.collider.gameObject;
        }
        else
        {
            rb.gravityScale = originalGravityScale;
        }
    }

    private GameObject contactingObj;
    //移动
    private void Move()
    {
        velocity = Vector2.zero;
        float horizontalInput = moveAction.ReadValue<Vector2>().x;
        float verticalInput = moveAction.ReadValue<Vector2>().y;

        // 根據地面朝向決定移動方向
        float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
        float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);

        if (isGrounded)
        {
            HandleRotation(angle, convertedAngleZ);

            if (stickPower > 0)
            {
                //just start moving
                if (horizontalInput != 0  && releaseMove == true && (convertedAngleZ != 90 && convertedAngleZ != 270))
                {
                    releaseMove = false;
                    angleWhenMove = ConvertTo360Base(transform.localEulerAngles.z);
                }
                else if (horizontalInput == 0 && releaseMove == false)
                {
                    releaseMove = true;
                    angleWhenMove = float.NaN;
                }
                convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
                if(ConvertTo360Base(transform.localEulerAngles.z) >= 65 && ConvertTo360Base(transform.localEulerAngles.z) <= 295)
                {
                    stickPower -= stickDownPower * Time.deltaTime;
                }
            }
            else
            {
                rb.gravityScale = originalGravityScale;
                movementAxis = new Vector2(1, 0);
            }

            if (!(ConvertTo360Base(transform.localEulerAngles.z) >= 65 && ConvertTo360Base(transform.localEulerAngles.z) <= 295) && stickPower <= 100)
            {
                stickPower += stickRestorePower * Time.deltaTime;
            }

            ChangeFaceDir(convertedAngleZ, horizontalInput); //改改

            if ((convertedAngleZ != 90 && convertedAngleZ != 270) || (horizontalInput!=0 && releaseMove != true))
            {
                movementAxis = DetermineMovementAxis(convertedAngleZ); //決定移動方向
                velocity = horizontalInput * movementSpeedBase * movementAxis;
            }
            else if ((((convertedAngleZ > 265) && (convertedAngleZ < 275)) || ((convertedAngleZ > 85) && (convertedAngleZ < 95))) && velocity == Vector2.zero)
            {
                movementAxis = DetermineMovementAxisVertical(convertedAngleZ); //決定移動方向
                velocity = verticalInput * movementSpeedBase * movementAxis;
            }

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

    private void HandleRotation(float angle, float convertedAngleZ)
    {
        if (Mathf.Abs(convertedAngleZ - ConvertTo360Base(angle - 90f)) < 60)
        {
            // 當角度差較小時，使用平滑旋轉
            float rotateAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, angle - 90f, ref r, 0.1f);
            transform.localRotation = Quaternion.Euler(0, 0, rotateAngle);
        }
        else
        {
            // 當角度差較大時，直接調整角度
            transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    // 跳跃 - 根據當前角度朝不同方向跳
    private void Jump()
    {
        if (jumpAction.triggered && isGrounded)
        {
            rb.gravityScale = originalGravityScale;
            contactingObj = null;
            float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
            jumpClicked = true;
            isGrounded = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            angleWhenMove = float.NaN;
            releaseMove = true;

            // 重置水平速度，防止跳得過遠
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            // 根據角度設置跳躍方向
            Vector2 jumpDirection = Vector2.zero;

            if (Mathf.Abs(convertedAngleZ - 270) < 0.2f)
            {
                jumpDirection = new Vector2(1, 1.5f); // 右上方跳
            }
            else if (convertedAngleZ >= 270)
            {
                jumpDirection = new Vector2(0, 1); // 垂直向上跳
            }
            else if (Mathf.Abs(convertedAngleZ - 90) < 0.2f)
            {
                jumpDirection = new Vector2(-1, 1.5f); // 左上方跳
            }
            else if (convertedAngleZ >= 90)
            {
                jumpDirection = new Vector2(0, -0.5f); // 垂直向下跳
            }
            else if (convertedAngleZ >= 0)
            {
                jumpDirection = new Vector2(0, 1); // 垂直向上跳
            }

            // 應用跳躍方向和力度
            rb.linearVelocity += jumpDirection.normalized * jumpForce;

            // 冷凍短暫的移動行為以避免連續輸入
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
            isGrounded = true;
            collisionEnter = true;
            jumpClicked = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                surfaceNormal = contact.normal; // Get the surface normal
            }
            if(stickPower > 0)
            {
                //rb.gravityScale = 0;
            }
        }
    }

    private bool collisionEnter;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            HandleCollision(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            if (stickPower > 0)
            {
                rb.gravityScale = 0;
                
            }
            foreach (ContactPoint2D contact in collision.contacts)
            {
                surfaceNormal = contact.normal; // Get the surface normal
            }
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isGrounded = false;
            contactingObj = null;
            //由用戶控制導致地離開地板 不吸回地面
            if (jumpClicked || isDashing)
            {
                rb.gravityScale = originalGravityScale;
                return;
            }
            else if(stickPower > 0)
            {
                TranslateToPos();
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

