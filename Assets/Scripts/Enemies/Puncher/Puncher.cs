using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puncher : MonoBehaviour
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

    private float stunTime = 2.5f;

    private bool isAttacking = false;

    private bool isStunned = false;
    private bool isFacingRight = true;
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
        if(distance < chaseRange && distance > stoppingRange && !isAttacking && !isStunned) 
        {
            MoveToPlayer();
        }
        else 
        {
            rb.velocity = Vector3.zero;
        }
         
    }

    private void Update()
    {
        distance = Vector3.Distance(playerObj.transform.position, gameObject.transform.position);
        //Debug.Log(distance);
        if (!isStunned)
        {
            HandleFlipping();
        }

        if (distance <= stoppingRange && !isAttacking && !isStunned)
        {
            rb.velocity = Vector3.zero;
            isAttacking = true;
            Debug.Log("charging");
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.4f);
        if(distance < 5f)
        {
            if(playerMovement.GetIsParrying() == false)
            {
                Debug.Log("DAMAGE!!");
                playerHealth.TakeDamage(attackDamage);
            }
            else
            {
                Debug.Log("PARRIED!!");
                GetStunned();
            }
            
        }
        isAttacking = false;

    }

    private void MoveToPlayer()
    {
        Vector2 direction  = new Vector2(playerTransform.position.x - transform.position.x, 0).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void HandleFlipping()
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

    private void GetStunned()
    {
        isStunned = true;
        Debug.Log("stunned!");
        // geriye dogru biraz it facing right degerine gore yon alarak
        StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log(" NOT  stunned!");
    }
}
