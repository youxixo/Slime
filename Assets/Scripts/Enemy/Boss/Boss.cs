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
    public bool Attacked = false;
    public GameObject StopPoint;
    public GameObject AirAttackPoint;
    public GameObject AirAttack;
    public GameObject ball;
    public GameObject Panding;

    public float LeftPointX;
    public float RightPointX;
    private Rigidbody2D rb;
    private Transform playerTr;
    private Animator ani;
    public int AttackNum = 0;
    public bool isGround = false;
    public bool isJump = false;
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
        Attack();
    }

    void Attack()
    {
        if(AttackNum == 3)
        {
            AttackNum = 0;
            ani.SetBool("空中攻击", true);
        }
        if(isleft)
        {
            if(playerTr.position.x >= Panding.transform.position.x)
            {
                ani.SetBool("walk", true);
            }
            if(playerTr.position.x < Panding.transform.position.x)
            {
               AttackChange();
            }
        }
        if(!isleft)
        {
            if(playerTr.position.x <= Panding.transform.position.x)
            {
                ani.SetBool("walk", true);
            }
            if(playerTr.position.x > Panding.transform.position.x)
            {
                AttackChange();
            }
        }
    }

    void AttackChange()
    {
        int num = Random.Range(1, 3);
        switch (num)
        {
            case 1:
                ani.SetBool("walk", true);
                break;
            case 2:
                ani.SetBool("跳跃攻击", true);
                break;
            case 3:
                AttackNum++;
                ani.SetBool("刀波", true);
                break;
        }
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

    void yuancheng()//远程攻击
    {
        Vector3 createPos = new Vector3(-8.66f, -1.95f, 0);

        Vector3 worldPos = transform.TransformPoint(createPos);
        Instantiate(Bo, worldPos, Quaternion.identity, transform);
    }

    void Jump()//跳跃 动画器调用
    {
        if(ani.GetBool("跳跃攻击"))
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
        if(ani.GetBool("空中攻击"))
        {
           rb.AddForce(Vector2.up * 20, ForceMode2D.Impulse);
        }
    }

    void Fashe()
    {
        GameObject newBall = Instantiate(ball, AirAttack.transform.position, Quaternion.identity);
        newBall.transform.SetParent(transform);
    }
}
