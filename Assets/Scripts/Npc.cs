using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    private int maxIndex;
    private int currentIndex = -1;
    [SerializeField] private int repeatingIndex;
    private bool repeat;
    [SerializeField] private List<string> dialogues = new List<string>();
    
    private void InitDialogue()
    {
        //very first initialize
        if(currentIndex == -1)
        {
            maxIndex = dialogues.Count - 1;
            currentIndex = 0;
        }
        else if(!repeat)
        {
            currentIndex += 1;
            if(currentIndex > maxIndex)
            {
                currentIndex = repeatingIndex;
                repeat = true;
            }
        }
    }

    public void StartDialogue()
    {
        InitDialogue();
        DialogueManager.startDialogue.Invoke(dialogues[currentIndex]);
    }
}
