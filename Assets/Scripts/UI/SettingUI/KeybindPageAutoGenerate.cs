using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

/// <summary>
/// Auto generate and update the keybinds for inputs
/// </summary>
public class KeybindPageAutoGenerate : MonoBehaviour
{
    public InputActionAsset inputActions;
    public KeybindDisplay keybindDisplayPrefab;
    public Transform scrollViewContent;
    public InputActionMap currentActionMap;
    public TMP_Text actionMapNameDisplay;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation; //rebind
    private KeybindButton selectedKeyDisplay;
    private Dictionary<string, KeybindButton> ActionNameBindingPair = new(); //key = action name, value = array of key bindings
    private int compositeKeyIndex;
    private string newBindingsJson;
    private string oldBindingJson;

    private const string Single_Keybind_Tag = "SingleKeybind";
    private const string Composite_Keybind_Tag = "CompositeKeybind";

    private void Start()
    {
        LoadActionMapKeybindPage("Player");
        //currentActionMap.SaveBindingOverridesAsJson();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            currentActionMap.Disable(); //***Delete after, sb unity activate all actionmap when start

            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            //***蠢 又再次取得GetComponent 看看之後能不能換方法 
            if (selectedObject != null)
            {
                selectedKeyDisplay = selectedObject.GetComponent<KeybindButton>();
                if (selectedObject.tag == Single_Keybind_Tag)
                {
                    Debug.Log("changing rebind for: " + selectedKeyDisplay.actionName);
                    SingleKeyBind(selectedObject.gameObject.name);
                }
                else if (selectedObject.tag == Composite_Keybind_Tag)
                {
                    Debug.Log("changing rebind for: " + selectedKeyDisplay.actionName);
                    CompositeKeyBind(selectedObject.gameObject.name);
                }
                else
                    Debug.LogWarning("Selected Object is not keybind");
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RestoreToDefault();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LoadActionMapKeybindPage("Player");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (CompareKeybindSet())
                Debug.Log("Same keybind");
            else
                Debug.Log("Diff");
            SaveKeybindSet();
            DestroyBindingDisplay();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (CompareKeybindSet())
                Debug.Log("Same keybind");
            else
                Debug.Log("Diff");
            NoSaveKeybindSet();
            DestroyBindingDisplay();
        }


    }

    /// <summary>
    /// Get the keybindpage with the mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void LoadActionMapKeybindPage(string mapName)
    {
        if(actionMapNameDisplay)
            actionMapNameDisplay.text = mapName;

        currentActionMap = inputActions.FindActionMap(mapName);
        if (currentActionMap != null)
        {
            //oldBindingJson = currentActionMap.ToJson(); ToJson returns the default one, not the override
            oldBindingJson = currentActionMap.SaveBindingOverridesAsJson();
            Debug.Log(oldBindingJson);
            currentActionMap.Disable();
            SpawnKeyViewItem(currentActionMap);
        }
        else
        {
            Debug.LogWarning("the map you looking for is missing");
        }
    }

