using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    private bool hasVisual = false;
    private bool isRunning = false;
    private bool isStunned = false;

    private float chaseSpeed = 7.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;
    private Collider2D _collider;

    private GameObject player;
    private Vector3 playerPosition;

    private bool canKill = true;
    private bool didHit = false;
    private bool isPassive = false;

      // ***************** //
     // ADD SPIRIT THWROW //
    // ***************** //

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        if (_collider == null) Debug.LogError("AAAÐ");
        initialColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player"); 
    }

    private void Update()
    {
        playerPosition = player.transform.position;
        //Debug.Log(playerPosition.x - transform.position.x);

        if (hasVisual && !isRunning && !isStunned && !didHit && !isPassive)
        {
            StartRunning();
            isRunning = true;
        }

        if (!isPassive && !didHit && isRunning && Mathf.Abs(rb.velocity.x) <= 0.1f)
        {
            GetStunned();
        }

        if (didHit && !isPassive)
        {
            BecomePassive();
            isPassive = true;
        }

    }

    private void StartRunning()
    {
        if(playerPosition.x - transform.position.x > 0)
        {
            rb.velocity = new Vector3(chaseSpeed, 0f, 0f);
        }
        else if (playerPosition.x - transform.position.x < 0)
        {
            rb.velocity = new Vector3(-chaseSpeed, 0f, 0f);
        }
    }

    private void GetStunned()
    {
        isRunning = false;
        isStunned = true;
        rb.isKinematic = true;
        Debug.Log("Stunned!");
        _collider.isTrigger = false;
        Invoke("CancelStun", 2f);
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canKill)
            {
                collision.gameObject.GetComponent<PlayerMovement>().Die();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canKill)
            {
                collision.gameObject.GetComponent<PlayerMovement>().Die();
            }
        }
    }

    public void SetHasVisual(bool hasVisual_)
    {
        hasVisual = hasVisual_;
    }

    private void CancelStun()
    {
        isStunned = false;
        Debug.Log("Active again!");
        _collider.isTrigger = true;
        rb.isKinematic = false;
    }

    private void BecomePassive()
    {
        isRunning = false;
        isStunned = true;
        rb.isKinematic = true;
        _collider.isTrigger = false;
        Debug.Log("Passive!");
    }
}
