using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using DG.Tweening;
using UnityEngine.UIElements;

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
    public TMP_Text bindingDescription;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation; //rebind
    private KeybindButton selectedKeyDisplay;
    private Dictionary<string, KeybindButton> ActionButtonPair = new(); //key = action name, value = array of key bindings
    private Dictionary<string, KeybindButton> keyButtonPairs = new();
    private int compositeKeyIndex;
    private string oldBindingJson;
    private string prevBindingJson;
    private Sequence currentSequence;

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
                {
                    Debug.LogWarning("Selected Object is not keybind");
                    return;
                }
                currentSequence?.Kill();
                bindingDescription.text = "Changing rebind for: " + selectedKeyDisplay.actionName;
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            //DetectDuplicateKeyF();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RefreshBindingDisplay();
        }
    }

    /// <summary>
    /// Get the keybindpage with the mapName
    /// </summary>
    /// <param name="mapName"></param>
    public void LoadActionMapKeybindPage(string mapName)
    {
        ActionButtonPair.Clear();
        keyButtonPairs.Clear();
        if(actionMapNameDisplay)
            actionMapNameDisplay.text = mapName;

        currentActionMap = inputActions.FindActionMap(mapName);
        if (currentActionMap != null)
        {
            //oldBindingJson = currentActionMap.ToJson(); ToJson returns the default one, not the override
            oldBindingJson = currentActionMap.SaveBindingOverridesAsJson();
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
        RefreshBindingDisplay();
    }

    /// <summary>
    /// Refresh all binding displays
    /// </summary>
    public void RefreshBindingDisplay()
    {
        keyButtonPairs.Clear();
        string bindingText = null;
        foreach (InputAction action in currentActionMap.actions)
        {
            if (action.bindings.Count > 1)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    int bindingIndex = action.bindings.IndexOf(b => b == binding);
                    if (ActionButtonPair.ContainsKey(binding.name))
                    {
                        bindingText = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                        ActionButtonPair[binding.name].buttonText.text = bindingText;
                        keyButtonPairs[bindingText] = ActionButtonPair[binding.name];
                    }
                    else
                    {
                        //Debug.LogWarning("Composite set to default failed: " + binding.name);
                    }
                }
            }
            else
            {
                if (ActionButtonPair.ContainsKey(action.name))
                {
                    //***hard code 0, change later
                    bindingText = action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                    ActionButtonPair[action.name].buttonText.text = bindingText;
                    keyButtonPairs[bindingText] = ActionButtonPair[action.name];
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
                            KeybindButton generateKey = displayItem.GenerateButton(action.name, bindingText, binding.name, Composite_Keybind_Tag);
                            ActionButtonPair[binding.name] = generateKey;
                            keyButtonPairs[bindingText] = generateKey;
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
                KeybindButton generateKey = displayItem.GenerateButton(null, action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontIncludeInteractions), action.name, Single_Keybind_Tag);
                ActionButtonPair[action.name] = generateKey;
                keyButtonPairs[bindingText] = generateKey;
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
        prevBindingJson = changingAction.SaveBindingOverridesAsJson();

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
    /// Íê³ÉæIÎ»ÔOÖÃ•r Dispose·ÀÖ¹leak
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
        bindingDescription.text = "";
        //there are key conflicts
        if (keyButtonPairs.ContainsKey(bindingText))
        {
            if(keyButtonPairs[bindingText] == selectedKeyDisplay)
                return;
            changingAction.LoadBindingOverridesFromJson(prevBindingJson);
            KeyConflictAnimation(keyButtonPairs[bindingText]);
            Debug.LogWarning("Already contains key use " + bindingText + " for " + keyButtonPairs[bindingText].actionName);
            bindingDescription.text = "\"" + bindingText + "\" key already use for " + keyButtonPairs[bindingText].actionName;
            currentSequence?.Kill();
            currentSequence = DOTween.Sequence()
                .AppendInterval(3f) // Wait for 5 seconds
                .AppendCallback(() => bindingDescription.text = "");
            return;
        }

        keyButtonPairs.Remove(selectedKeyDisplay.buttonText.text);
        keyButtonPairs[bindingText] = selectedKeyDisplay;
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

    private void KeyConflictAnimation(KeybindButton key)
    {
        key.buttonImage.DOColor(Color.red, 0.5f)
        .OnComplete(() =>
        {
            key.buttonImage.DOColor(Color.white, 0.5f);
        });
    }
}
