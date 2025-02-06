using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEditorInternal;

public class PlayerBasicFireAttack : PlayerBasicAttack
{
    [SerializeField] private bool inAttackFrames;
    [SerializeField] private SpriteRenderer sprd;
    [SerializeField] private Animator fireAttackAnim;

    [SerializeField] private float timeLength;



    private void OnEnable()
    {
        EventHandler.AttackCheckStartEvent += OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent += OnAttackCheckEndEvent;
        slimeType = SlimeType.Fire;
        //attackCD = 0;
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
        //InputAction attackAction = controller.GetAttackAction();
    }


    private void OnAttackCheckStartEvent()
    {
        Debug.LogWarning("fire start check attack collision" + Time.realtimeSinceStartup);
        inAttackFrames = true;
    }
    private void OnAttackCheckEndEvent()
    {
        Debug.LogWarning("fire end check attack collision" + Time.realtimeSinceStartup);
        inAttackFrames = false;
    }

    public override void Attack()
    {
        base.Attack();

        //Debug.Log("fire attack");
        //if(!sprd.enabled)
        //    sprd.enabled = true;
        //inAttackFrames = true;

        ////anim.Play();
        //StopAllCoroutines();
        //StartCoroutine(StopPlayAnim());

        controller.playerMove.FreeControl(timeLength);
        fireAttackAnim.SetBool("canAttack", true);
        //fireAttackAnim.Play("fireAttack");
        StartCoroutine(StopPlayAnim());
        sprd.enabled = true;

    }

    IEnumerator StopPlayAnim()
    {
        //yield return new WaitForSeconds(0.5f);
        //inAttackFrames = false;
        //if (sprd.enabled)
        //    sprd.enabled = false;
        ////fireAttackAnim.Stop();
        //Debug.LogWarning("Fire in atacck end ");
        yield return new WaitForSeconds(timeLength);
        fireAttackAnim.SetBool("canAttack", false);
        sprd.enabled = false;


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && inAttackFrames)
        {
            Debug.LogWarning("Fire check: Exit" + collision.gameObject.name);

            Debug.LogWarning("check attack collision: hit an enemy" + Time.realtimeSinceStartup);
            Destroy(collision.gameObject);
        }
    }
}
