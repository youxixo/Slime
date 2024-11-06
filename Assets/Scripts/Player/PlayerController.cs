using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

enum SlimeType
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
    [SerializeField] private Transform trans;
    [SerializeField] private SpriteRenderer sprd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorDict = new Dictionary<SlimeType, Color> { { SlimeType.None, Color.white }, { SlimeType.Water, Color.cyan }, 
                                                           { SlimeType.Fire, Color.red }, { SlimeType.Grass, Color.green }, };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnSlimeTypeLeave(slimeType);
            OnSlimeTypeEnter(slimeType = (int)++slimeType <= 3 ? slimeType : 0);
        }

    }

    private void OnSlimeTypeEnter(SlimeType type)
    {
        Debug.Log("switch slime type to " + type);

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

    private void OnSlimeTypeLeave(SlimeType type)
    {
        Debug.Log("leave slime type to " + type);
        switch (type)
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
