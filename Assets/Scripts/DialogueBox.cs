using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueBox : MonoBehaviour
{
    public GameObject box;
    public GameObject dialogue1;
    public GameObject dialogue2;
    public GameObject dialogue3;
    private int dialogueNumber = 0;
    public PlayerMovement playerMovement;

    public InteractionCamera interactionCamera;

    private TutorialCanvas tutorialCanvas;

    void Start()
    {
        tutorialCanvas = FindObjectOfType<TutorialCanvas>();
    }

    private void Update()
    {
        if (dialogueNumber == 1)
        {
            tutorialCanvas.ShowTutorialText(false);
            box.SetActive(true);
            dialogue1.SetActive(true);
            dialogue2.SetActive(false);
            dialogue3.SetActive(false);
        }
        else if (dialogueNumber == 2)
        {
            box.SetActive(true);
            dialogue1.SetActive(false);
            dialogue2.SetActive(true);
            dialogue3.SetActive(false);
        }
        else if (dialogueNumber == 3)
        {
            box.SetActive(true);
            dialogue1.SetActive(false);
            dialogue2.SetActive(false);
            dialogue3.SetActive(true);
        }
        else if(dialogueNumber > 3) 
        {
            box.SetActive(false);
            dialogue1.SetActive(false);
            dialogue2.SetActive(false);
            dialogue3.SetActive(false);
            interactionCamera.SetCanInteract(false);
            interactionCamera.SetIsInteracting(false);
            playerMovement.SetIsInDialogue(false);
            dialogueNumber = 0;
        }
    }

    public void IncreaseDialogueNumber()
    {
        dialogueNumber++;
    }
}
