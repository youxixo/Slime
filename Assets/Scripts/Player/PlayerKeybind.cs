using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
/// <summary>
/// 鍵位設置
/// 目前還要加 退出時保存用戶設置道playerKetset
/// Bug: 使用enter點進keybind page 會被判定為按下enter所以一進來就會開始rebind (已完成 修復辦法是一進場的一小段時間不判定rebind防止誤觸)
/// </summary>
public class PlayerKeybind : MonoBehaviour
{
    public InputActionAsset inputActions; 
    private InputActionMap playerActionMap; //player的action map
    private InputActionMap uiActionMap; //ui的action map
    private InputAction moveAction; //移動的action

    [SerializeField] private TMP_Text keybindDescription; //顯示一些說明的文字
    [SerializeField] private GameObject firstButton; //進場時第一個focus的按鈕
    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation; //rebind

    [Header("Keybind Display")] //每個按鈕的字
    [SerializeField] private TMP_Text up;
    [SerializeField] private TMP_Text down;
    [SerializeField] private TMP_Text left;
    [SerializeField] private TMP_Text right;
    [SerializeField] private TMP_Text jump;
    [SerializeField] private TMP_Text dash;

    [Header("Default Keybind")]
    [SerializeField] private Button defaultWASDButton; //預設按鈕1
    [SerializeField] private Button defaultArrowButton; //預設按鈕2
    private float firstCooldown = 0.2f; //防止剛入場的enter誤觸
    private bool cooldownPast = false;
    private Keybind defaultArrow = new Keybind
    {
        up = "<Keyboard>/upArrow",
        down = "<Keyboard>/downArrow",
        left = "<Keyboard>/leftArrow",
        right = "<Keyboard>/rightArrow",
        jump = "<Keyboard>/space",
        dash = "<Keyboard>/leftShift"
    };
    private Keybind defaultWASD = new Keybind
    {
        up = "<Keyboard>/w",
        down = "<Keyboard>/s",
        left = "<Keyboard>/a",
        right = "<Keyboard>/d",
        jump = "<Keyboard>/j",
        dash = "<Keyboard>/k"
    };
    private Keybind playerKeyset; //存玩家的鍵位

    private void Start()
    {
        defaultWASDButton.onClick.AddListener(() => LoadDefault(defaultWASD));
        defaultArrowButton.onClick.AddListener(() => LoadDefault(defaultArrow));
    }

    //加載預設鍵位
    private void LoadDefault(Keybind key)
    {
        playerActionMap.FindAction("move").ApplyBindingOverride(FindMoveBindingIndex("left"), key.left);
        playerActionMap.FindAction("move").ApplyBindingOverride(FindMoveBindingIndex("right"), key.right);
        playerActionMap.FindAction("move").ApplyBindingOverride(FindMoveBindingIndex("up"), key.up);
        playerActionMap.FindAction("move").ApplyBindingOverride(FindMoveBindingIndex("down"), key.down);
        playerActionMap.FindAction("jump").ApplyBindingOverride(key.jump);
        playerActionMap.FindAction("sprint").ApplyBindingOverride(key.dash);
        Debug.Log("change key bind");
        UpdateKeyVisual();
    }

    private void OnEnable()
    {
        keybindDescription.text = "Press enter on the selected key to rebind.";
        playerActionMap = inputActions.FindActionMap("Player");
        //playerActionMap.Disable();
        moveAction = playerActionMap.FindAction("Move");
        //inputActions.FindActionMap("UI").Enable();
        GameManager.ActivateActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstButton);
        UpdateKeyVisual();
        StartCoroutine(EnableSubmitAfterCooldown());
    }

    private void OnDisable()
    {
        cooldownPast = false;
    }

    private IEnumerator EnableSubmitAfterCooldown()
    {
        yield return new WaitForSeconds(firstCooldown);
        cooldownPast = true;
    }

    private void Update()
    {
        if(!cooldownPast)
        {
            return;
        }
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
