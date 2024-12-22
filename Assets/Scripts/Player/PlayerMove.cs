﻿using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

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
/// 
/// 12/4
/// 90度登強跳會相反
/// </summary>

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float movementSpeedBase = 5; //移动速度
    [SerializeField] float jumpForce = 5; //跳跃强度
    private float originalGravityScale; //存正常的gravity力

    public static Rigidbody2D rb;
    //Vector2 movementDirection = new Vector2(); //移动方向

    private bool jumpClicked = false; //按下跳跃
    private bool allowToMove = true; //能否移动
    private bool isGrounded = false; //检查是否在地上
    private Vector2 velocity; //角色目前移动速度&方向

    [Header("贴墙移动")]
    [SerializeField] private float stickPower; //吸牆力
    [SerializeField] private float maxStickPower = 100; //max stick power
    [SerializeField] private float stickDownPower = 0;
    [SerializeField] private float stickRestorePower;
    private Vector2 surfaceNormal; //角色目前所在地板的法线（用于贴墙移动）
    float r;
    private bool collisionEnter; //collide with new object?
    [SerializeField] LayerMask groundLayer; //地板的layerMask
    [SerializeField] float raycastDistance = 100f; //檢測地面的raycast長度
    [SerializeField] Transform wallDetect; //檢測地面的raycast位置
    private float maxVelocity = 40;
    Vector2 movementAxis = new Vector2();
    private bool releaseMove = true; //是否鬆開移動鍵
    private float angleWhenMove = float.NaN; //開始移動的角度

    [Header("掉落到計時")]
    [SerializeField] private float dropCD = 3; //掉落時間
    [SerializeField] private float dropCountDown; //掉落倒數

    [Header("吸回牆")]
    public float stickSpeed = 3f; // Sitck back to wall speed
    public float minDistance = 0.05f; // Stopping threshold for stick back
    private Vector2 stickTargetPosition; //吸牆目標地點
    private bool isMovingToStickTarget = false; //is sticking to wall

    [Header("跳躍")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float coyoteTimeCountDown;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpBufferCountDown = 0.2f;


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
    //private bool jumpPressed;

    //以後移到其他地方 如player stats來保持腳本整潔
    [Header("StickPowerUI")]
    [SerializeField] private Image stickPowerBar;

    [Header("Events")]
    public static UnityEvent pauseGame = new UnityEvent();

    [Header("Animation")]
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField]private Animator player_animator;

    void Start()
    {
        stickDownPower = 0;
        InitInput();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable(); // Disable the entire InputActionAsset
        }
    }


    void FixedUpdate()
    {
        if (stickPower < 0)
        {
            rb.gravityScale = originalGravityScale;
        }
        if (!isDashing && allowToMove)
        {
            Move();
        }
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
        DetectNotOnGround();
    }

    private void Update()
    {
        if (dashAction.IsPressed() && canDash)
        {
            //StartCoroutine(DashA());
        }
        if (jumpBufferCountDown > 0 && coyoteTimeCountDown > 0)
        {
            jumpBufferCountDown = 0;
            coyoteTimeCountDown = 0;
            Jump();
        }
        if (pauseAction.IsPressed())
        {
            pauseGame.Invoke();
        }
        JumpBufferCount();
        CoyoteTimeCount();
        CountDownDrop();
        StickPowerUIUpdate();
        JumpAnimation();
    }

    private void InitInput()
    {
        var playerActionMap = inputActions.FindActionMap("Player");

        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        pauseAction = playerActionMap.FindAction("Pause");
        dashAction = playerActionMap.FindAction("Sprint");
    }

    private void StickPowerUIUpdate()
    {
        if (stickPowerBar != null)
        {
            stickPowerBar.fillAmount = stickPower / maxStickPower;
        }
    }

    //再加上對initial horizontal input 的判斷? 假設如果一開始在 0 度角按下左鍵 (正向運動的反向移動) 當碰到270度角 ()
    //按下a/d之後檢測所在地型角度 如果不松 變換角度將依照按下時的角度判斷前後
    private Vector2 DetermineMovementAxis(float angle)
    {
        int moveType;
        Vector2 _movementAxis = new Vector2();
        angle = Mathf.RoundToInt(angle);
        if ((angle >= 0 && angle <= 90) || angle > 270)
        {
            _movementAxis = new Vector2(surfaceNormal.y, -surfaceNormal.x);
            moveType = 0; //正向運動
        }
        else if ((angle > 90 && angle <= 270))
        {
            _movementAxis = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            moveType = 1; //反向運動
        }
        else
        {
            Debug.LogWarning("Error: " + angle);
            moveType = -1;
        }
        //***kono sekai no bye bye bye bye
        if (jumpClicked)
        {
            jumpClicked = false;
        }
        else
        {
            _movementAxis = ConsistentMovement(_movementAxis, moveType);
        }
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
        //_movementAxis = ConsistentMovement(_movementAxis, moveType);
        return _movementAxis;
    }

    //move type目前movement
    private Vector2 ConsistentMovement(Vector2 _movementAxis, int moveType)
    {
        if (angleWhenMove != float.NaN || !releaseMove)
        {
            if (((angleWhenMove >= 0 && angleWhenMove <= 90) || angleWhenMove > 270) && moveType == 1)
            {
                _movementAxis = -_movementAxis;
            }
            else if ((angleWhenMove > 90 && angleWhenMove <= 270) && moveType == 0)
            {
                _movementAxis = -_movementAxis;
            }
        }
        //Debug.Log(_movementAxis);
        return _movementAxis;
    }


    //change direction facing base on standing angle and input
    private void ChangeFaceDir(float angle, Vector2 velocity)
    {
        if ((angle >= 0 && angle < 90) || angle > 270)
        {
            if(velocity.x > 0)
            {
                //true
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if(velocity.x < 0)
            {
                //false
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
        else if ((angle > 90 && angle < 270))
        {
            if (velocity.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if (velocity.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
        else if(angle == 90)
        {
            if(velocity.y > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if(velocity.y < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
        else if (angle == 270)
        {
            if (velocity.y > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else if (velocity.y < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
    }

    private void DetectNotOnGround()
    {
        var hit = Physics2D.Raycast(wallDetect.position, -transform.up.normalized, raycastDistance, groundLayer);
        if (hit.collider == null)
        {
            rb.gravityScale = originalGravityScale;
            isGrounded = false;
        }
    }

    private void TranslateToPos()
    {
        //用raycast得到法線 計算移動方向
        Vector2 rayDirection = -transform.up * raycastDistance;
        //***change later too long
        Debug.Log(transform.localRotation.eulerAngles);
        var hit = Physics2D.Raycast(wallDetect.position, rayDirection.normalized, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            surfaceNormal = hit.normal;
            isGrounded = true;
            if (contactingObj != hit.collider.gameObject)
            {
                stickTargetPosition = hit.point;
                isMovingToStickTarget = true;
                if (isMovingToStickTarget)
                {
                    Vector2 currentPosition = transform.position;
                    Vector2 direction = (stickTargetPosition - currentPosition).normalized; // Direction to target
                    float distance = Vector2.Distance(currentPosition, stickTargetPosition); // Distance to target
                    float step = stickSpeed * Time.deltaTime;

                    if (distance <= minDistance)
                    {
                        isMovingToStickTarget = false;
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
            Debug.Log("xi fu shi bai");
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
            player_animator.SetBool("Jump", false);
            player_animator.SetBool("Fall", false);
            HandleRotation(angle, convertedAngleZ);

            if (stickPower > 0)
            {
                //just start moving
                if (horizontalInput != 0 && releaseMove == true && (convertedAngleZ != 90 && convertedAngleZ != 270))
                {
                    releaseMove = false;
                    angleWhenMove = ConvertTo360Base(transform.localEulerAngles.z);
                    player_animator.SetBool("Move", true);
                }
                else if (horizontalInput == 0 && releaseMove == false)
                {
                    player_animator.SetBool("Move", false);
                    releaseMove = true;
                    angleWhenMove = float.NaN;
                }
                convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
                if (ConvertTo360Base(transform.localEulerAngles.z) >= 65 && ConvertTo360Base(transform.localEulerAngles.z) <= 295)
                {
                    stickPower -= stickDownPower * Time.deltaTime;
                }
            }
            else
            {
                rb.gravityScale = originalGravityScale;
                movementAxis = new Vector2(1, 0);
            }

            if (!(ConvertTo360Base(transform.localEulerAngles.z) >= 65 && ConvertTo360Base(transform.localEulerAngles.z) <= 295) && stickPower <= maxStickPower)
            {
                stickPower += stickRestorePower * Time.deltaTime;
            }

            if ((Mathf.Abs(convertedAngleZ - 90) <= 0.001f || Mathf.Abs(convertedAngleZ - 270) <= 0.001f) == false)
            {
                movementAxis = DetermineMovementAxis(Mathf.RoundToInt(convertedAngleZ)); //決定移動方向
                if(horizontalInput != 0)
                {
                    dropCountDown = dropCD;
                }
                velocity = horizontalInput * movementSpeedBase * movementAxis;
            }
            if ((((convertedAngleZ > 265) && (convertedAngleZ < 275)) || ((convertedAngleZ > 85) && (convertedAngleZ < 95))) && velocity == Vector2.zero)
            {
                movementAxis = DetermineMovementAxisVertical(Mathf.RoundToInt(convertedAngleZ)); //決定移動方向
                velocity = verticalInput * movementSpeedBase * movementAxis;
                if(verticalInput != 0)
                {
                    dropCountDown = dropCD;
                    player_animator.SetBool("Move", true);
                    angleWhenMove = ConvertTo360Base(transform.localEulerAngles.z);
                }
                else
                {
                    player_animator.SetBool("Move", false);
                }
            }
            //Debug.Log(velocity);
            ChangeFaceDir(convertedAngleZ, velocity);
            rb.linearVelocity = new Vector2(velocity.x, velocity.y);
        }
        else
        {
            // 在空中也可以控制方向
            if (horizontalInput != 0)
            {
                rb.linearVelocity = new Vector2(horizontalInput * movementSpeedBase, rb.linearVelocity.y);
                ChangeFaceDir(convertedAngleZ, new Vector2(horizontalInput, 0));
            }
            player_animator.SetBool("Move", false);
        }

        if (collisionEnter)
        {
            collisionEnter = false;
        }
    }

    private void CountDownDrop()
    {
        if(dropCountDown > 0)
        {
            dropCountDown -= Time.deltaTime;
        }

        if(isGrounded && dropCountDown < 0)
        {
            rb.gravityScale = originalGravityScale;
        }
    }

    private void JumpAnimation()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0)
            {
                player_animator.SetBool("Jump", false);
                player_animator.SetBool("Fall", true);
            }
            else if (rb.linearVelocity.y > 0)
            {
                player_animator.SetBool("Jump", true);
                player_animator.SetBool("Fall", false);
            }
        }
        else
        {
            player_animator.SetBool("Jump", false);
            player_animator.SetBool("Fall", false);
        }
    }

    private void HandleRotation(float angle, float convertedAngleZ)
    {
        if (Mathf.Abs(convertedAngleZ - ConvertTo360Base(angle - 90f)) < 60)
        {
            // 當角度差較小時，使用平滑旋轉
            float rotateAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, angle - 90f, ref r, 0.01f);
            transform.localRotation = Quaternion.Euler(0, 0, rotateAngle);
        }
        else
        {
            // 當角度差較大時，直接調整角度
            transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    private void CoyoteTimeCount()
    {
        if(isGrounded && !jumpClicked)
        {
            coyoteTimeCountDown = coyoteTime;
        }
        else
        {
            coyoteTimeCountDown -= Time.deltaTime;
        }
    }

    private void JumpBufferCount()
    {
        if(jumpAction.triggered)
        {
            jumpBufferCountDown = jumpBufferTime;
        }
        else
        {
            jumpBufferCountDown -= Time.deltaTime;
        }
    }


    // 跳跃 - 根據當前角度朝不同方向跳
    // 跳跃 - 根據當前角度朝不同方向跳
    private void Jump()
    {
        rb.gravityScale = originalGravityScale;
        contactingObj = null;
        jumpClicked = true;
        isGrounded = false;


        float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
        Vector2 jumpDirection = Vector2.zero;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        //angleWhenMove = float.NaN;
        releaseMove = true;

        // 根據角度設置跳躍方向
        if (Mathf.Abs(convertedAngleZ - 270) < 0.2f)
        {
            jumpDirection = new Vector2(1, 1.8f); // 右上方跳
        }
        else if (convertedAngleZ >= 270)
        {
            jumpDirection = new Vector2(0, 1); // 垂直向上跳
        }
        else if (Mathf.Abs(convertedAngleZ - 90) < 0.2f)
        {
            jumpDirection = new Vector2(-1, 1.8f); // 左上方跳
        }
        else if (convertedAngleZ >= 250 && convertedAngleZ < 270)
        {
            jumpDirection = new Vector2(1, 1); // 右上方跳
        }
        else if (convertedAngleZ >= 90)
        {
            jumpDirection = new Vector2(0, -0.25f); // 垂直向下跳
        }
        else if (convertedAngleZ >= 0)
        {
            jumpDirection = new Vector2(0, 1); // 垂直向上跳
        }
        jumpDirection = new Vector2 (jumpDirection.x + moveAction.ReadValue<Vector2>().x * 0.3f, jumpDirection.y);
        jumpDirection.Normalize();
        // 應用跳躍方向和力度
        rb.AddForce(jumpDirection.normalized * jumpForce, ForceMode2D.Impulse);

        // 冷卻跳躍輸入，避免連續觸發
        StartCoroutine(freezeMovement());
    }


    public static void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
    }

    //*需優化
    //冲刺-朝着朝向
    private IEnumerator DashA()
    {
        contactingObj = null;
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(Mathf.RoundToInt(Input.GetAxis("Horizontal")) * dashForce, Mathf.RoundToInt(Input.GetAxis("Vertical")) * dashForce * 0.5f);
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
            bool surfaceSet = false;
            isGrounded = true;
            collisionEnter = true;
            
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;
                float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
                if(angleWhenMove != (ConvertTo360Base(angle - 90f)))
                {
                    Debug.Log("Surface Normal: " + normal + ", tangent Angle: " + (ConvertTo360Base(angle - 90f) + "angle when move: " + angleWhenMove));
                    surfaceNormal = contact.normal; // Get the surface normal
                    surfaceSet = true;
                    break;
                }
                else if(angleWhenMove == float.NaN)
                {
                    surfaceNormal = contact.normal;
                    surfaceSet = true;
                    break;
                }
            }
            if(jumpClicked && !surfaceSet)
            {
                surfaceNormal = collision.contacts[0].normal;
                Debug.Log("?");
            }

            jumpClicked = false;
            float groundAngle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
            float convertedAngleZ = ConvertTo360Base(transform.localEulerAngles.z);
            HandleRotation(groundAngle, convertedAngleZ);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            HandleCollision(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (stickPower > 0)
            {
                rb.gravityScale = 0;
            }
            foreach (ContactPoint2D contact in collision.contacts)
            {
                //surfaceNormal = contact.normal; // Get the surface normal
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
            else if (stickPower > 0)
            {
                TranslateToPos();
                DetectNotOnGround();
            }

            if(!isGrounded)
            {
                Debug.Log("自然掉落");
                angleWhenMove = float.NaN;
                releaseMove = true;
            }
        }
    }

    private Npc npc;
    public void TalkToNpc(InputAction.CallbackContext context)
    {
        if (inNpc && context.started)
        {
            GameManager.ActivateActionMap("Npc");
            npc.StartDialogue();
        }
    }

    private bool inNpc = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //interactable layer
        if (collision.gameObject.layer == 11)
        {
            collision.gameObject.transform.Find("UI").gameObject.SetActive(true);
            //可以創一個string var 在playerkeybind那邊在設置時再修改這邊的string才不用每次調用
            collision.gameObject.transform.Find("UI/KeyHint/Key Text").GetComponent<TMP_Text>().text = inputActions.FindActionMap("Player").FindAction("Interact").GetBindingDisplayString(0);
        }
        if(collision.CompareTag("Npc"))
        {
            inNpc =true;
            collision.TryGetComponent<Npc>(out npc);
            if(npc == null)
            {
                Debug.LogWarning("no npc script");
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //interactable layer
        if (collision.gameObject.layer == 11)
        {
            collision.gameObject.transform.Find("UI").gameObject.SetActive(false);
        }
        if (collision.CompareTag("Npc"))
        {
            inNpc = false;
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

