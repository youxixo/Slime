using UnityEngine;
using UnityEngine.Events;

public class Bone : MonoBehaviour
{
    Rigidbody2D rb;
    public static UnityEvent<Vector2> OnBoneWallColldie;
    [SerializeField] int gravityForce = 10;

    private void Awake()
    {
        if (OnBoneWallColldie == null)
            OnBoneWallColldie = new UnityEvent<Vector2>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //TestSoftBodyMove.resetBoneGravity.AddListener(ResetVelocity);
    }

    private void Update()
    {
        //rb.AddForce(TestSoftBodyMove.gravityDir * gravityForce);
    }

    private void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Vector2 contactNormal = collision.contacts[0].normal;
            OnBoneWallColldie.Invoke(-contactNormal); // Use negative normal to get the correct gravity direction
        }
    }
}
