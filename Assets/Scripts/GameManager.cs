using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputActionAsset inputActions; // Reference to the InputActionAsset

    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    private void Awake()
    {

        // Get references to action maps
        playerActionMap = inputActions.FindActionMap("Player");
        uiActionMap = inputActions.FindActionMap("UI");

        PlayerMove.pauseGame.AddListener(PauseGame);
        UIController.resumeGameEvent.AddListener(ResumeGame);
    }

    private void Start()
    {
        ResumeGame(); //開局先換成Player Input Action Map
    }

    private void PauseGame()
    {
        // 切換input action map來限制玩家輸入
        uiActionMap.Enable();
        playerActionMap.Disable();

        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        // 切換input action map來限制玩家輸入
        playerActionMap.Enable();
        uiActionMap.Disable();

        Time.timeScale = 1.0f;
    }
}
