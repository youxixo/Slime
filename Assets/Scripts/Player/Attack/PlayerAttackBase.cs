using UnityEngine;

public class PlayerAttackBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] protected PlayerController controller;
    [SerializeField] protected Animation anim;
    [SerializeField] protected SlimeType slimeType;

    virtual public SlimeType GetSlimeType()
    {
        return slimeType;
    }


    void Start()
    {
        //controller = GetComponent<PlayerController>();
        //Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual protected void Init()
    {
        anim = GetComponent<Animation>();
    }

    virtual public void Attack()
    {

    }

}
