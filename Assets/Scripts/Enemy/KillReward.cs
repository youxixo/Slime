using UnityEngine;

public class KillReward : MonoBehaviour
{
    public GameObject water;
    public GameObject fire;
    public GameObject grassy;
    private bool Randed;
    private int random;
    void Start()
    {
        Randed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Randed == false)
        {
            random = Random.Range(1, 10);
            Randed = true;
            GetReward();
        }
    }

    void GetReward()
    {
        if(random == 1 || random == 2)
        {
            Instantiate(water, transform.position, Quaternion.identity);
        }
        else if(random == 4 || random == 5)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
        }
        else if(random == 7 || random == 8)
        {
            Instantiate(grassy, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
