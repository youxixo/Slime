using UnityEngine;

public class CloseAttack : MonoBehaviour
{
    private CloseCombat closeCombat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeCombat = transform.parent.gameObject.GetComponent<CloseCombat>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            closeCombat.isAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            closeCombat.isAttack = false;
        }
    }
}
