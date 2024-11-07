using System.Collections.Generic;
using UnityEngine;

public class GrantAbilityProp : Prop
{
    [SerializeField] private SlimeType grantType = SlimeType.None;
    private Dictionary<SlimeType, Color> colorDict;
    private SpriteRenderer sprd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorDict = new Dictionary<SlimeType, Color> { { SlimeType.None, Color.white }, { SlimeType.Water, Color.cyan },
                                                           { SlimeType.Fire, Color.red }, { SlimeType.Grass, Color.green }, };
        sprd = gameObject.GetComponent<SpriteRenderer>();
        sprd.color = colorDict[grantType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnEnter(Collider2D collision)
    {
        base.OnEnter(collision);
        Debug.Log(collision.tag);

        if (collision.tag =="Player")
        {
            Debug.Log("prop collide with player");
            EventHandler.CallSlimeTypeEnterEvent(grantType);
            Destroy(gameObject);
        }

        
    }
}
