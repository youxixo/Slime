using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //this.GetComponent<Button>().onClick.AddListener(SelectChild);
    }

    private void SelectChild()
    {
        GameObject child = this.transform.GetChild(0).gameObject;
        EventSystem.current.SetSelectedGameObject(child);
        //EventSystem.current.gameObject.transform.GetChild(0).TryGetComponent<>
    }
}
