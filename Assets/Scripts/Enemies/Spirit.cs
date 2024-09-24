using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spirit : MonoBehaviour
{
    private float chaseRange = 7f;
    private float moveSpeed = 5f;
    private bool canChase;
    private bool canHit;
    private bool canChangeSing;
    private bool canShoot;
    private bool inRange;
    private bool inSingArea;//*****
    private bool isTouching = false;

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
    private PlayerMovement playerMovement;

    private bool canUmbrella;
    public GameObject umbrella;

    private float touchRange = 1f;
    private bool canTouch = true;

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
        canChangeSing = true;
        rb.isKinematic = false;
        canShoot = true;
        inRange = false;
        inSingArea = false;//********
        canUmbrella = false;
    }

    void Update()
    {
        CalculateDistanceToPlayer();

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < chaseRange)
            {
                if (canChase && !inSingArea)
                {
                    MoveTowardsPlayer();
                    RotateTowardsPlayer();
                }
                /*else if (inSingArea)
                {
                    //ThrowSpirit();
                }*/
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
                }
            }
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
            canHit = true;
            playerHealth.SetIsHoldingSpirit(true);
            isTouching = true;
        }

    }

    public void ThrowSpirit()// if a bug occurs when holding many spirit and you can not throw some spirits, take a look at canShoot.
    {
        if (canShoot && inRange)
        {
            rb.velocity = Vector3.zero;
            if (canUmbrella)
            {
                ThrowSpiritWithUmbrella();
                canShoot = false;
                //didUmbrella = false;
            }
            else if (rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction after hold
            {
                ThrowSpiritRandomDirection();
                //Debug.Log("kinematic shoot");
                canShoot = false;
            }

            /*else if (!rb.isKinematic && !playerSpiritThrow.GetCanThrow()) // throw to random direction before hold
            {
                if (canSing)
                {
                    ThrowSpiritBeforeHold();
                    //Debug.Log("before hold shoot");
                    canShoot = false;
                }
            }*/

            else if (playerSpiritThrow.GetCanThrow() && inRange) // throw to target in range
            {
                ThrowSpiritToTarget(playerSpiritThrow.GetTargetTransform());
                playerSpiritThrow.SetCanThrow(false);
                //Debug.Log("target shoot");
                canShoot = false;
            }
            playerHealth.SetDidThrowSpirit(true);
            playerHealth.SetIsHoldingSpirit(false);
            playerMovement.SetIsHoldingSpirit(false);
            SetInSingArea(false);
            isTouching = false;
        }
    }

    public void SetSingPosition()
    {
        if (canHit)
        {
            transform.SetParent(singAreaTransform);
            //transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
            //playerHealth.SetIsHoldingSpirit(true);
            playerMovement.SetIsHoldingSpirit(true);
            SetInSingArea(true);
            
        }
    }

    public void SetUmbrellaThrow()
    {
        transform.SetParent(spiritThrowHolderTransform);
        rb.isKinematic = false;
        canChase = true;
        playerMovement.SetIsHoldingSpirit(false);
        SetInSingArea(false);
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player")) && canHit)
        {
            transform.SetParent(spiritHolderTransform);
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;
            canChase = false;
            canHit = false;
            playerHealth.SetIsHoldingSpirit(true);
        }
    }*/

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        if (canChangeSing)
        {
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
        canChase = false;
        transform.SetParent(spiritThrowHolderTransform);
        rb.isKinematic = false;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector2 targetPosition = (Vector2)transform.position + randomDirection * 50f;
        StartCoroutine(MoveTowardsTarget(targetPosition, 2f));
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


    public void ThrowSpiritToTarget(Transform targetTransform)
    {
        canChase = false;
        transform.SetParent(spiritThrowHolderTransform);
        rb.isKinematic = false;
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

    private void CalculateDistanceToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < chaseRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }
    
    public void SetInSingArea(bool inArea)
    {
        inSingArea = inArea;
    }

    public bool GetInSingArea()
    {
        return inSingArea;
    }

    public void SetUmbrella(bool umbrella)
    {
        canUmbrella = umbrella;
    }

    public bool GetIsTouching()
    {
        return isTouching;
    }
}
