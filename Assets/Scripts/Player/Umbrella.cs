using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Umbrella : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private Collider2D umbrellaCollider;

    public Transform followPosition;
    public Transform flyPosition;
    public Transform throwPosition;
    private float followSpeed = 6f;

    private float floatAmplitude = 0.03f;  
    private float floatFrequency = 2.1f;    
    private float floatTimer = 0f;

    private bool isFlying = false;
    private bool isThrowing = false;
    private float playerSpeedX;
    private float playerSpeedY;
    public Rigidbody2D playerRb;

    private float speedMultiplier = 1f; 
    private float maxMultiplier = 1f; 
    private float speedIncreaseRate = 0.5f; 

    public GameObject umbrellaOpened;
    public GameObject umbrellaClosed;
    public GameObject windUmbrella;

    private float rotationSpeedZ = 5f; 

    private bool isResetting = false; 
    private float targetZRotation = 0f;

    public float maxSpeedThrowFollow = 25f;

    private bool canSmoothThrowFollow = true;
    public GameObject umbrellaRotate;
    private float playerFollowOffset;

    private bool didTurn = false;
    public float rotationSpeedForThrow = 5f;
    public float moveSpeedForThrow = 1f;
    public float radiusForThrow = 4f;
    public float throwSpeedMultiplier = 1f;

    private Vector3 throwDirection; 
    private bool hasThrowDirection = false;
    private Coroutine rotateCoroutine;
    private bool isMovingThrow = false;
    private bool isRotating = false;
    private float tempRadius;
    private SpiritEconomy spiritEco;



    private void Awake()
    {
       // followPosition.position = new Vector3(followPosition.position.x, followPosition.position.y - 1.8f, followPosition.position.z);
    }

    private void Start()
    {
        umbrellaCollider = GetComponent<Collider2D>();
        spiritEco = GetComponent<SpiritEconomy>();
    }

    void FixedUpdate()
    {
        playerSpeedX = playerRb.velocity.x;
        playerSpeedY = playerRb.velocity.y;

        if(isFlying)
        {
            Flying();
            if (didTurn)
            {
                didTurn = false;
            }
        }
        else if(isThrowing || isMovingThrow)
        {
            if (umbrellaCollider.enabled)
            {
                RotateSlower(throwPosition.position, rotationSpeedForThrow, moveSpeedForThrow, radiusForThrow);
                umbrellaOpened.SetActive(true);
                umbrellaClosed.SetActive(false);
                //FollowThrowPosition();
            }
        }
        else
        {
            FollowPlayer();
            Idle();
            umbrellaOpened.SetActive(false);
            umbrellaClosed.SetActive(true);
            windUmbrella.SetActive(false);
            SmoothResetZRotation();

            if(canSmoothThrowFollow == false)
            {
                canSmoothThrowFollow = true;
            }
            if (didTurn)
            {
                didTurn = false;
            }
        }

        if (!isFlying)
        {
            ResetFlyingSpeed(); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            spirit.SetUmbrella(true);
            spirit.ThrowSpirit();
        }

        /*else if (collision.CompareTag("WalkerSpirit"))
        {
            WalkerSpirit walkerSpirit = collision.GetComponent<WalkerSpirit>();
            walkerSpirit.SetUmbrella(true);
            walkerSpirit.ThrowSpirit();
        }*/
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = (followPosition.position);
        targetPosition.x -= 0.3f;
        umbrellaRotate.transform.position = Vector3.Lerp(umbrellaRotate.transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void Idle()
    {
        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        umbrellaRotate.transform.position = new Vector3(umbrellaRotate.transform.position.x, umbrellaRotate.transform.position.y + floatOffset, umbrellaRotate.transform.position.z);
    }

    private void Flying()
    {
        Vector3 targetPosition = flyPosition.position;
        //targetPosition.x -= 0.3f;

        Vector3 currentPosition = umbrellaRotate.transform.position;

        speedMultiplier += speedIncreaseRate * Time.deltaTime;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, maxMultiplier); 

        float newX = Mathf.Lerp(currentPosition.x, targetPosition.x, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newY = Mathf.Lerp(currentPosition.y, targetPosition.y, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newZ = currentPosition.z;

        umbrellaRotate.transform.position = new Vector3(newX, newY, newZ);

        float distanceToTarget = Vector3.Distance(umbrellaRotate.transform.position, targetPosition);

        if (distanceToTarget < 0.5f)
        {
            windUmbrella.SetActive(true);
            umbrellaClosed.SetActive(false);
            umbrellaOpened.SetActive(false);
        }
        else
        {
            /*windUmbrella.SetActive(false);
            umbrellaClosed.SetActive(true);
            umbrellaOpened.SetActive(false);*/
        }
    }

    private void FollowThrowPosition()
    {
        Vector3 targetPosition = throwPosition.position;
        targetPosition.x -= 0.3f; 

        float distanceToTarget = Vector3.Distance(umbrellaRotate.transform.position, targetPosition); 
        //float thresholdDistance = 0.25f;
        windUmbrella.SetActive(false);

        if (canSmoothThrowFollow)
        {
            umbrellaRotate.transform.position = Vector3.Lerp(umbrellaRotate.transform.position, targetPosition, maxSpeedThrowFollow * Time.deltaTime);
        }
        else
        {
            umbrellaRotate.transform.position = throwPosition.position;
        }

        /*if (distanceToTarget <= thresholdDistance)
        {
            canSmoothThrowFollow = false;
        }*/
    }


    private void ResetFlyingSpeed()
    {
        speedMultiplier = 1f;
    }
    public void ResetRotation()
    {
        isResetting = true;
    }

    private void SmoothResetZRotation()
    {

        float currentZRotation = transform.eulerAngles.z;
        float newZRotation = Mathf.LerpAngle(currentZRotation, targetZRotation, Time.deltaTime * rotationSpeedZ);

        if (isResetting)
        {
            transform.rotation = Quaternion.Euler(0, 0, newZRotation);
        }

        if (Mathf.Abs(newZRotation - targetZRotation) < 0.01f)
        {
            isResetting = false;
        }
        else
        {
            isResetting = true;
        }
    }


    // OLD SCRIPT ****************************************************************************************************
    /*void RotateSlower(float rotationSpeed)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - umbrellaRotate.transform.position.x, mousePos.y - umbrellaRotate.transform.position.y);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        if(!didTurn)
        {
            umbrellaRotate.transform.rotation = targetRotation;
            didTurn = true;
        }
        umbrellaRotate.transform.rotation = Quaternion.Lerp(umbrellaRotate.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }*/
    // 1.8 y ve umbrellaya, starttakini aktifleþtir oluyor

    void RotateSlower(Vector3 throwPosition, float rotationSpeed, float moveSpeed, float radius)
    {
        if (!isRotating)
        {
            isRotating = true;
            isMovingThrow = true;
            rotateCoroutine = StartCoroutine(RotateAndMove(throwPosition, rotationSpeed, moveSpeed, radius));
        }
    }

    private IEnumerator RotateAndMove(Vector3 throwPosition, float rotationSpeed, float baseMoveSpeed, float radius)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;

        Vector3 direction = (mousePos - throwPosition).normalized;
        Vector3 targetPosition = throwPosition + direction * radius;

        float targetAngle = Mathf.Atan2(targetPosition.y - throwPosition.y, targetPosition.x - throwPosition.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        while (Vector3.Distance(umbrellaRotate.transform.position, targetPosition) > 0.1f ||
               Quaternion.Angle(gameObject.transform.rotation, targetRotation) > 0.1f)
        {
            float distance = Vector3.Distance(umbrellaRotate.transform.position, targetPosition);
            float dynamicSpeed = Mathf.Lerp(baseMoveSpeed * 0.5f, baseMoveSpeed * 1.5f, distance / radius) * throwSpeedMultiplier;

            dynamicSpeed = Mathf.Clamp(dynamicSpeed, 0f, 12.5f);
            umbrellaRotate.transform.position = Vector3.MoveTowards(umbrellaRotate.transform.position, targetPosition, dynamicSpeed * Time.deltaTime);

            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }
        spiritEco.ThrowSpirit();
        Invoke("ReturnToInitialPosition", 1f);
    }

    void ReturnToInitialPosition()
    {
        isRotating = false;
        rotateCoroutine = null;
        isThrowing = false;
        isMovingThrow = false;
    }



    void RotateFaster()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - umbrellaRotate.transform.position.x, mousePos.y - umbrellaRotate.transform.position.y);
        umbrellaRotate.transform.up = direction;
    }

    public void SetIsFlying(bool fly)
    {
        isFlying = fly;
    }

    public bool GetIsFlying()
    {
        return isFlying;
    }

    public void SetIsThrowing(bool throwing)
    {
        isThrowing = throwing;
    }
}
