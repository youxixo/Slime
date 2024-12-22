using UnityEngine;

public class CollideAttack : MonoBehaviour
{
    private Collide collide;
    private float player;
    private float LeftPointX;
    private float RightPointX;

    void Start()
    {
        collide = transform.parent.gameObject.GetComponent<Collide>();
        LeftPointX = collide.LeftPoint.position.x;
        RightPointX = collide.RightPoint.position.x;
    }

    void Update()
    {
        player = collide.PlayerTransformX;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPlayerPosition();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckPlayerPosition();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collide.isAttack = false;
        }
    }

    private void CheckPlayerPosition()
    {
        if (LeftPointX <= player && RightPointX >= player)
        {
            collide.isAttack = true;
        }
        else
        {
            collide.isAttack = false;
        }
    }
}
