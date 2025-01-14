using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private bool inNpc = false;
    private string interactableTag;
    private Npc npc;
    private Dictionary<string, Action<InputAction.CallbackContext, GameObject>> interactPair;
    private GameObject interactObject;
    private PlayerSave save;

    private void Start()
    {
        InitDict();

        save = this.GetComponent<PlayerSave>();
    }

    public void ExecuteInteraction(InputAction.CallbackContext context)
    {
        if (interactObject != null)
        {
            if (interactPair.TryGetValue(interactableTag, out var action))
            {
                action.Invoke(context, interactObject); // Call the associated method with the context
            }
            else
            {
                Debug.LogWarning($"No interaction found for key: {interactableTag}");
            }
        }
    }

    private void InitDict()
    {
        interactPair = new Dictionary<string, Action<InputAction.CallbackContext, GameObject>>
        {
            { "Npc", TalkToNpc },
            { "SavePoint", SaveGame },
        };
    }

    public void TalkToNpc(InputAction.CallbackContext context, GameObject collision)
    {
        collision.TryGetComponent<Npc>(out npc);
        if (npc == null)
        {
            Debug.LogWarning("no npc script");
        }
        if (context.started)//&& inNpc)
        {
            GameManager.ActivateActionMap("Npc");
            npc.StartDialogue();
        }
    }

    private void SaveGame(InputAction.CallbackContext context, GameObject collision)
    {
        save.Save(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "DeathPoint")
        {
            save.PlayerDataLoad();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //interactable layer
        if (collision.gameObject.layer == 11 && interactObject == null)
        {
            collision.gameObject.transform.Find("UI").gameObject.SetActive(true);
            //可以創一個string var 在playerkeybind那邊在設置時再修改這邊的string才不用每次調用
            collision.gameObject.transform.Find("UI/KeyHint/Key Text").GetComponent<TMP_Text>().text = inputActions.FindActionMap("Player").FindAction("Interact").GetBindingDisplayString(0);
            interactableTag = collision.tag;
            interactObject = collision.gameObject;
            Debug.Log(collision.tag);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //interactable layer
        if (collision.gameObject.layer == 11)
        {
            collision.gameObject.transform.Find("UI").gameObject.SetActive(false);
            interactObject = null;
        }
    }
}
