using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public float lifetime = 3f; // 子弹存活时间
    public TagHandle tag;

    void Start()
    {
        

        // 在一定时间后销毁子弹
        Destroy(gameObject, lifetime);
    }

    public void Init(float InSpeed = 5f, float Inlifetime = 3f, string InTagName = "Player")
    {
        speed = InSpeed;
        lifetime = Inlifetime;
        tag = TagHandle.GetExistingTag(InTagName);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 子弹沿着它的前进方向移动
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
        else
        {
            //Destroy(gameObject);
        }
    }
}
