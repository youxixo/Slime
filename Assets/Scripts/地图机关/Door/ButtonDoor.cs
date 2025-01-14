using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    private Animator ani;
    private bool isPlayerInTrigger = false; // Tracks if the player is in the trigger zone
    public bool isOpen = false; // Tracks if the door is open

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if the player is in the trigger zone and presses the F key
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            ani.SetBool("Open", true);
            isOpen = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInTrigger = true; // Player entered the trigger zone
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInTrigger = false; // Player left the trigger zone
        }
    }
}
