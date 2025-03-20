using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f; // Movement speed
    public float JumpTime = 10f; // Jump force
    public float 上升力 = 40f;
    public float HP;
    public float currentHp;
    public GameObject HPUI;
    public GameObject Bo;
    public bool CanAttack = false;
    public GameObject StopPoint;
    public GameObject AirAttackPoint;
    public GameObject AirAttack;
    public GameObject ball;
    public GameObject Panding;
    public GameObject AirColl;
    public GameObject Dead;

    public float LeftPointX;
    public float RightPointX;
    private Rigidbody2D rb;
    private Transform playerTr;
    private Animator ani;
    public static int AttackNum = 0;
    public bool isGround = false;
    public bool isJump = false;
    public bool Air= false;
    public static float downYspeed;
    void Start()
    {
        if(Instance != null)
            Instance= null;
        Instance = this;

        HPUI.GetComponent<Slider>().maxValue = HP;
        HPUI.GetComponent<Slider>().value = HP;
        currentHp = HP;


        AttackNum = 0;
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();

        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    void Update()
    {
        isColliding = false;
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if(HPUI.GetComponent<Slider>().value <= 0)
        {
            HPUI.SetActive(false);
            Instantiate(Dead, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        
    }

    void OnCollisionExit2D(Collision2D other)
    {
        
    }

    void yuancheng()//远程攻击
    {
        Vector3 createPos = new Vector3(-8.66f, -1.95f, 0);

        Vector3 worldPos = transform.TransformPoint(createPos);
        Instantiate(Bo, worldPos, Quaternion.identity, transform);
    }

    void Jump()//跳跃 动画器调用
    {
            Vector2 staryPos = rb.position;
            Vector2 tarfetPos = playerTr.position;
            Vector2 dir = tarfetPos - staryPos;

            //计算重力加速度
            float gravity = Mathf.Abs(Physics2D.gravity.y);

            //计算水平初速度
            float vx = dir.x / JumpTime;

            //计算垂直初速度
            float vy = (dir.y + 0.5f * gravity * JumpTime * JumpTime) / JumpTime;

            Vector2 jumpVelocity = new Vector2(vx, vy);

            rb.linearVelocity = jumpVelocity;
    }

    void Fashe()
    {
        GameObject newBall = Instantiate(ball, AirAttack.transform.position, Quaternion.identity);
        newBall.transform.SetParent(transform);
    }

    void 上升()
    {
        rb.AddForce(Vector2.up * 上升力, ForceMode2D.Impulse);
    }

    public bool Isleft()
    {
        if(playerTr.position.x >= LeftPointX && playerTr.position.x <= RightPointX)//判断玩家是否在范围内,并进行追杀
        {
            if (playerTr.position.x > transform.position.x)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false; // Default return value when player is not within range
    }

    public void GetHurt(float damage)
    {
        currentHp = Math.Clamp(currentHp - damage, 0, HP);
        HPUI.GetComponent<Slider>().value = currentHp;

    }
    public bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.tag == "PlayerAttack" )
        //{
        //    if (isColliding)
        //    {
        //        return;
        //    }
        //    //collision.overl  
        //    GetHurt(20);
        //    isColliding = true;
        //}
    }
    public static Boss Instance;
    public void PlayerDead()
    {
        HPUI.SetActive(false);
        currentHp = HP;
        HPUI.GetComponent<Slider>().value = currentHp;
        CameraChanger.Instance.DeactiveChild();
    }
}
