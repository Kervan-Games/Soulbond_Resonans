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
    private float moveSpeed = 5f;
    private float chaseRange = 20f;
    private float distance = 100f;
    private float attackDamage = 25f;

    private float stunTime = 2.2f;

    private bool isAttacking = false;

    private bool isStunned = false;
    private bool isFacingRight = true;

    public float parryPushStrength = 20f;

    private bool isDead = false;

    private bool isCharging = false;
    private bool canCharge = true;
    private float chargeRange = 8f;
    private bool chasingRight = false;

    private Coroutine stopCoroutine;
    private Coroutine activateCoroutine;
    private Coroutine prepareCoroutine;
    private bool preparing = false;

    private Vector2 chargeDirection;

    public float chargeDuration = 0.5f;
    public float chargeSpeed = 10f;

    private Collider2D collider2d;
    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerTransform = playerObj.transform;
        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        //Debug.Log(isCharging);
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

                rb.velocity = chargeDirection * chargeSpeed;

                if (stopCoroutine == null)
                {
                    stopCoroutine = StartCoroutine(StopCharging());
                }
            }
            else if (preparing)
            {
                rb.velocity = Vector2.zero;
            }
            /*else if (preparing && distance < chargeRange - 0.5f)  ////////////////////////////////////////////////////////////////////////////////
            {
                //rb.velocity = Vector2.zero;                       //////////////////////////////////////////////////////////////////////////////// SALDIRIYA HAZIRLANIRKEN DE PLAYER'I TAKIP ETMESINI SAGLAR
                MoveFromPlayer();
            }                                                                                                                                           EGER ACARSAN PREPARE SURESINI AZALT
            else if (preparing && distance > chargeRange + 0.5f)        //////////////////////////////////////////////////////////////////////////////// SOR VE ONA GORE AC 
            {
                MoveToPlayer();
            }
            else if(preparing && distance >= chargeRange - 0.5f && distance <= chargeRange + 0.5f)
            {
                rb.velocity = Vector2.zero;
            }*/
            else if (distance >= chaseRange)
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
                prepareCoroutine = StartCoroutine(Prepare());
            }
            else if (distance < chargeRange - 0.5f && !canCharge && !isStunned)
            {
                MoveFromPlayer();
            }
            else if (distance > chargeRange + 0.5f && !canCharge && !isStunned)
            {
                MoveToPlayer();
            }
            else if (distance >= chargeRange - 0.5f && distance <= chargeRange + 0.5f)
            {
                rb.velocity = Vector2.zero;
            }
            /*else if(distance < 2f && !isCharging) 
            { 
                if(prepareCoroutine != null)
                {
                    StopCoroutine(prepareCoroutine);
                    prepareCoroutine = null;
                }
                isCharging = true;
            }*/

        }
    }

    private IEnumerator Prepare()
    {
        yield return new WaitForSeconds(2f);
        preparing = false;
        chargeDirection = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
        isCharging = true;
        canCharge = false;
        prepareCoroutine = null;
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


        if (!isStunned && !isCharging)
        {
            RotateTowardsPlayer();
        }


        /*if ((transform.position.x > playerTransform.position.x && !isFacingRight) || (transform.position.x < playerTransform.position.x && isFacingRight))
        {
            reverse = false;
        }
        else
        {
            reverse = true;
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isCharging)
        {
            if (isCharging)
            {
                if (playerMovement.GetIsParrying() == false && playerMovement.GetIsDashing() == false)
                {
                    playerHealth.TakeDamage(attackDamage);
                    playerMovement.TakeDamagePush(isFacingRight, true);
                }
                else if (playerMovement.GetIsParrying())
                {
                    Debug.Log("PARRIED!!");
                    GetStunned();
                }
            }
            
        }
    }

    private IEnumerator StopCharging()
    {
        yield return new WaitForSeconds(chargeDuration);
        isCharging = false;
        stopCoroutine = null;

        if (activateCoroutine == null)
        {
            activateCoroutine = StartCoroutine(ActivateCanCharge());
        }
    }

    private IEnumerator ActivateCanCharge()
    {
        yield return new WaitForSeconds(2f);
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

    private void MoveFromPlayer()
    {
        Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, playerTransform.position.y - transform.position.y).normalized;
        rb.velocity = -direction * moveSpeed;
    }

    public void RotateTowardsPlayer()
    {
        if (!isCharging && !isStunned)
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
        isCharging = false;
        canCharge = false;
        Debug.Log("Stunned!");

        if (activateCoroutine != null)
            StopCoroutine(activateCoroutine);
        if (stopCoroutine != null)
            StopCoroutine(stopCoroutine);
        activateCoroutine = null;
        stopCoroutine = null;

        if(transform.position.y > playerTransform.position.y)
        {
            rb.gravityScale = 2;
            collider2d.isTrigger = false;
            Vector2 dir = new Vector2(transform.position.x - playerTransform.position.x, transform.position.y - playerTransform.position.y).normalized;
            rb.AddForce(dir * parryPushStrength, ForceMode2D.Impulse);
            //playerMovement.EnemyColliderOff(stunTime);
        }
        else
        {
            Vector2 dir = new Vector2(transform.position.x - playerTransform.position.x, transform.position.y - playerTransform.position.y).normalized;
            rb.AddForce(dir * parryPushStrength / 1.25f, ForceMode2D.Impulse);
        }



            StartCoroutine(StunEnd());
        StartCoroutine(StunLocationFix());
    }

    private IEnumerator StunEnd()
    {
        yield return new WaitForSeconds(stunTime);
        isCharging = false;
        stopCoroutine = null;

        if (activateCoroutine == null)
        {
            activateCoroutine = StartCoroutine(ActivateCanCharge());
        }
        isStunned = false;
        //canCharge = true;
        rb.gravityScale = 0;
        collider2d.isTrigger = true;
        Debug.Log("Active Again!");
    }

    private IEnumerator StunLocationFix()
    {
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
    }

    public void SetIsDead(bool dead)
    {
        isDead = dead;
    }
}
