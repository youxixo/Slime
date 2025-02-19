using UnityEngine;

public class Bo : MonoBehaviour
{
    public float speed;

    private bool isleft;

    void Start()
    {
        isleft = GameObject.Find("Boss").GetComponent<Boss>().isleft;
    }

    // Update is called once per frame
    void Update()
    {
        if (isleft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }  
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
