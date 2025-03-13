using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillWaterAttack : PlayerSkillAttack
{
    [SerializeField] private bool inAttackFrames;
    [SerializeField] private bool inDashDown;
    [SerializeField] private Animation anim2;
    [SerializeField] private float downForce = -1000;


    private void OnEnable()
    {
        EventHandler.AttackCheckStartEvent += OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent += OnAttackCheckEndEvent;
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
    void FixedUpdate()
    {
        InputAction attackAction = controller.GetAttackAction();

        if (inDashDown)
        {
            controller.rb.linearVelocity = new Vector2(0, downForce);

            if (controller.playerMove.CheckOnGround())
            {
                Debug.LogWarning("LAND ON GORUND");
                controller.playerMove.allowToMove = true;
                inDashDown = false;
                controller.playerMove.EnableWallstick();
                controller.playerMove.ChangeBackNormalMaxVel();
                inAttackFrames = false;
                anim.Play();
                anim2.Play();
            }
        }

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

        controller.playerMove.allowToMove = false;
        controller.playerMove.DisableWallstick();
        controller.playerMove.ChangeWaterSkillMaxVel();
        controller.rb.transform.rotation = Quaternion.identity;
        controller.rb.linearVelocity = new Vector2(0, downForce);
        //controller.rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse);
        inDashDown = true;
        inAttackFrames = true;
        
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && inAttackFrames)
        {
            Debug.Log("grass check attack collision: hit an enemy" + Time.realtimeSinceStartup);
           // Destroy(collision.gameObject);
        }
    }
}
