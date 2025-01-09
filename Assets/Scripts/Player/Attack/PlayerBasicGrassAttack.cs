using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBasicGrassAttack : PlayerBasicAttack
{
    [SerializeField] private bool inAttackFrames;



    private void OnEnable()
    {
        EventHandler.AttackCheckStartEvent += OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent += OnAttackCheckEndEvent;
        slimeType = SlimeType.Grass;
        Init();
    }

    private void OnDisable()
    {
        EventHandler.AttackCheckStartEvent -= OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent -= OnAttackCheckEndEvent;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        InputAction attackAction = controller.GetAttackAction();

    }


    private void OnAttackCheckStartEvent()
    {
        Debug.Log("start check attack collision" + Time.realtimeSinceStartup);
        inAttackFrames = true;
    }
    private void OnAttackCheckEndEvent()
    {
        Debug.Log("end check attack collision" + Time.realtimeSinceStartup);
        inAttackFrames = false;
    }

    public override void Attack()
    {
        base.Attack();

        anim.Play();
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("check attack collision: hit an enemy");
        if (collision.tag == "Enemy" && inAttackFrames)
        {
            Debug.Log("check attack collision: hit an enemy" + Time.realtimeSinceStartup);
            Destroy(collision.gameObject);
        }
    }

}
