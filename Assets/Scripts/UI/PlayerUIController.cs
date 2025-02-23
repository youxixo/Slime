using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public Image typeSr;
    public Sprite waterIcon;
    public Sprite fireIcon;
    public Sprite grassIcon;
    public Transform healthParent;

    private void Start()
    {
        EventHandler.SlimeTypeEnterEvent += ChangePlayerUI;
    }

    private void ChangePlayerUI(SlimeType newType)
    {
        switch (newType)
        {
            case SlimeType.Water:
                typeSr.sprite = waterIcon;
                typeSr.color = Color.cyan;
                ChangeHealthUI(Color.cyan);
                break;
            case SlimeType.Fire:
                typeSr.sprite = fireIcon;
                typeSr.color = Color.red;
                ChangeHealthUI(Color.red);
                break;
            case SlimeType.Grass:
                typeSr.sprite = grassIcon;
                typeSr.color = Color.green;
                ChangeHealthUI(Color.green);
                break;
        }
    }

    private void ChangeHealthUI(Color newColor)
    {
        foreach(Transform health in healthParent)
        {
            health.GetChild(1).GetComponent<Image>().color = newColor;
        }
    }
}
