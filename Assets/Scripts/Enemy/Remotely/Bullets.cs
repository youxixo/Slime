using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 5f; // 子弹速度
    public float lifetime = 3f; // 子弹存活时间

    void Start()
    {
        // 在一定时间后销毁子弹
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 子弹沿着它的前进方向移动
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // 处理命中玩家的逻辑，比如减少玩家的生命值
            Debug.Log("Player hit!");
            Destroy(gameObject); // 子弹命中后销毁
        }
        else if (other.tag == "Obstacle")
        {
            // 如果碰到其他障碍物，销毁子弹
            Destroy(gameObject);
        }
    }
}
