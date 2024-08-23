using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spirit : MonoBehaviour
{
    private float chaseRange = 7f;
    private float moveSpeed = 5f;
    private bool canChase;
    private bool canHit;
    private bool canSing;
    private bool canChangeSing;
    private bool canShoot;

    private Transform playerTransform;
    private Transform spiritHolderTransform;
    private GameObject player;
    private GameObject spiritHolder;

    private Rigidbody2D rb;

    private PlayerSpiritThrow playerSpiritThrow;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spiritHolder = GameObject.FindGameObjectWithTag("SpiritHolder");
        rb = GetComponent<Rigidbody2D>();
        playerSpiritThrow = GameObject.FindGameObjectWithTag("SpiritThrower").GetComponent<PlayerSpiritThrow>();

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player is NULL!");
        }

        if (spiritHolder != null)
        {
            spiritHolderTransform = spiritHolder.transform;
        }
        else
        {
            Debug.LogError("SpiritHolder is NULL!");
        }

        canChase = true;
        canHit = true;
        canSing = false;
        canChangeSing = true;
        rb.isKinematic = false;
        canShoot = true;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < chaseRange)
            {
                if (canChase)
                {
                    MoveTowardsPlayer();
                    RotateTowardsPlayer();
                }
            }
        }
    }

    public void ThrowSpirit()
    {
        if (canShoot)
        {
            if (rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction after hold
            {
                ThrowSpiritRandomDirection();
                Debug.Log("kinematic shoot");
                canShoot = false;
            }

            else if (!rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction before hold
            {
                if (canSing)
                {
                    ThrowSpiritBeforeHold();
                    Debug.Log("before hold shoot");
                    canShoot = false;
                }
            }

            else if (playerSpiritThrow.GetCanThrow()) // throw to target in range
            {
                ThrowSpiritToTarget(playerSpiritThrow.GetTargetTransform());
                playerSpiritThrow.SetCanThrow(false);
                Debug.Log("target shoot");
                canShoot = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player")) && canHit)
        {
            transform.SetParent(spiritHolderTransform);
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        if (canChangeSing)
        {
            canSing = true;
            canChangeSing = false;
        }
        
    }

    private void RotateTowardsPlayer()
    {
        Vector2 direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void ThrowSpiritRandomDirection()
    {
        rb.isKinematic = false;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 targetPosition = (Vector2)transform.position + randomDirection * 50f;
        canSing = false;
        StartCoroutine(MoveTowardsTarget(targetPosition));
    }

    private IEnumerator MoveTowardsTarget(Vector2 targetPosition)
    {
        while ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * 2 * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void ThrowSpiritToTarget(Transform targetTransform)
    {
        rb.isKinematic = false;
        canSing = false;
        StartCoroutine(MoveTowardsTarget(targetTransform));
    }

    private IEnumerator MoveTowardsTarget(Transform targetTransform)
    {
        while ((Vector2)transform.position != (Vector2)targetTransform.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetTransform.position, moveSpeed * 2f * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void ThrowSpiritBeforeHold()
    {
        canChase = false;
        rb.isKinematic = false;
        Vector2 currentDirection = (playerTransform.position - transform.position).normalized;
        Vector2 oppositePosition = currentDirection * -1;
        canSing = false;
        StartCoroutine(MoveToOppositeDirection(oppositePosition));
    }

    private IEnumerator MoveToOppositeDirection(Vector2 oppositePosition)
    {
        Vector2 targetPosition = (Vector2)transform.position + oppositePosition * 50f;

        while ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * 2f * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
