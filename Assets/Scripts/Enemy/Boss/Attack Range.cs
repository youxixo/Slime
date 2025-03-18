using UnityEngine;

public class AttackRange : MonoBehaviour
{
    Boss boss;
    private void Awake()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            boss.CanAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            boss.CanAttack = false;
        }
    }
}
