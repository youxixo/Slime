using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static InputActionAsset inputActions; // Reference to the InputActionAsset
    [SerializeField] private InputActionAsset inputActionsReference;

    //這些action map可以移到UImanager統一管理?
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    //private static InputActionMap keybindMap;

    private void Awake()
    {
        inputActions = inputActionsReference;
        DontDestroyOnLoad(this);
        // Get references to action maps
        playerActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");

        PlayerMove.pauseGame.AddListener(PauseGame);
        UIController.resumeGameEvent.AddListener(ResumeGame);
    }

    private int GetActiveActionMapCount()
    {
        int activeCount = 0;
        if (inputActions != null)
        {

            foreach (var actionMap in inputActions.actionMaps)
            {
                if (actionMap.enabled)
                {
                    activeCount++;
                }
                if(activeCount > 1)
                {
                    return activeCount;
                }
            }
        }
        return activeCount;
    }

    private void Update()
    {
        if(GetActiveActionMapCount() > 1)
        {
            //好像很容易有問題 因為player input component會在一開始 active所有 action map很煩
            ActivateActionMap("Player");
        }
        Debug.Log(inputActions.FindActionMap("Map").enabled);
        Debug.Log(inputActions.FindActionMap("Player").enabled);
        Debug.Log(inputActions.FindActionMap("UI").enabled);
    }


    private void Start()
    {
        ActivateActionMap("Player");
    }

    

    //暫停遊戲
    private void PauseGame()
    {
        // 切換input action map來限制玩家輸入
        ActivateActionMap("UI");
        Time.timeScale = 0f;
    }

    //繼續遊戲
    private void ResumeGame()
    {
        // 切換input action map來限制玩家輸入
        ActivateActionMap("Player");
        Time.timeScale = 1.0f;
    }


    public static void ActivateActionMap(string targetMapName)
    {
        // 停用所有的 ActionMap
        foreach (var map in inputActions.actionMaps)
        {
            if (map.name == targetMapName)
            {
                Debug.Log(map.name + " activing");
                map.Enable(); // 啟用目標 ActionMap
            }
            else
            {
                Debug.Log(map.name + "de activing");
                map.Disable(); // 停用其他 ActionMap
            }
        }
    }
}
