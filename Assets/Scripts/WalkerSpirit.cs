using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class WalkerSpirit : MonoBehaviour
{
    private float touchRange = 1f;
    private float moveSpeed = 5f;
    private float patrolSpeed = 2.5f;
    private bool canChase;
    private bool canHit;
    private bool canSing;
    private bool canChangeSing;
    private bool canShoot;

    private Transform playerTransform;
    private Transform spiritHolderTransform;
    private Transform spiritThrowHolderTransform;
    private Transform singAreaTransform;//********
    private GameObject player;
    private GameObject spiritHolder;
    private GameObject spiritThrowHolder;
    private GameObject singArea;//********


    private Rigidbody2D rb;

    private PlayerSpiritThrow playerSpiritThrow;
    private PlayerHealth playerHealth;

    private bool inRange;
    private bool canPatrol;
    private bool canTouch;
    private SpiritDetectRange detectRange;

    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;
    private bool isWalkingA;
    private bool isWalkingB;
    private bool didShoot;
    private bool closeToPlayer;
    private bool inSingArea;//*****

    private PlayerMovement playerMovement;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spiritHolder = GameObject.FindGameObjectWithTag("SpiritHolder");
        spiritThrowHolder = GameObject.FindGameObjectWithTag("SpiritThrowHolder");
        singArea = GameObject.FindGameObjectWithTag("SingArea");//********
        rb = GetComponent<Rigidbody2D>();
        playerSpiritThrow = GameObject.FindGameObjectWithTag("SpiritThrower").GetComponent<PlayerSpiritThrow>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
        currentPoint = pointB.transform;

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

        if (spiritThrowHolder != null)
        {
            spiritThrowHolderTransform = spiritThrowHolder.transform;
        }
        else
        {
            Debug.LogError("SpiritThrowHolder is NULL!");
        }

        //*******
        if (singArea != null)
        {
            singAreaTransform = singArea.transform;
        }
        else
        {
            Debug.LogError("SingArea is NULL!");
        }
        //*******

        canChase = true;
        canHit = true;
        canSing = false;
        canChangeSing = true;
        rb.isKinematic = false;
        canShoot = true;
        inRange = false;
        canPatrol = true;
        canTouch = true;
        isWalkingA = false;
        isWalkingB = true;
        didShoot = false;
        closeToPlayer = false;
        inSingArea = false;//********
    }

    void Update()//on patrol phase, now spirit can be shot even before chase. if it is not necessary, change it.
    {
        CalculateDistanceToPlayer();
        if (canChase && inRange)
        {
            MoveTowardsPlayer();
            RotateTowardsPlayer();
        }
        else if(!inRange && canPatrol)
        {
            if (!didShoot && !inSingArea)
            {
                Patrol();
            }
        }

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < touchRange)
            {
                if (canHit && canTouch)
                {
                    TouchToPlayer();
                    canTouch = false;
                    DisableTheDetectObject();
                }
            }
        }
    }

    public void ThrowSpirit()
    {
        if (canShoot &&closeToPlayer)
        {
            if (rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction after hold
            {
                ThrowSpiritRandomDirection();
                //Debug.Log("kinematic shoot");
                canShoot = false;
                didShoot = true;

                playerHealth.SetDidThrowSpirit(true);
                playerHealth.SetIsHoldingSpirit(false);
            }

            else if (!rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction before hold
            {
                if (canSing)
                {
                    ThrowSpiritBeforeHold();
                    //Debug.Log("before hold shoot");
                    canShoot = false;
                    didShoot = true;

                    playerHealth.SetDidThrowSpirit(true);
                    playerHealth.SetIsHoldingSpirit(false);
                }
            }

            else if (playerSpiritThrow.GetCanThrow() && closeToPlayer) // throw to target in range
            {
                ThrowSpiritToTarget(playerSpiritThrow.GetTargetTransform());
                playerSpiritThrow.SetCanThrow(false);
                //Debug.Log("target shoot");
                canShoot = false;
                didShoot = true;

                playerHealth.SetDidThrowSpirit(true);
                playerHealth.SetIsHoldingSpirit(false);
            }
            playerMovement.SetIsHoldingSpirit(false);
        }
    }


    private void MoveTowardsPlayer()
    {
        rb.velocity = Vector2.zero; // for zeroing the remaining velocity from patrolling
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

    public void SetSingPosition()
    {
        if (canHit)
        {
            DisableTheDetectObject();
            inSingArea = true; 
            inRange = true;
            canPatrol = false;
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
            transform.SetParent(singAreaTransform);
            transform.rotation = Quaternion.identity;
            rb.velocity = Vector2.zero;
            playerHealth.SetIsHoldingSpirit(true);
            playerMovement.SetIsHoldingSpirit(true);
        }
    }

    public void ThrowSpiritRandomDirection()
    {
        transform.SetParent(spiritThrowHolderTransform);
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
        transform.SetParent(spiritThrowHolderTransform);
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
        transform.SetParent(spiritThrowHolderTransform);
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

    private void Patrol()
    {
        if (canPatrol)
        {
            Vector2 point = currentPoint.position - transform.position;
            if (currentPoint == pointB.transform)
            {
                rb.velocity = new Vector2(patrolSpeed, 0f);
            }
            else
            {
                rb.velocity = new Vector2(-patrolSpeed, 0f);
            }

            if (isWalkingA && !isWalkingB)
            {
                currentPoint = pointA.transform;
            }
            if (!isWalkingA && isWalkingB)
            {
                currentPoint = pointB.transform;
            }

            /*float angle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));*/

            float targetAngle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
            float currentAngle = transform.rotation.eulerAngles.z;

            // Açýlarý yumuþak bir þekilde deðiþtir
            float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, 3f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothedAngle));
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    private void TouchToPlayer()
    {
        if (canHit)
        {
            transform.SetParent(spiritHolderTransform);
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
            playerHealth.SetIsHoldingSpirit(true);
        }
    }

    void DisableTheDetectObject()
    { 
        Transform detectObject = transform.Find("DetectArea");

        if (detectObject != null)
        {
            detectObject.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DetectArea object couldn't find! -debugWarning");
        }
    }

    public void SetInRange(bool range)
    {
        inRange = range;
    }

    public void SetCanPatrol(bool patrol)
    {
        canPatrol = patrol;
    }

    public void WalkToPointA()
    {
        isWalkingA = true;
        isWalkingB = false;
    }

    public void WalkToPointB()
    {
        isWalkingB = true; 
        isWalkingA = false;
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale; 

    }

    private void CalculateDistanceToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < 7f)
        {
            closeToPlayer = true;
        }
        else
        {
            closeToPlayer = false;
        }
    }
}
