using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindDisplay : MonoBehaviour
{
    public TMP_Text actionNameText;
    public Transform buttonParent;
    public KeybindButton buttonPrefab;
    private Dictionary<string, KeybindButton> buttons = new();

    public void SetActionNameText(string actionName)
    {
        actionNameText.text = actionName;
    }

    public KeybindButton GenerateButton(string compositeParent, string bindingText, string actionName, string tagName)
    {
        KeybindButton newButton = Instantiate(buttonPrefab, buttonParent);//.GetComponent<KeybindButton>().buttonText.text = bindingText;
        buttons.Add(actionName, newButton);
        newButton.tag = tagName;
        newButton.buttonText.text = bindingText;
        newButton.gameObject.name = actionName;

        if (compositeParent != null) 
            newButton.actionName = compositeParent + " " + actionName;
        else
            newButton.actionName = actionName;

        return newButton;
    }
}
