using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("對話框")]
    private Tween _typeWriter; //打字機效果s
    [SerializeField] private TMP_Text UITextMeshPro;

    private string _dialogue;
    private float duration;
    [SerializeField] private float _userChosenSpeed = 50;
    private bool skipping = false;
    private GameObject child;
    public static UnityEvent<string> startDialogue = new UnityEvent<string>();
    [SerializeField] private InputActionAsset inputActions;
    private InputAction nextAction;

    private void Start()
    {
        child = this.transform.GetChild(0).gameObject;
        startDialogue.AddListener(InitDialogue);
        nextAction = inputActions.FindActionMap("Npc").FindAction("Next");
    }

    private void LoadMessage(string dialogue) //, float duration)
    {
        UITextMeshPro.maxVisibleCharacters = 0;
        UITextMeshPro.text = dialogue;

        UITextMeshPro.ForceMeshUpdate();
        _dialogue = UITextMeshPro.GetParsedText();
        if (!skipping)
            duration = 1 / _userChosenSpeed * _dialogue.Length;
        else
            duration = 1 / (_userChosenSpeed + 20) * _dialogue.Length;

        _typeWriter = DOTween.To(value => OnUpdate(value), 0, 1, duration).SetEase(Ease.Linear).SetAutoKill(false);
    }

    private void OnUpdate(float value)
    {
        var current = Mathf.Lerp(0, _dialogue.Length, value);
        var count = Mathf.FloorToInt(current);
        UITextMeshPro.maxVisibleCharacters = count;
    }

    private void Update()
    {
        //如果npc action map 的那啥被調用
        if(nextAction.triggered)
        {
            if (!_typeWriter.IsComplete())
            {
                _typeWriter.Complete();
            }
            else if (currentIndex > maxDialogueIndex)
            {
                FinishDialogue();
                return;
            }
            else if (_typeWriter.IsComplete())
            {
                LoadMessage(splitDialogue[currentIndex]);
                currentIndex++;
            }
        }
    }

    private void FinishDialogue()
    {
        child.SetActive(false);
        GameManager.ActivateActionMap("Player"); //***game manager可能可以存一個previous action map來方便切換 更有拓展性
    }

    private string[] splitDialogue;
    private int maxDialogueIndex;
    private int currentIndex;

    private void InitDialogue(string dialogue)
    {
        UITextMeshPro.text = "";
        child.SetActive(true);
        splitDialogue = dialogue.Split("//");
        maxDialogueIndex = splitDialogue.Length - 1;
        currentIndex = 0;
        LoadMessage(splitDialogue[currentIndex]);
        currentIndex++;
    }
}
