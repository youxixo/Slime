using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerKeybind : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    InputAction moveAction;

    [SerializeField] private TMP_Text keybindDescription;
    [SerializeField] private GameObject firstButton;
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    [Header("Keybind Display")]
    [SerializeField] private TMP_Text up;
    [SerializeField] private TMP_Text down;
    [SerializeField] private TMP_Text left;
    [SerializeField] private TMP_Text right;
    [SerializeField] private TMP_Text jump;
    [SerializeField] private TMP_Text dash;

    private void OnEnable()
    {
        keybindDescription.text = "Press enter on the selected key to rebind.";
        playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        uiActionMap = inputActions.FindActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstButton);
        UpdateKeyVisual();
    }

    private void Update()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (selectedObject != null && selectedObject.tag == "Keybind")
        {
            if (Input.GetButtonDown("Submit"))
            {
                keybindDescription.text = "Press a key to set for " + selectedObject.name;
                //非常蠢 之後改改
                if (selectedObject.name == "up" || selectedObject.name == "down" || selectedObject.name == "left" || selectedObject.name == "right")
                {
                    keybindDescription.text = "Press a key to set for " + selectedObject.name + " movement";
                    MoveKeyRebind(selectedObject.name);
                }
                else
                {
                    SingleKeyBind(selectedObject.name);
                }
            }
        }
    }

    //更新顯示 - 還可以優化 只更新剛剛更改的鍵位
    private void UpdateKeyVisual()
    {
        jump.text = playerActionMap.FindAction("jump").GetBindingDisplayString(0); 
        dash.text = playerActionMap.FindAction("sprint").GetBindingDisplayString(0);
        up.text = moveAction.GetBindingDisplayString(FindMoveBindingIndex("up"));
        down.text = moveAction.GetBindingDisplayString(FindMoveBindingIndex("down"));
        left.text = moveAction.GetBindingDisplayString(FindMoveBindingIndex("left"));
        right.text = moveAction.GetBindingDisplayString(FindMoveBindingIndex("right"));
    }

    //各移動的binding index
    private int FindMoveBindingIndex(string actionName)
    {
        for (int i = 0; i < moveAction.bindings.Count; i++)
        {
            if (moveAction.bindings[i].name == actionName)
            {
                return i;
            }
        }
        return -1;
    }

    //其他的keybind
    public void SingleKeyBind(string actionMapName)
    {
        InputAction changingAction = playerActionMap.FindAction(actionMapName);
        Debug.Log("start bind");
        _rebindingOperation = changingAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind())
            .Start();
    }

    //移動的keybind
    //可優化: instead of 一個一個改(上下左右分開改) 變成一次改四個
    //可參考: Change key bind, ChangeCoposite Binding
    //https://docs.unity3d.com/Packages/com.unity.inputsystem@1.3/manual/ActionBindings.html#changing-bindings
    public void MoveKeyRebind(string direction)
    {
        int bindingIndex = FindMoveBindingIndex(direction.ToLower());
        Debug.Log("start bind");
        _rebindingOperation = moveAction.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind())
            .Start();
    }

    //完成鍵位設置時 Dispose防止leak
    private void CompleteRebind()
    {
        keybindDescription.text = "Binding complete!";
        _rebindingOperation.Dispose();
        UpdateKeyVisual();
    }
}

public struct Keybind
{
    public string up;
    public string down;
    public string left;
    public string right;
    public string jump;
    public string dash;
}
