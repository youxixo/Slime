using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject button;
    private ButtonDoor buttonDoor;
    private Animator ani;
    void Start()
    {
        ani = GetComponent<Animator>();
        buttonDoor = button.GetComponent<ButtonDoor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonDoor.isOpen)
        {
            ani.SetBool("Open", true);
        }
    }
}
