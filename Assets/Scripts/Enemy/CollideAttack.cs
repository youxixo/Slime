using UnityEngine;

public class CollideAttack : MonoBehaviour
{
    private Collide collide;
    void Start()
    {
        collide = transform.parent.gameObject.GetComponent<Collide>();
    }
   void OnTriggerEnter2D(Collider2D other)
{
    if (other.tag == "Player")
    {
        collide.isAttack = true;
    }
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.tag == "Player")
    {
        collide.isAttack = false;
    }
}

}
