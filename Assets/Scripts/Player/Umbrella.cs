using System.Collections;
using System.Collections.Generic;
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

    private float maxSpeedThrowFollow = 25f;

    private bool canSmoothThrowFollow = true;

    public GameObject umbrellaTurn;
    public float umbrellaOffset = 2f;
    public float smoothSpeed = 2f;
    private Coroutine adjustYPositionCoroutine;


    private void Start()
    {
        umbrellaCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        playerSpeedX = playerRb.velocity.x;
        playerSpeedY = playerRb.velocity.y;

        if(isFlying)
        {
            Flying();
        }
        else if(isThrowing)
        {
            if (umbrellaCollider.enabled)
            {
                RotateSlower(rotationSpeed);
                umbrellaOpened.SetActive(true);
                umbrellaClosed.SetActive(false);
                FollowThrowPosition();
            }
            SmoothAdjustYPosition(umbrellaOffset);
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
            SmoothAdjustYPosition(0f);
        }

        if (!isFlying)
        {
            ResetFlyingSpeed(); 
        }
    }

    private void SmoothAdjustYPosition(float targetYOffset)
    {
        // Hedef pozisyonu hesapla
        float targetY = followPosition.position.y + targetYOffset;

        // Mevcut y pozisyonunu al
        Vector3 currentPosition = transform.position;

        // Hedef y pozisyonuna doðru sabit hýzda yaklaþ
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, smoothSpeed * Time.deltaTime);

        // Yeni pozisyonu uygula
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
    }

    private IEnumerator SmoothAdjustYPositionCoroutine(float targetYOffset)
    {
        while (true)
        {
            // Hedef pozisyonu hesapla
            float targetY = followPosition.position.y + targetYOffset;

            // Mevcut y pozisyonunu al
            Vector3 currentPosition = transform.position;

            // Hedef y pozisyonuna doðru sabit hýzda yaklaþ
            float newY = Mathf.MoveTowards(currentPosition.y, targetY, smoothSpeed * Time.deltaTime);

            // Yeni pozisyonu uygula
            transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);

            // Hedefe ulaþýldýysa coroutine'i sonlandýr
            if (Mathf.Abs(currentPosition.y - targetY) < 0.01f)
            {
                break;
            }

            yield return null; // Bir sonraki frame'e kadar bekle
        }
    }

    public void StartAdjustYPosition(float targetYOffset)
    {
        // Var olan coroutine'i durdur
        if (adjustYPositionCoroutine != null)
        {
            StopCoroutine(adjustYPositionCoroutine);
        }

        // Yeni coroutine baþlat
        adjustYPositionCoroutine = StartCoroutine(SmoothAdjustYPositionCoroutine(targetYOffset));
    }

    public void StopAdjustYPosition()
    {
        // Coroutine'i durdur
        if (adjustYPositionCoroutine != null)
        {
            StopCoroutine(adjustYPositionCoroutine);
            adjustYPositionCoroutine = null;
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
        umbrellaTurn.transform.position = Vector3.Lerp(umbrellaTurn.transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void Idle()
    {
        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        umbrellaTurn.transform.position = new Vector3(umbrellaTurn.transform.position.x, umbrellaTurn.transform.position.y + floatOffset, umbrellaTurn.transform.position.z);
    }

    private void Flying()
    {
        Vector3 targetPosition = flyPosition.position;
        targetPosition.x -= 0.3f;

        Vector3 currentPosition = umbrellaTurn.transform.position;

        speedMultiplier += speedIncreaseRate * Time.deltaTime;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, maxMultiplier); 

        float newX = Mathf.Lerp(currentPosition.x, targetPosition.x, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newY = Mathf.Lerp(currentPosition.y, targetPosition.y, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newZ = currentPosition.z;

        umbrellaTurn.transform.position = new Vector3(newX, newY, newZ);

        float distanceToTarget = Vector3.Distance(umbrellaTurn.transform.position, targetPosition);

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

        float distanceToTarget = Vector3.Distance(umbrellaTurn.transform.position, targetPosition); 
        float thresholdDistance = 0.25f;
        windUmbrella.SetActive(false);

        if (canSmoothThrowFollow)
        {
            umbrellaTurn.transform.position = Vector3.Lerp(umbrellaTurn.transform.position, targetPosition, maxSpeedThrowFollow * 1.5f * Time.deltaTime);
        }
        else
        {
            umbrellaTurn.transform.position = throwPosition.position;
        }

        if (distanceToTarget <= thresholdDistance)
        {
            canSmoothThrowFollow = false;
        }
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

        float currentZRotation = umbrellaTurn.transform.eulerAngles.z;
        float newZRotation = Mathf.LerpAngle(currentZRotation, targetZRotation, Time.deltaTime * rotationSpeedZ);

        if (isResetting)
        {
            umbrellaTurn.transform.rotation = Quaternion.Euler(0, 0, newZRotation);
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
    void RotateSlower(float rotationSpeed)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - umbrellaTurn.transform.position.x, mousePos.y - umbrellaTurn.transform.position.y);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        umbrellaTurn.transform.rotation = Quaternion.Lerp(umbrellaTurn.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RotateFaster()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - umbrellaTurn.transform.position.x, mousePos.y - umbrellaTurn.transform.position.y);
        umbrellaTurn.transform.up = direction;
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
