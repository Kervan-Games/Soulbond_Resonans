using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class WalkerSpirit : MonoBehaviour
{
    private float touchRange = 1f;
    public float moveSpeed = 5f;
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
    public GameObject detectArea;

    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;
    private bool isWalkingA;
    private bool isWalkingB;
    private bool didShoot;
    private bool closeToPlayer;
    private bool inSingArea;//*****
    private bool isTouching = false;

    private PlayerMovement playerMovement;
    private bool canUmbrella;
    private GameObject umbrella;
    private bool didTouch = false;

    private PlayerHide playerHideScript;

    private Animator _animator;

    private bool didHit = false;

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
        playerHideScript = player.GetComponent<PlayerHide>();
        _animator = GetComponent<Animator>();
        umbrella = GameObject.FindGameObjectWithTag("Umbrella");

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
        if (umbrella == null)
        {
            Debug.LogError("Umbrella is NULL!");
        }

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
        canUmbrella = false;

    }

    void Update()//on patrol phase, now spirit can be shot even before chase. if it is not necessary, change it.
    {
        CalculateDistanceToPlayer();
        if (!didHit && canChase && inRange && playerHideScript.GetIsHiding() == false)
        {
            MoveTowardsPlayer();
            RotateTowardsPlayer();
            _animator.SetBool("isChasing", true);
        }
        else if(!inRange && canPatrol && !didHit)
        {
            if (!didShoot && !inSingArea)
            {
                Patrol();
                _animator.SetBool("isChasing", false);
            }
        }

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < touchRange)
            {
                if (canHit && canTouch && playerHideScript.GetIsHiding() == false)
                {
                    playerMovement.SetAnimatorIsDeadTrue();
                    TouchToPlayer();
                    canTouch = false;
                    DisableTheDetectObject();
                }
            }
        }

        if (didHit)
        {
            Patrol();
            _animator.SetBool("isChasing", false);
            detectArea.SetActive(false);
        }

        /*if(detectArea.activeSelf == true && canPatrol == false)
        {
            canPatrol = true;
        }
        else if(detectArea.activeSelf == false && canPatrol == true)
        {
            canPatrol = false;
        }*/
    }

    public void ThrowSpirit()
    {
        if (canShoot &&closeToPlayer)
        {
            if (canUmbrella)
            {
                DisableTheDetectObject();
                ThrowSpiritWithUmbrella();
                canShoot = false;
                playerHealth.SetIsHoldingSpirit(false);
                playerMovement.SetIsHoldingSpirit(false);
                playerHealth.SetDidThrowSpirit(true);
                //didUmbrella = false;
            }
            else if (rb != null && rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction after hold
            {
                DisableTheDetectObject();
                ThrowSpiritRandomDirection();
                //Debug.Log("kinematic shoot");
                canShoot = false;
                didShoot = true;

                playerHealth.SetIsHoldingSpirit(false);
                playerMovement.SetIsHoldingSpirit(false);
                playerHealth.SetDidThrowSpirit(true);
            }

            else if (rb != null && !rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction before hold
            {
                if (canSing)
                {
                    DisableTheDetectObject();
                    ThrowSpiritBeforeHold();
                    //Debug.Log("before hold shoot");
                    canShoot = false;
                    didShoot = true;

                    playerHealth.SetIsHoldingSpirit(false);
                    playerMovement.SetIsHoldingSpirit(false);
                    playerHealth.SetDidThrowSpirit(true);
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
                playerMovement.SetIsHoldingSpirit(false);
            }
            isTouching = false;
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

        if (transform.localScale.x < 0)
        {
            angle += 180f;
        }

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
            //playerHealth.SetIsHoldingSpirit(true);
            playerMovement.SetIsHoldingSpirit(true);
        }
    }
    public void SetUmbrellaThrow()
    {
        inSingArea = false;
        inRange = true;
        canPatrol = true;
        rb.isKinematic = false;
        canChase = true;
        transform.SetParent(spiritThrowHolderTransform);
        //playerHealth.SetIsHoldingSpirit(true);
        playerMovement.SetIsHoldingSpirit(false);
    }

    public void ThrowSpiritRandomDirection()
    {
        canChase = false;
        transform.SetParent(spiritThrowHolderTransform);
        rb.isKinematic = false;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 targetPosition = (Vector2)transform.position + randomDirection * 50f;
        canSing = false;
        StartCoroutine(MoveTowardsTarget(targetPosition, 2f));
    }

    public void ThrowSpiritWithUmbrella()
    {
        canChase = false;
        transform.SetParent(spiritThrowHolderTransform); 
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;

        Vector2 umbrellaDirection = umbrella.transform.up.normalized; 
        Vector2 targetPosition = (Vector2)transform.position + umbrellaDirection * 50f; 

        StartCoroutine(MoveTowardsTarget(targetPosition, 3f)); 
    }

    private IEnumerator MoveTowardsTarget(Vector2 targetPosition, float speedMultiplier)
    {
        while ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * speedMultiplier * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    public void ThrowSpiritToTarget(Transform targetTransform)
    {
        canChase = false;
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
            //transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            float currentAngle = transform.rotation.eulerAngles.z;
            if (currentAngle != 0f)
            {
                float targetAngle = 0f;

                float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, 3f * Time.deltaTime);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothedAngle));
            }

            Vector2 point = currentPoint.position - transform.position;
            if (currentPoint == pointB.transform)
            {
                rb.velocity = new Vector2(patrolSpeed, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                rb.velocity = new Vector2(-patrolSpeed, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            if (isWalkingA && !isWalkingB)
            {
                currentPoint = pointA.transform;
            }
            if (!isWalkingA && isWalkingB)
            {
                currentPoint = pointB.transform;
            }

            /*
            float targetAngle = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
            float currentAngle = transform.rotation.eulerAngles.z;

            float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, 3f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, smoothedAngle));
            */

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
            DisableTheDetectObject();
            /*transform.SetParent(spiritHolderTransform);
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;*/
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
            canPatrol = false;
            //playerHealth.SetIsHoldingSpirit(true);
            didTouch = true;
            isTouching = true;
        }
    }

    void DisableTheDetectObject()
    { 
        if (detectArea != null)
        {
            detectArea.SetActive(false);
            canPatrol = false;
            rb.velocity = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("DetectArea object couldn't find!");
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
    public void SetUmbrella(bool umbrella)
    {
        canUmbrella = umbrella;
    }

    public bool GetDidTouch()
    {
        return didTouch;
    }

    public bool GetInSingArea()
    {
        return inSingArea;
    }

    public bool GetIsTouching()
    {
        return isTouching;
    }

    public void SetDidHit(bool hit)
    {
        didHit = hit;
    }
}
