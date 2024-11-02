using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    private Animator animator;
    private bool isClosed = false;

    public GameObject vision;
    private bool canBlink = true;

    public SpriteRenderer visionSpriteRenderer;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StrongSpirit") || collision.CompareTag("Spirit"))
        {
            canBlink = false;
            visionSpriteRenderer.color = Color.green;
            Color visionColor = visionSpriteRenderer.color;
            visionColor.a = 0.5f;
            visionSpriteRenderer.color = visionColor;
            animator.SetBool("isClosed", true);

        }
    }

}
