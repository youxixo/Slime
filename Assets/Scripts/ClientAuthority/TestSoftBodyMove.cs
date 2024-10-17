using UnityEngine;
using UnityEngine.Events;

public class TestSoftBodyMove : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] float gravity = 50f;
    public static Vector2 gravityDir = Vector2.down;
    private bool alreadyChangeGravity = false;
    private bool jumpClicked = false;
    public static UnityEvent resetBoneGravity;

    private void Awake()
    {
        if (resetBoneGravity == null)
            resetBoneGravity = new UnityEvent();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Bone.OnBoneWallColldie.AddListener(ChangeGravityDir);
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(x, y);

        // Use the Run method to handle movement
        Run(direction);

        // Check for jump input and perform the jump in the gravity direction
        if (Input.GetButtonDown("Jump"))
        {
            Jump(gravityDir);
        }

        // Apply a continuous gravity force in the current gravity direction
        rb.AddForce(gravityDir * gravity, ForceMode2D.Impulse);
    }

    void Run(Vector2 dir)
    {
        // Project the input direction onto the plane perpendicular to gravityDir
        Vector2 projectedDir = dir - Vector2.Dot(dir, gravityDir.normalized) * gravityDir.normalized;

        // Normalize the projected direction to avoid speed variance due to input angles
        if (projectedDir != Vector2.zero)
        {
            projectedDir = projectedDir.normalized;
        }

        // Apply the movement at a consistent speed in the projected direction
        rb.linearVelocity = projectedDir * speed + Vector2.Dot(rb.linearVelocity, gravityDir) * gravityDir;
    }



    void Jump(Vector2 dir)
    {
        ResetGravityForAll();
        rb.linearVelocityY = 0;
        rb.linearVelocity += -gravityDir * jumpPower;
    }

    private void ResetGravityForAll()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        gravityDir = Vector2.down;
        resetBoneGravity.Invoke();
    }

    private void ChangeGravityDir(Vector2 contactDir)
    {
        if (contactDir != gravityDir) //!alreadyChangeGravity)
        {
            Debug.Log("new gravity: " + contactDir);
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            alreadyChangeGravity = true;
            gravityDir = contactDir;
            resetBoneGravity.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("leaving wall");
        ResetGravityForAll();
        alreadyChangeGravity = false;
        //gravityDir = Vector2.down; // Reset gravity to default when leaving a wall
    }

}
