using UnityEngine;

public class Prop : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnter(collision);
    }

    virtual protected void OnEnter(Collider2D collision)
    {

    }
}