    /// <summary>
    /// Destroy all buttons
    /// </summary>
    public void DestroyBindingDisplay()
    {
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Restore to default
    /// </summary>
    public void RestoreToDefault()
    {
        currentActionMap.RemoveAllBindingOverrides();
        currentActionMap.ToJson();
        RefreshBindingDisplay();
    }

    /// <summary>
    /// Refresh all binding displays
    /// </summary>
    public void RefreshBindingDisplay()
    {
        foreach (InputAction action in currentActionMap.actions)
        {
            if (action.bindings.Count > 1)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    int bindingIndex = action.bindings.IndexOf(b => b == binding);
                    if (ActionNameBindingPair.ContainsKey(binding.name))
                    {
                        ActionNameBindingPair[binding.name].buttonText.text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                    }
                    else
                    {
                        //Debug.LogWarning("Composite set to default failed: " + binding.name);
                    }
                }
            }
            else
            {
                if (ActionNameBindingPair.ContainsKey(action.name))
                {
                    //***hard code 0, change later
                    ActionNameBindingPair[action.name].buttonText.text = action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                }
            }
        }
    }

    /// <summary>
    /// Instantiate the key displays according to the actionmap
    /// </summary>
    /// <param name="actionMap"></param>
    private void SpawnKeyViewItem(InputActionMap actionMap)
    {
        foreach (InputAction action in actionMap.actions)
        {
            KeybindDisplay displayItem = Instantiate(keybindDisplayPrefab.gameObject, scrollViewContent).GetComponent<KeybindDisplay>();
            displayItem.gameObject.name = action.name;
            //Case: Composite key
            if (action.bindings.Count > 1)
            {
                displayItem.SetActionNameText(action.name);
                foreach (InputBinding binding in action.bindings)
                {
                    if (binding.groups.Contains("Keyboard"))
                    {
                        //***indexOf is kinda trash, it iterate through the action list, try find other way
                        int bindingIndex = action.bindings.IndexOf(b => b == binding);
                        if (bindingIndex != -1)
                        {
                            string bindingText = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                            ActionNameBindingPair[binding.name] = displayItem.GenerateButton(action.name, bindingText, binding.name, Composite_Keybind_Tag);
                            //ActionNameBindingPair[binding.name] = bindingText;
                        }
                    }
                }
            }
            //Case: Single Key
            else
            {
                string bindingText = action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                displayItem.SetActionNameText(action.name);
                ActionNameBindingPair[action.name] = displayItem.GenerateButton(null, action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions), action.name, Single_Keybind_Tag);
                //ActionNameBindingPair[action.name] = bindingText;
            }
        }
    }

    /// <summary>
    /// Starting binding single key action
    /// </summary>
    /// <param name="actionName"></param>
    private void SingleKeyBind(string actionName)
    {
        InputAction changingAction = currentActionMap.FindAction(actionName);
        if (changingAction == null)
        {
            Debug.LogWarning(actionName + " not found");
            selectedKeyDisplay = null;
            return;
        }

        _rebindingOperation = changingAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind(actionName, changingAction))
            .Start();
    }

    /// <summary>
    /// Starting binding composite key action
    /// </summary>
    /// <param name="actionName"></param>
    private void CompositeKeyBind(string actionName)
    {
        InputAction changingAction = currentActionMap.FindAction(selectedKeyDisplay.transform.parent.parent.name);
        for (int i = 0; i < changingAction.bindings.Count; i++)
        {
            if (changingAction.bindings[i].name == actionName)
            {
                compositeKeyIndex = i;
                _rebindingOperation = changingAction.PerformInteractiveRebinding(i)
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .OnComplete(operation => CompleteRebind(actionName, changingAction))
                    .Start();
                return;
            }
        }
        Debug.LogWarning("Could not rebind composite key, action name not found, setting index to -1");
        compositeKeyIndex = -1;
    }

    /// <summary>
    /// 完成鍵位設置時 Dispose防止leak
    /// </summary>
    /// <param name="actionName"></param>
    /// <param name="changingAction"></param>
    private void CompleteRebind(string actionName, InputAction changingAction)
    {
        _rebindingOperation.Dispose();
        string bindingText = "Error";
        if (selectedKeyDisplay.tag == Single_Keybind_Tag)
        {
            //***0 here is hardcode to find the first binding index fix later?
            bindingText = changingAction.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
        else if (selectedKeyDisplay.tag == Composite_Keybind_Tag)
        {
            bindingText = changingAction.GetBindingDisplayString(compositeKeyIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }
        //ActionNameBindingPair[actionName] = bindingText;
        selectedKeyDisplay.buttonText.text = bindingText;
        selectedKeyDisplay = null;
    }

    public void SaveKeybindSet()
    {
        //currentActionMap.SaveBindingOverridesAsJson();
    }

    public void NoSaveKeybindSet()
    {
        currentActionMap.LoadBindingOverridesFromJson(oldBindingJson);
    }

    private bool CompareKeybindSet()
    {
        if (oldBindingJson == currentActionMap.SaveBindingOverridesAsJson())
            return true;
        return false;
    }
}
