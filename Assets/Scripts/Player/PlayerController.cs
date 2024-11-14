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

public enum SlimeType
{
    None,
    Water,
    Fire,
    Grass
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] private SlimeType currentSlimeType = SlimeType.None;
    private Dictionary<SlimeType, Color> colorDict;
    private Transform trans;
    [SerializeField] private SpriteRenderer sprd;


    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction attackAction;

    private Dictionary<SlimeType, PlayerBasicAttack> attackDict = new Dictionary<SlimeType, PlayerBasicAttack> { };

    public InputAction GetAttackAction()
    {
        return attackAction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitInput();

        foreach(Transform go in transform.Find("Attack"))
        {
            PlayerBasicAttack attack = go.GetComponent<PlayerBasicAttack>();
            Debug.Log(attack);
            go.gameObject.SetActive(true);
            attackDict.Add(attack.GetSlimeType(), attack);
            go.gameObject.SetActive(false);
        }



        colorDict = new Dictionary<SlimeType, Color> { { SlimeType.None, Color.white }, { SlimeType.Water, Color.cyan }, 
                                                       { SlimeType.Fire, Color.red }, { SlimeType.Grass, Color.green }, };
        trans = gameObject.GetComponent<Transform>();
        //sprd = gameObject.GetComponent<SpriteRenderer>();

        
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SlimeType targetType = (int)currentSlimeType + 1 <= 3 ? currentSlimeType + 1 : 0;
            EventHandler.CallSlimeTypeEnterEvent(targetType);

        }

        if (attackAction.ReadValue<float>() > 0)
        {
            //EventHandler.CallAttackEvent();

            if (attackDict[currentSlimeType].CanAttack())
            {
                foreach (KeyValuePair<SlimeType, PlayerBasicAttack> pair in attackDict)
                { 
                    pair.Value.gameObject.SetActive(false);
                    Debug.Log(pair);
                }
                attackDict[currentSlimeType].gameObject.SetActive(true);
                attackDict[currentSlimeType].Attack();

            }

            // TODO: ¹¥»÷½áÊøºó×Ô¼ºdisenable

        }


    }

    private void InitInput()
    {
        var playerActionMap = inputActions.FindActionMap("Player");

        attackAction = playerActionMap.FindAction("Attack");

    }

    private void OnSlimeTypeEnter(SlimeType type)
    {
        EventHandler.CallSlimeTypeLeaveEvent();

        Debug.Log("switch slime type to " + type);
        currentSlimeType = type;
        sprd.color = colorDict[type];
        switch (type)
        {
            case SlimeType.None:
                break;
            case SlimeType.Water:
                transform.DOScale(new Vector3(2, 2, 2), 1)
                    .SetEase(Ease.InOutQuart);
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
            case SlimeType.None:
                break;
            case SlimeType.Water:
                transform.DOScale(new Vector3(1, 1, 1), 1)
                    .SetEase(Ease.InOutQuart);
                break;
            case SlimeType.Fire:
                break;
            case SlimeType.Grass:
                break;
            default:
                break;
        }

    }


}
