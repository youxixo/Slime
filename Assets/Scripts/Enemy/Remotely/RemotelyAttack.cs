using UnityEngine;

public class RemotelyAttack : MonoBehaviour
{
    private Remotely remotely;
    void Start()
    {
        remotely = transform.parent.gameObject.GetComponent<Remotely>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            remotely.isAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            remotely.isAttack = false;
        }
    }
}
