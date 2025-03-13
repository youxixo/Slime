using UnityEngine;

public class ballCon : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1f;
    public GameObject ball1;
    public GameObject ball2;
    public GameObject ball3;
    void Awake()
    {
        ball1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1)*speed, ForceMode2D.Impulse);
        ball2.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1)*speed, ForceMode2D.Impulse);
        ball3.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, -1)*speed, ForceMode2D.Impulse);
    }
    
    void Start()
    {
        rb = GameObject.Find("Boss").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ball1 == null && ball2 == null && ball3 == null)
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Destroy(gameObject);
        }
    }
}
