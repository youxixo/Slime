using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public Remotely remotely;
    private bool isleft;

    void Start()
    {
        isleft = remotely.isleft;
    }

    void Update()
    {
        if (isleft)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Ground"))
    {
        // 如果碰到地面，销毁子弹
        Debug.Log("Bullet hit the ground!");
        Destroy(gameObject);
    }
    if (other.CompareTag("Player"))
    {
        // 处理命中玩家的逻辑，比如减少玩家的生命值
        Debug.Log("Player hit!");
        Destroy(gameObject); // 子弹命中后销毁
    }
    
}
}
