using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputActionAsset inputActions; // Reference to the InputActionAsset

    //這些action map可以移到UImanager統一管理?
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    //private static InputActionMap keybindMap;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        // Get references to action maps
        playerActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");
        inputActions.FindActionMap("Map").Disable();
        //keybindMap = inputActions.FindActionMap("Keybind");

        PlayerMove.pauseGame.AddListener(PauseGame);
        UIController.resumeGameEvent.AddListener(ResumeGame);
    }

    private void Start()
    {
        ResumeGame(); //開局先換成Player Input Action Map
    }

    //暫停遊戲
    private void PauseGame()
    {
        // 切換input action map來限制玩家輸入
        uiActionMap.Enable();
        playerActionMap.Disable();

        Time.timeScale = 0f;
    }

    //繼續遊戲
    private void ResumeGame()
    {
        // 切換input action map來限制玩家輸入
        playerActionMap.Enable();
        uiActionMap.Disable();
        Debug.Log("resume");
        Time.timeScale = 1.0f;
    }
}
