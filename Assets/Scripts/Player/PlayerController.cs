using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.InputSystem.Interactions;
using UnityEditor;
using UnityEngine;
using Unity.Collections;
using UnityEditor.Animations;

public enum SlimeType
{
    Water,
    Fire,
    Grass
}

public class PlayerController : MonoBehaviour
{
    public PlayerMove playerMove;
    public Rigidbody2D rb;
    public Animator animator;

    public AnimatorController waterAnimator;
    public AnimatorController fireAnimator;
    public AnimatorController grassAnimator;

    [SerializeField] private SlimeType currentSlimeType = SlimeType.Water;
    private Dictionary<SlimeType, Color> colorDict;
    private Transform trans;
    [SerializeField] private SpriteRenderer sprd;


    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction attackAction;
    private InputAction skillAction;


    private Dictionary<SlimeType, PlayerBasicAttack> attackDict = new Dictionary<SlimeType, PlayerBasicAttack> { };
    private Dictionary<SlimeType, PlayerSkillAttack> skillDict = new Dictionary<SlimeType, PlayerSkillAttack> { };

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private Transform healthSpawnParent;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private List<Transform> healths;





    public InputAction GetAttackAction()
    {
        return attackAction;
    }
    public InputAction GetSkillAction()
    {
        return skillAction;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitInput();

        foreach(Transform go in transform.Find("Attack"))
        {
            PlayerBasicAttack attack = go.GetComponent<PlayerBasicAttack>();

            go.gameObject.SetActive(true);
            attackDict.Add(attack.GetSlimeType(), attack);
            go.gameObject.SetActive(false);
        }

        foreach (Transform go in transform.Find("Skill"))
        {
            PlayerSkillAttack skill = go.GetComponent<PlayerSkillAttack>();

            go.gameObject.SetActive(true);
            skillDict.Add(skill.GetSlimeType(), skill);
            go.gameObject.SetActive(false);
        }

        //foreach (Key k in attackDict.Keys)
        //    Debug.LogWarning(k);

        //foreach (var v in attackDict.Values)
        //    Debug.LogWarning(v);
        //Debug.LogWarning(attackDict.Keys.ToString());

        //colorDict = new Dictionary<SlimeType, Color> { { SlimeType.Water, Color.cyan }, 
        //                                               { SlimeType.Fire, Color.red }, { SlimeType.Grass, Color.green }, };
        trans = gameObject.GetComponent<Transform>();
        //sprd = gameObject.GetComponent<SpriteRenderer>();

        EventHandler.CallSlimeTypeEnterEvent(SlimeType.Water);
        ChangeToWater();


        // 生成生命
        for (int i = 0; i < maxHealth; i++)
        {
            Debug.Log("Initilizing Health");
            GameObject health = GameObject.Instantiate(healthPrefab, healthSpawnParent);
            healths.Add(health.transform);
        }
        currentHealth = maxHealth;

    }



    private void OnEnable()
    {
        EventHandler.SlimeTypeEnterEvent += OnSlimeTypeEnter;
        EventHandler.SlimeTypeLeaveEvent += OnSlimeTypeLeave;

    }
    private void OnDisable()
    {
        EventHandler.SlimeTypeEnterEvent -= OnSlimeTypeEnter;
        EventHandler.SlimeTypeLeaveEvent += OnSlimeTypeLeave;

    }

    // Update is called once per frame
    void Update()
    {


        //Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

        // Change Playmode Tint in editor preferences
        //EditorPrefs.SetString("Playmode Tint", UnityEngine.ColorUtility.ToHtmlStringRGBA(randomColor));

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SlimeType targetType = (int)currentSlimeType + 1 < 3 ? currentSlimeType + 1 : 0;
            ChangeType(targetType);
            EventHandler.CallSlimeTypeEnterEvent(targetType);
            Debug.LogWarning(targetType);
        }

        if (attackAction.ReadValue<float>() > 0)
        {
            //EventHandler.CallAttackEvent();

            if (attackDict[currentSlimeType].CanAttack())
            {
                foreach (KeyValuePair<SlimeType, PlayerBasicAttack> pair in attackDict)
                {
                    if (pair.Key == SlimeType.Fire) continue;
                    pair.Value.gameObject.SetActive(false);
                }
                attackDict[currentSlimeType].gameObject.SetActive(true);
                attackDict[currentSlimeType].Attack();

            }

            // TODO: ¹¥»÷½áÊøºó×Ô¼ºdisenable

        }

        if (skillAction.ReadValue<float>() > 0)
        {
            //EventHandler.CallAttackEvent();

            if (skillDict[currentSlimeType].CanAttack())
            {
                foreach (KeyValuePair<SlimeType, PlayerSkillAttack> pair in skillDict)
                {
                    pair.Value.gameObject.SetActive(false);
                }
                skillDict[currentSlimeType].gameObject.SetActive(true);
                skillDict[currentSlimeType].Attack();

            }

            // TODO: ¹¥»÷½áÊøºó×Ô¼ºdisenable

        }


    }

    private void InitInput()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        attackAction = playerActionMap.FindAction("Attack");
        skillAction = playerActionMap.FindAction("Skill");

        Debug.Log("asd");
    }

    private void OnSlimeTypeEnter(SlimeType type)
    {
        EventHandler.CallSlimeTypeLeaveEvent();

        Debug.Log("switch slime type to " + type);
        currentSlimeType = type;
        //sprd.color = colorDict[type];
        switch (type)
        {
            case SlimeType.Water:
                break;
            case SlimeType.Fire:
                break;
            case SlimeType.Grass:
                break;
            default:
                break;
        }

    }

    private void OnSlimeTypeLeave()
    {
        Debug.Log("leave slime type to " + currentSlimeType);
        switch (currentSlimeType)
        {
            case SlimeType.Water:
                break;
            case SlimeType.Fire:
                break;
            case SlimeType.Grass:
                break;
            default:
                break;
        }
    }

    private void ChangeType(SlimeType targetType)
    {
        Debug.Log(currentSlimeType.ToString() + "To" + targetType.ToString());
        animator.SetTrigger(currentSlimeType.ToString() + "To" + targetType.ToString());
    }

    public void ChangeToFire()
    {
        animator.runtimeAnimatorController = fireAnimator;
    }

    public void ChangeToGrass()
    {
        animator.runtimeAnimatorController = grassAnimator;
    }

    public void ChangeToWater()
    {
        animator.runtimeAnimatorController = waterAnimator;
    }
    
    public bool FacingToRightDirection()
    {
        if (transform.localScale.x >= 0)
            return true;
        else
            return false;

    }

    [Header("碰撞伤害")]
    [SerializeField] private float horzForce = 100;
    [SerializeField] private float vertForce = 100;
    [SerializeField] private float freezeTime = 0.2f;


    private void DealHurt(int damage)
    {
        ChangeHealth(-damage);

    }

    private void ChangeHealth(int value)
    {
        currentHealth += value;
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);

        for (int i = 0; i < maxHealth; i++)
        {
            if(i < currentHealth)
            {
                healths[i].Find("Health").gameObject.SetActive(true);
            }
            else
            {
                healths[i].Find("Health").gameObject.SetActive(false);
            }
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.collider.tag == "Enemy")
        {
            Debug.LogWarning("collide with enemy");
            int horzDir = collision.transform.position.x < gameObject.transform.position.x ? 1 : -1;
            Vector2 hurtForce = new Vector2(horzForce * horzDir, vertForce);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(hurtForce, ForceMode2D.Impulse);
            playerMove.FreeControl(freezeTime);

            ChangeHealth(-1);
        }


    }

}
