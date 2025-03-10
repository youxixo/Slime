using UnityEngine;

public class Attack : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player is attacked");
        }
    }
}
