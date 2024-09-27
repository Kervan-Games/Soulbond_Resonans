using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    private bool isInDialogue = false;
    public DialogueBox box;

    public void SetIsInDialogue(bool inD)
    {
        isInDialogue = inD;
    }

    public void OnDialoguePressed(InputAction.CallbackContext context)
    {
        if (context.performed && isInDialogue)
        {
            //Debug.Log("INTERACTED!");
            if (box != null)
                box.IncreaseDialogueNumber();
            else
                Debug.LogError("Box null");
        }
    }
}
