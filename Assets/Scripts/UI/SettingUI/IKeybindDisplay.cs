using UnityEngine;

public interface IKeybindDisplay
{
    public KeybindButton GenerateButton(string compositeParent, string bindingText, string actionName, string tagName);
}
