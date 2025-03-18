using UnityEngine;

public class isGround : MonoBehaviour
{
    Boss boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            boss.AirColl.SetActive(false);
            boss.isGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            boss.isGround = false;
        }
    }
}
