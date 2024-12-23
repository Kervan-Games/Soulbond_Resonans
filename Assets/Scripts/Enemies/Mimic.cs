using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    private bool hasVisual = false;
    private bool isStunned = false;

    private float chaseSpeed = 14f;
    private Animator animator;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;
    private Collider2D _collider;
    public Collider2D physCollider;

    private GameObject player;
    private Vector3 playerPosition;

    private bool canKill = true;
    private bool didHit = false;
    private bool isPassive = false;

    public GameObject boxVisual;
    public GameObject vision;
    public ParticleSystem passiveParticle;

    [SerializeField] private float acceleration = 0.85f;

    private float speed;

    private Vector2 targetVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        if (_collider == null) Debug.LogError("AAAÐ");
        initialColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        playerPosition = player.transform.position;
        //Debug.Log(playerPosition.x - transform.position.x);

        if (hasVisual && !isStunned && !didHit && !isPassive)
        {
            StartRunning();
        }
        else
        {
            SmoothStop();
        }

        /*if (!isPassive && !didHit && isRunning && Mathf.Abs(rb.velocity.x) <= 0.01f)
        {
            GetStunned();
        }*/

        if (didHit && !isPassive)
        {
            BecomePassive();
            isPassive = true;
        }
        UpdateAnimation();

    }

    private void SmoothStop()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 2f * Time.deltaTime);
        if (rb.velocity.magnitude < 0.01f)
        {
            rb.velocity = Vector2.zero;
        }
    }

    /*private void StartRunning()
    {
        if(playerPosition.x - transform.position.x > 0)
        {
            rb.velocity = new Vector3(chaseSpeed, 0f, 0f);
        }
        else if (playerPosition.x - transform.position.x < 0)
        {
            rb.velocity = new Vector3(-chaseSpeed, 0f, 0f);
        }
    }*/

    private void StartRunning()
    {
        float direction = playerPosition.x - transform.position.x;

        if (direction > 0)
        {
            targetVelocity = new Vector2(chaseSpeed, 0f); 
        }
        else if (direction < 0)
        {
            targetVelocity = new Vector2(-chaseSpeed, 0f); 
        }
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.deltaTime);
    }

    private void GetStunned()
    {
        rb.velocity = Vector3.zero;
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
                collision.gameObject.GetComponent<PlayerMovement>().SetAnimatorIsDeadTrue();
            }
        }
        else if (collision.CompareTag("StrongSpirit") || collision.CompareTag("Spirit"))
        {
            didHit = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canKill)
            {
                collision.gameObject.GetComponent<PlayerMovement>().SetAnimatorIsDeadTrue();
            }
        }
    }

    public void SetHasVisual(bool hasVisual_)
    {
        hasVisual = hasVisual_;
    }

    private void CancelStun()
    {
        if (!didHit)
        {
            isStunned = false;
            Debug.Log("Active again!");
            _collider.isTrigger = true;
            rb.isKinematic = false;
        }
    }

    private void BecomePassive()
    {
        rb.velocity = Vector3.zero;
        isStunned = true;

        rb.isKinematic = true;    

        if(rb.isKinematic == true)
        {
            rb.isKinematic = false;
        }


        rb.gravityScale = 1f;
        _collider.isTrigger = false;
        //Debug.Log("Passive!");
        boxVisual.SetActive(true);
        vision.SetActive(false);
        passiveParticle.Play();

        canKill = false;
        _collider.isTrigger = true;
        physCollider.isTrigger = true;
        spriteRenderer.enabled = false;
    }

    public void SetDidHit(bool hitt)
    {
        didHit = hitt;
    }

    void UpdateAnimation()
    {
        speed = rb.velocity.magnitude;
        animator.speed = speed / chaseSpeed;

        if (transform.position.x > playerPosition.x) /*if(rb.velocity.x < 0f && canKill)*/
        {
            spriteRenderer.flipX = false;
        }
        else /*if(rb.velocity.x > 0f && canKill)*/
        {
            spriteRenderer.flipX = true;
        }
        


    }
}
