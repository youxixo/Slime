using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderControl : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Sprite unSelectImage;
    public Sprite onSelectImage;
    public Image handleImage;

    public void OnSelect()
    {
        handleImage.sprite = onSelectImage;
    }

    public void OnDeselect()
    {
        handleImage.sprite = unSelectImage;
    }

    public void OnSelect(BaseEventData eventData)
    {
        handleImage.sprite = onSelectImage;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        handleImage.sprite = unSelectImage;
    }
}
