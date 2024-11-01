using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    private Animator animator;
    private bool isClosed = false;

    public GameObject vision;
    private bool canBlink = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ToggleEyeState());
    }

    private IEnumerator ToggleEyeState()
    {
        while (true)
        {
            isClosed = !isClosed;
            if (canBlink)
            {
                animator.SetBool("isClosed", isClosed);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public void OpenVision()
    {
        vision.SetActive(true);
        //Debug.Log("Opened");
    }

    public void CloseVision()
    {
        vision?.SetActive(false);
        //Debug.Log("Closed");
    }

    public void StopCoroutineBlink()
    {
        canBlink = false;
    }

}
