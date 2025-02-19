using DG.Tweening;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f; // Movement speed
    public float JumpTime = 10f; // Jump force
    public GameObject Bo;
    public bool CanAttack = false;
    public bool isleft = true;

    private float LeftPointX;
    private float RightPointX;
    private Rigidbody2D rb;
    private Transform playerTr;
    private Animator ani;
    private bool isGround = false;
    private bool isJump = false;
    void Start()
    {
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();

        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    void Update()
    {
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        Face();
        action();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGround = false;
        }
    }

    void Face()
    {
        if(playerTr.position.x > LeftPointX && playerTr.position.x < RightPointX)//判断玩家是否在范围内,并进行追杀
        {
            if (playerTr.position.x > transform.position.x)
            {
                isleft = false;
            }
            else
            {
                isleft = true;
            }
        }
    }

    void action()
    {
        //JumpAttack();
        // if (CanAttack)
        // {
        //     NormalAttack();
        // }
        // else
        // {
        //     Move();
        // }
    }

    void Move()
    {
        ani.SetBool("普通攻击", false);
        ani.SetBool("walk", true);
        if (!isleft)
        {
            rb.transform.rotation = Quaternion.Euler(0, 180, 0); // 面朝右
            rb.MovePosition(rb.position + new Vector2(MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x >= RightPointX)
            {
                isleft = true;
            }
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, 0); // 面朝左
            rb.MovePosition(rb.position + new Vector2(-MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x <= LeftPointX)
            {
                isleft = false;
            }
        }
    }

    void NormalAttack()//普通攻击
    {
        ani.SetBool("walk", false);
        ani.SetBool("普通攻击", true);
    }

    void JumpAttack()//跳跃攻击
    {
        ani.SetBool("walk", false);
        ani.SetBool("跳跃攻击", true);

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

            isJump = true;
        }

        if(rb.linearVelocity.y < 0 && !isGround)//下落
        {
            ani.SetBool("降落", true);
        }

        if(isGround && isJump)//落地
        {
            isJump = false;
            ani.SetBool("降落", false);
            ani.SetBool("跳跃攻击", false);
        }
    }

    void yuancheng()//远程攻击
    {
        Vector3 createPos = new Vector3(-8.66f, -1.95f, 0);

        Vector3 worldPos = transform.TransformPoint(createPos);
        Instantiate(Bo, worldPos, Quaternion.identity, transform);
    }
}
