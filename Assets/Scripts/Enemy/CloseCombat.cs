using Unity.VisualScripting;
using UnityEngine;

public class CloseCombat : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f; // Movement speed
    public GameObject DeadEffect;
    private float LeftPointX;
    private float RightPointX;
    private Rigidbody2D rb;
    private bool isMovingLeft = true;

    private float playerLocalScale;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LeftPointX = LeftPoint.position.x;
        RightPointX = RightPoint.position.x;

        Destroy(LeftPoint.gameObject);
        Destroy(RightPoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        playerLocalScale = GameObject.Find("PlayerFR").transform.localScale.x;
        Move();
    }

    void Move()
    {
        // Set direction based on movement
        float direction = isMovingLeft ? -1 : 1;
        rb.transform.rotation = Quaternion.Euler(0, isMovingLeft ? 0 : 180, 0);

        // Move the object
        rb.MovePosition(rb.position + new Vector2(direction * MoveSpeed * Time.deltaTime, 0));

        // Check boundaries
        if (isMovingLeft && rb.position.x <= LeftPointX)
        {
            isMovingLeft = false;
        }
        else if (!isMovingLeft && rb.position.x >= RightPointX)
        {
            isMovingLeft = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "PlayerAttack")
        {
            if(playerLocalScale == 1)
            {
                rb.AddForce(new Vector2(1, 1).normalized * 3, ForceMode2D.Impulse);
            }
            else if(playerLocalScale == -1)
            {
                rb.AddForce(new Vector2(-1, 1).normalized * 3, ForceMode2D.Impulse);
            }

            Instantiate(DeadEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
