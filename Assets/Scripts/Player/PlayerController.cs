using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum SlimeType
{
    None,
    Water,
    Fire,
    Grass
}

public class PlayerController : MonoBehaviour
{

    [SerializeField] private SlimeType slimeType = SlimeType.None;
    private Dictionary<SlimeType, Color> colorDict;
    private Transform trans;
    [SerializeField] private SpriteRenderer sprd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            SlimeType targetType = (int)slimeType + 1 <= 3 ? slimeType + 1 : 0;
            EventHandler.CallSlimeTypeEnterEvent(targetType);

        }

    }

    private void OnSlimeTypeEnter(SlimeType type)
    {
        EventHandler.CallSlimeTypeLeaveEvent();

        Debug.Log("switch slime type to " + type);
        slimeType = type;
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
        Debug.Log("leave slime type to " + slimeType);
        switch (slimeType)
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
