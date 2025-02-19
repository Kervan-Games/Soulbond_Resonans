using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private TutorialCanvas tutorialCanvas;

    [TextArea] public string tutorialMessage;

    private void Start()
    {
        tutorialCanvas = FindObjectOfType<TutorialCanvas>();

        if (tutorialCanvas == null)
        {
            //Debug.LogError("TutorialCanvas script is not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && tutorialCanvas != null)
        {
            tutorialCanvas.SetTutorialText(tutorialMessage);
            tutorialCanvas.ShowTutorialText(true);
        }
    }
}
