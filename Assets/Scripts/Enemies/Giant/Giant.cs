using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : MonoBehaviour
{
    private GameObject playerObj;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    public float moveSpeed = 5f;
    private float chaseRange = 20f;
    private float stoppingRange = 3f;
    private float distance;
    private float attackDamage = 25f;

    private float stunTime = 2.2f;

    private bool isAttacking = false;

    private bool isStunned = false;
    private bool isFacingRight = true;

    public float parryPushStrength = 75f;

    private bool isDead = false;

    private bool isCharging = false;
    private bool canCharge = true;
    private float chargeRangeMin = 8f;
    private float chargeRangeMax = 15f;
    private bool reverse = false;
    private bool chasingRight = false;

    private Coroutine stopCoroutine;
    private Coroutine activateCoroutine;
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
            if (isCharging)
            {
                if (chasingRight)
                {
                    rb.velocity = new Vector2(moveSpeed * 2f, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-moveSpeed * 2f, rb.velocity.y);
                }

                if (reverse && stopCoroutine == null)
                {
                    stopCoroutine = StartCoroutine(StopCharging());
                }
            }
            else if (distance < chaseRange && distance > chargeRangeMax && !isAttacking && !isStunned && !isDead && !isCharging && canCharge)
            {
                MoveToPlayer();
            }
            else if (distance <= chargeRangeMax && distance > chargeRangeMin && !isCharging && !isStunned && canCharge && !isAttacking)
            {
                isCharging = true;
                canCharge = false;
                Debug.Log("Charging");
                //StartCoroutine(StopCharging());
            }
            else if (distance <= chargeRangeMin && distance > stoppingRange && !isAttacking && !isStunned && !isDead && !isCharging && canCharge)
            {
                MoveToPlayer();
            }

            /*if (distance < chargeRangeMax && distance > chargeRangeMin && !isCharging && !isStunned)
            {
                MoveToPlayer();
            }
            else if (isStunned)
            {
                //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }*/
        }
    }

    private void Update()
    {
        distance = Vector3.Distance(playerObj.transform.position, gameObject.transform.position);
        //Debug.Log(distance);
        if(transform.position.x < playerTransform.position.x && !isCharging)
        {
            chasingRight = true;
        }
        else if(!isCharging)
        {
            chasingRight = false;
        }


        if (!isStunned && !isCharging && canCharge)
        {
            HandleFlipping();
        }

        if (distance <= stoppingRange && !isAttacking && !isStunned && !isCharging && canCharge)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isAttacking = true;
            Debug.Log("Charging, get ready to parry!");
            StartCoroutine(Attack());
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
            playerHealth.TakeDamage(attackDamage*2);
            playerMovement.TakeDamagePush(isFacingRight, true);
            playerMovement.EnemyColliderOff();
        }
    }

    private IEnumerator StopCharging()
    {
        yield return new WaitForSeconds(0.25f);
        isCharging = false;
        stopCoroutine = null;

        if(activateCoroutine == null)
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

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.4f);
        if (distance < 5f)
        {
            if ((transform.position.x > playerTransform.position.x && !isFacingRight) || (transform.position.x < playerTransform.position.x && isFacingRight))
            {
                //Debug.Log("DAMAGE!!");
                playerHealth.TakeDamage(attackDamage);
                playerMovement.TakeDamagePush(isFacingRight, false);
            }
        }
        isAttacking = false;
        canCharge = true;
        isCharging = false;
    }

    private void MoveToPlayer()
    {
        Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, 0).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void HandleFlipping()
    {
        if (!isAttacking)
        {
            if (transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(-2, 2, 2);
                isFacingRight = false;
            }
            else
            {
                transform.localScale = new Vector3(2, 2, 2);
                isFacingRight = true;
            }
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
