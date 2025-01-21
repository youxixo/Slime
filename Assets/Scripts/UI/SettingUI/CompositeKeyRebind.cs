using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompositeKeyRebind : MonoBehaviour, IKeybindDisplay
{
    public TMP_Text actionName;
    public Button button;
    public TMP_Text buttonText;
    public Transform buttonParent;
    public KeybindButton buttonPrefab;

    public KeybindButton GenerateButton(string compositeParent, string bindingText, string actionName, string tagName)
    {
        KeybindButton newButton = Instantiate(buttonPrefab, buttonParent);//.GetComponent<KeybindButton>().buttonText.text = bindingText;
        Debug.Log(compositeParent + " " + actionName);
        buttonParent.gameObject.name = compositeParent + " " + actionName;
        return newButton;
    }
}
