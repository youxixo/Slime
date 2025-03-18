using UnityEngine;

public class Bo : MonoBehaviour
{
    Boss boss;
    public float speed;

    private bool isleft;
    private Animator AniBoss;
    private float RightPointX;
    private float LeftPointX;
    void Awake()
    {
    }

    void Start()
    {
        AniBoss = GameObject.Find("Boss").GetComponent<Animator>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        RightPointX = boss.RightPointX;
        LeftPointX = boss.LeftPointX;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= RightPointX ||
        transform.position.x <= LeftPointX)
        {
        AniBoss.SetBool("刀波", false);
        Destroy(gameObject);
        }

    // 根据 boss 的朝向设置旋转
    if (boss.Isleft())
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        // 始终沿局部右方向移动
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            AniBoss.SetBool("刀波", false);
            Destroy(gameObject);
        }
    }
}
