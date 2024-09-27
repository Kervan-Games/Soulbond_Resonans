using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;

    private void Start()
    {
        ShowTutorialText(false);
    }

    public void SetTutorialText(string newText)
    {
        if (tutorialText != null)
        {
            tutorialText.text = newText;
        }
        else
        {
            Debug.LogWarning("Tutorial text is not assigned!");
        }
    }

    public void ShowTutorialText(bool isActive)
    {
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Tutorial text is not assigned!");
        }
    }
}
