using UnityEngine;

public class CloseCombat : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public float MoveSpeed = 1f;//移动速度
    private float LeftPointX;
    private float RightPointX;
    private float Speed;
    private Rigidbody2D rb;
    public bool isAttack = false;
    
    private bool isleft = false;//面朝向

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
       move(); 
    }

    void move()
    {
        if (!isleft)
        {
            rb.transform.rotation = Quaternion.Euler(0, 180, 0);//面朝向右
            
            rb.MovePosition(rb.position + new Vector2(Speed * Time.deltaTime, 0));
            if(rb.position.x >= RightPointX)
            {
                isleft = true;
            }
           
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0, 0, 0);//面朝向左
            
            rb.MovePosition(rb.position + new Vector2(-Speed * Time.deltaTime, 0));
           if(rb.position.x <= LeftPointX)
           {
               isleft = false;
           }
        }
    }
}
