using UnityEngine;

public class PlayerAttackBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] protected PlayerController controller;
    [SerializeField] protected Animation anim;
    [SerializeField] protected SlimeType slimeType;

    [SerializeField] protected float attackCD = 2f;
    protected float timer = 0;
    protected bool canAttack = true;


    virtual public SlimeType GetSlimeType()
    {
        return slimeType;
    }

    virtual public bool CanAttack()
    {
        Debug.Log("can attck : " + canAttack.ToString()+ timer);

        if(Time.realtimeSinceStartup - timer > attackCD)
        {
            timer = Time.realtimeSinceStartup;
            return true;
        }
        return false;

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
