using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKeybind : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    InputAction moveAction;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void Start()
    {
        playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
    }

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

    public void SingleKeyBind(string actionMapName)
    {
        InputAction changingAction = playerActionMap.FindAction(actionMapName);

        _rebindingOperation = changingAction.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind())
            .Start();
    }

    public void MoveKeyRebind(string direction)
    {
        int bindingIndex = FindMoveBindingIndex(direction.ToLower());

        _rebindingOperation = moveAction.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind())
            .Start();
    }

    private void CompleteRebind()
    {
        _rebindingOperation.Dispose();
    }
}
