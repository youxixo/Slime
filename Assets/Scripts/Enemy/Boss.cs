using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f; // Movement speed
    public GameObject DeadEffect;

    private float LeftPointX;
    private float RightPointX;
    private Rigidbody2D rb;
    private bool isleft = true;
    private Transform player;
    void Start()
    {
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;
        rb = GetComponent<Rigidbody2D>();

        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    void FixedUpdate()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if (player.position.x > transform.position.x)
        {
            isleft = false;
        }
        else
        {
            isleft = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
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
}
