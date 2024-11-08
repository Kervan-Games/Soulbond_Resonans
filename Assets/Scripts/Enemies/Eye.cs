using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Eye : MonoBehaviour
{
    private Animator animator;
    private bool isClosed = false;

    public GameObject vision;
    private bool canBlink = true;

    public SpriteRenderer visionSpriteRenderer;
    private GameObject player;
    public float moveSpeed = 10f;
    private bool canChase = true;

    private PlayerHide playerHide;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        playerHide = player.GetComponent<PlayerHide>();
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
            yield return new WaitForSeconds(2f);
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
        else if (collision.CompareTag("Player") && collision != null)
        {
            collision.gameObject.GetComponent<PlayerMovement>().SetAnimatorIsDeadTrue();
            canChase = false;
        }
    }

    public void MoveTowardsPlayer()
    {
        if (canChase && playerHide.GetIsHiding() == false)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            animator.SetBool("canChase", true);
        }
        else
        {
            animator.SetBool("canChase", false);
        }
    }

    public void RotateTowardsPlayer()
    {
        if (canChase && playerHide.GetIsHiding() == false)
        {
            Vector2 direction = player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
            {
                angle += 180f;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void AnimatorSetChaseTrigger()
    {
        animator.SetTrigger("chase");
    }

    public void SetCanChase(bool can)
    {
        canChase = can;
    }

}
