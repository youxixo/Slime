using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaiscWaterAttack : PlayerBasicAttack
{
    
    private bool inAttackFrames;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform bulletSpawnPos;

    private void OnEnable()
    {
        EventHandler.AttackCheckStartEvent += OnAttackCheckStartEvent;
        EventHandler.AttackCheckEndEvent += OnAttackCheckEndEvent;
        slimeType = SlimeType.Water;
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


    void Update()
    {
        InputAction attackAction = controller.GetAttackAction();

        //if (attackAction.triggered)
        //{
        //    //anim.Play();
        //    Bullet bullet = GameObject.Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity, bulletParent).GetComponent<Bullet>();
        //    bullet.Init(InTagName: "Player");
        //}
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

        Bullet bullet = GameObject.Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity, bulletParent).GetComponent<Bullet>();
        bullet.Init(InTagName: "Player");
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
