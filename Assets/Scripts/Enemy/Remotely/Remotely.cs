using UnityEngine;

public class Remotely : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public GameObject bulletPrefab; // 子弹预制体
    public Transform firePoint;     // 子弹发射点
    public float MoveSpeed = 1f;    // 移动速度

    private float LeftPointX;
    private float RightPointX;
    private Rigidbody2D rb;
    private Animator anim;
    public bool isAttack = false;

    public bool isleft = true;    // 面朝向
    private bool hasAttack = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;

        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    void Update()
    {
        if (isAttack)
        {
            Attack(); // 停止移动并生成子弹
        }
        else
        {
            Move(); // 正常移动
        }
    }

    void Move()
    {
        anim.SetBool("Attack", false); // 停止播放攻击动画
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

    void Attack()
    {
        anim.SetBool("Attack", true); // 播放攻击动画
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsTag("Attacking"))
        {
            anim.SetBool("Attacking", true);
            hasAttack = false;
        }

        if (stateInfo.IsTag("Attacking") && !hasAttack)
        {
            FireBullet();
            anim.SetBool("Attacking", false);
            hasAttack = true;
        }
    }

    void FireBullet()
    {
    // 实例化子弹
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    // 获取子弹脚本并设置 remotely 的引用
    Bullets bulletScript = bullet.GetComponent<Bullets>();
    if (bulletScript != null)
    {
        bulletScript.remotely = this; // 传递当前 Remotely 对象的引用
    }
    }

}
