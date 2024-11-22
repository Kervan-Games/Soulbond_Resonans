using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DieMenu : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    public void RetryButton()
    {
        //gameObject.SetActive(false);
        Time.timeScale = 1f;
        //playerMovement.SetIsPaused(false);
        //playerMovement.SetDepthOfField(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitButton()
    {
        //gameObject.SetActive(false);
        Time.timeScale = 1f;
        //playerMovement.SetIsPaused(false);
        //playerMovement.SetDepthOfField(false);
        SceneManager.LoadScene(0);
    }

}
