using UnityEngine;

public class PanelParent : MonoBehaviour, IPanels
{
    public GameObject selectedButtonWhenOpen;
    public void DisableThisPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void EnableThisPanel()
    {
        this.gameObject.SetActive(true);
    }
}
