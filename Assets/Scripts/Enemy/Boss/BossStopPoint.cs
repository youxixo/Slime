using UnityEngine;

public class BossStopPoint : MonoBehaviour
{
    public GameObject BossairAttackPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(BossairAttackPoint.transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
