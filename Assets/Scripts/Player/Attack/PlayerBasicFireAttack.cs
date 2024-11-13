using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerBasicFireAttack : PlayerBasicAttack
{
    private bool inAttackFrames;
    [SerializeField] private SpriteRenderer sprd;


    private void OnEnable()
    {
        EventHandler.AttackCheckStartEvent += OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent += OnAttackCheckEndEvent;
        slimeType = SlimeType.Fire;
        attackCD = 0;
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

        Debug.Log("fire attack" + Time.realtimeSinceStartup);

        sprd.enabled = true;

        //anim.Play();
        StopAllCoroutines();
        StartCoroutine(StopPlayAnim());
 
    }

    IEnumerator StopPlayAnim()
    {
        yield return new WaitForSeconds(0.5f);

        sprd.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag == "Enemy" && inAttackFrames)
        {
            Debug.Log("check attack collision: hit an enemy" + Time.realtimeSinceStartup);
            Destroy(collision.gameObject);
        }
    }
}
