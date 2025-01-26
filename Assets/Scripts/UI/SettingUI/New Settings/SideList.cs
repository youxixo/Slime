using UnityEngine;
using UnityEngine.EventSystems;

public class SideList : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"{gameObject.name} selected!");
    }
}
