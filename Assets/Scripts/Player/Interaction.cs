using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private bool canInteract;
    private GameObject interactedObject;
    private GameObject eButton;
    private InteractionCamera interactionCamera;
    public PlayerMovement playerMovement;
    private Dialogue dialogue;

    private void Start()
    {
        canInteract = false;
        interactionCamera = GetComponent<InteractionCamera>();
        dialogue = GetComponent<Dialogue>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            //Debug.Log("Can Interact");
            canInteract = true;
            interactedObject = collision.gameObject;

            eButton = interactedObject.transform.Find("eButton").gameObject;

            if (eButton != null)
            {
                eButton.SetActive(true);
            }
            else
            {
                Debug.LogError("e Button is NULL!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            //Debug.Log("Can NOT Interact");

            if (eButton != null)
            {
                eButton.SetActive(false);
            }
            else
            {
                Debug.LogError("e Button is NULL!");
            }

            canInteract = false;
            interactedObject = null;
        }
    }

    public void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed && canInteract)
        {
            //Debug.Log("INTERACTED!");
            interactionCamera.SetIsInteracting(true);
            eButton.SetActive(false);
            playerMovement.SetIsInDialogue(true);
            dialogue.SetIsInDialogue(true);
            //interactedObject.GetComponent<*script*>().function();
            // or at the beginning check the tag of triggered object, then perform the getComponent and run function

        }
    }


}
