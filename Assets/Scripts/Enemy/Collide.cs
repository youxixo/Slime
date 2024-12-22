using System.Collections;
using UnityEngine;

public class Collide : MonoBehaviour
{
    public float PlayerTransformX;
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f;//移动速度
    public float AttackSpeed = 2f;//攻击速度

    public float WaitingTime = 5f;
    private float LeftPointX;
    private float RightPointX;
    private float MyTransform;
    private Rigidbody2D rb;
    private float Speed;
    public bool isAttack = false;

    private Animator anim;
    
    public bool isleft = true;//面朝向
    private bool isWaiting = false; //是否在等待

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;

        anim = GetComponent<Animator>();
        
        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerTransformX = GameObject.FindWithTag("Player").GetComponent<Transform>().position.x;
        MyTransform = GetComponent<Transform>().position.x;

        if(!isWaiting)
        {
            move(); 
        }
    }

    void move()
    {
        Attack();

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Attack"))
        {
            Speed = 0;
        }

        if (!isleft)
        {
            rb.transform.rotation = Quaternion.Euler(0, 180, 0);//面朝向右
            
            rb.MovePosition(rb.position + new Vector2(Speed * Time.deltaTime, 0));
            if(rb.position.x >= RightPointX)
            {
                StartCoroutine(WaitBedoreChangeDirection());
            }
           
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, 0);//面朝向左
            rb.MovePosition(rb.position + new Vector2(-Speed * Time.deltaTime, 0));
           if(rb.position.x <= LeftPointX)
           {
               StartCoroutine(WaitBedoreChangeDirection());
           }
        }
    }
    void Attack()
    {
        if(PlayerTransformX >= LeftPointX & PlayerTransformX <= RightPointX)//判断玩家是否在攻击范围内,并进行追杀
        {
           if(MyTransform < PlayerTransformX)
           {
               isleft = false;
           }
           else
           {
               isleft = true;
           }
        }

        if (isAttack)
        {
            anim.SetBool("Attack", true);
            Speed = AttackSpeed;
        }  
        else
        {
            anim.SetBool("Attack", false);
            Speed = MoveSpeed;
        }
    }

    IEnumerator WaitBedoreChangeDirection()
    {
        isWaiting = true;
        Speed = 0;
        anim.SetBool("Waiting", true);
        yield return new WaitForSeconds(WaitingTime);
        isleft = !isleft;
        isWaiting = false;
        anim.SetBool("Waiting", false);
    }
}