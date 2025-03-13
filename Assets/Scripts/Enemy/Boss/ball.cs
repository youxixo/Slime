using UnityEngine;

public class ball : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Destroy(gameObject);
        }
        if(collision.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
