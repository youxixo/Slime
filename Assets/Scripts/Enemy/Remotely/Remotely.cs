using UnityEngine;

public class Remotely : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public GameObject bulletPrefab; // 子弹预制体
    public Transform firePoint;     // 子弹发射点
    public float MoveSpeed = 1f;    // 移动速度
    public float fireRate = 1f;     // 子弹生成速率

    private float LeftPointX;
    private float RightPointX;
    private float Speed;
    private Rigidbody2D rb;
    public bool isAttack = false;

    private bool isleft = false;    // 面朝向
    private float nextFireTime = 0f; // 下一次生成子弹的时间

    void Start()
    {
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
        if (!isleft)
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, 0); // 面朝右
            rb.MovePosition(rb.position + new Vector2(MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x >= RightPointX)
            {
                isleft = true;
            }
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0, 180, 0); // 面朝左
            rb.MovePosition(rb.position + new Vector2(-MoveSpeed * Time.deltaTime, 0));
            if (rb.position.x <= LeftPointX)
            {
                isleft = false;
            }
        }
    }

    void Attack()
    {
        Speed = 0; // 停止移动
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FireBullet()
    {
        // 生成子弹
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
