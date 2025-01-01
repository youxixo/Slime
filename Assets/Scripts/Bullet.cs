using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public float lifetime = 3f; // 子弹存活时间
    public TagHandle tag;
    public Vector2 dir;
    void Start()
    {
        

        // 在一定时间后销毁子弹
        Destroy(gameObject, lifetime);
    }

    public void Init(float InSpeed = 5f, float Inlifetime = 3f, string InTagName = "Player", Vector2? InDir = null)
    {
        speed = InSpeed;
        lifetime = Inlifetime;
        tag = TagHandle.GetExistingTag(InTagName);
        dir = InDir ?? Vector2.right;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 子弹沿着它的前进方向移动
        transform.Translate(dir * speed * Time.deltaTime,Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning("collider 1");
        if (other.tag == tag.ToString()) return;
        Debug.Log(tag.ToString() + " bullet hit" + other );
        if (other.tag == "Player")
        {
            EventHandler.CallBulletHitPlayerEvent(tag);
            Destroy(gameObject); // 子弹命中后销毁
        }
        else if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
            EventHandler.CallBulletHitEnemyEvent(tag);
            Destroy(gameObject);
        }
        else if (other.tag == "Ground")
        {
            Destroy(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning("collider 2");
        if (collision.collider.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
