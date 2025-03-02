using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private GameObject playerObj;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    public float moveSpeed = 5f;
    private float chaseRange = 20f;
    private float stoppingRange = 3f;
    private float distance = 100f;
    private float attackDamage = 25f;

    private float stunTime = 2.2f;

    private bool isAttacking = false;

    private bool isStunned = false;
    private bool isFacingRight = true;

    public float parryPushStrength = 75f;

    private bool isDead = false;

    private bool isCharging = false;
    private bool canCharge = true;
    private float chargeRange = 6f;
    private bool reverse = false;
    private bool chasingRight = false;

    private Coroutine stopCoroutine;
    private Coroutine activateCoroutine;

    private bool preparing = false;

    private Vector2 chargeDirection;
    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerTransform = playerObj.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (isCharging) //charge
            {
                /*if (chasingRight)
                {
                    rb.velocity = new Vector2(moveSpeed * 2f, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-moveSpeed * 2f, rb.velocity.y);
                }*/

                rb.velocity = chargeDirection * moveSpeed * 2f;

                if (reverse && stopCoroutine == null)
                {
                    stopCoroutine = StartCoroutine(StopCharging());
                }
            }
            else if (preparing)
            {
                rb.velocity = Vector2.zero;
            }
            else if(distance >= chaseRange)
            {
                rb.velocity = Vector2.zero;
            }
            else if (distance < chaseRange && distance > chargeRange && !isAttacking && !isStunned && !isDead && !isCharging && canCharge)
            {
                MoveToPlayer();
                chaseRange *= 3;
            }
            else if (distance <= chargeRange && !isCharging && !isStunned && canCharge && !isAttacking && !preparing)
            {
                preparing = true;
                StartCoroutine(Prepare());
            }

        }
    }

    private IEnumerator Prepare()
    {
        yield return new WaitForSeconds(2f);
        preparing = false;
        chargeDirection = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
        isCharging = true;
        canCharge = false;
        Debug.Log("Charging");
    }

    private void Update()
    {
        distance = Vector3.Distance(playerObj.transform.position, gameObject.transform.position);
        //Debug.Log(distance);
        if (transform.position.x < playerTransform.position.x && !isCharging)
        {
            chasingRight = true;
        }
        else if (!isCharging)
        {
            chasingRight = false;
        }


        if (!isStunned && !isCharging && canCharge)
        {
            RotateTowardsPlayer();
        }


        if ((transform.position.x > playerTransform.position.x && !isFacingRight) || (transform.position.x < playerTransform.position.x && isFacingRight))
        {
            reverse = false;
        }
        else
        {
            reverse = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && isCharging)
        {
            playerHealth.TakeDamage(attackDamage * 2);
            playerMovement.TakeDamagePush(isFacingRight, true);
            playerMovement.EnemyColliderOff();
        }
    }

    private IEnumerator StopCharging()
    {
        yield return new WaitForSeconds(0.25f);
        isCharging = false;
        stopCoroutine = null;

        if (activateCoroutine == null)
        {
            activateCoroutine = StartCoroutine(ActivateCanCharge());
        }
    }

    private IEnumerator ActivateCanCharge()
    {
        yield return new WaitForSeconds(1f);
        canCharge = true;
        activateCoroutine = null;
    }

    

    private void MoveToPlayer()
    {
        // Move to look position
        /*Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
        rb.velocity = direction * moveSpeed;*/

        // Move to only +x or -x direction
        if (chasingRight)
        {
            if (Mathf.Abs(playerTransform.position.y - transform.position.y) > chargeRange -2f)
            {
                Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
                rb.velocity = direction * moveSpeed;
            }
            else
                rb.velocity = new Vector2(moveSpeed, 0);
        }


        else
        {
            if (Mathf.Abs(playerTransform.position.y - transform.position.y) > chargeRange -2f)
            {
                Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
                rb.velocity = direction * moveSpeed;
            }
            else
                rb.velocity = new Vector2(-moveSpeed, 0);
        }
    }

    private void HandleFlipping()
    {
        if (!isAttacking)
        {
            if (transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
                isFacingRight = false;
            }
            else
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                isFacingRight = true;
            }
        }
    }

    public void RotateTowardsPlayer()
    {
        if (!isCharging)
        {
            Vector2 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
            {
                angle += 180f;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void GetStunned()
    {
        isStunned = true;
        Debug.Log("Stunned!");
        if (isFacingRight)
        {
            Vector2 dir = new Vector2(-1f, 1f);
            rb.AddForce(dir * parryPushStrength, ForceMode2D.Impulse);
        }
        else if (!isFacingRight)
        {
            Vector2 dir = new Vector2(1f, 1f);
            rb.AddForce(dir * parryPushStrength, ForceMode2D.Impulse);
        }
        StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log("Active Again!");
    }

    public void SetIsDead(bool dead)
    {
        isDead = dead;
    }
}
