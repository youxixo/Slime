using UnityEngine;

public class BossairAttackPoint : MonoBehaviour
{
    public GameObject rightPoint;
    public GameObject leftPoint;
    private float rightPointX;
    private float leftPointX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rightPointX = rightPoint.transform.position.x;
        leftPointX = leftPoint.transform.position.x;
        float mid = (rightPointX + leftPointX)/2;
        transform.position = new Vector3(mid, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
