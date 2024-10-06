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

    private float floatAmplitude = 0.2f;  
    private float floatFrequency = 2f;    
    private float floatTimer = 0f;

    private bool isFlying = false;
    private bool isThrowing = false;
    private float playerSpeedX;
    private float playerSpeedY;
    public Rigidbody2D playerRb;

    private float speedMultiplier = 1f; //1f
    private float maxMultiplier = 1f; //5f 
    private float speedIncreaseRate = 0.5f; //5f

    public GameObject umbrellaOpened;
    public GameObject umbrellaClosed;
    public GameObject windUmbrella;
    private bool isReachedFlyPosition = false;

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
            if (!isReachedFlyPosition)
            {
                /*windUmbrella.SetActive(false);
                umbrellaClosed.SetActive(true);
                umbrellaOpened.SetActive(false);*/
            }
            else if (isReachedFlyPosition)
            {
                /*windUmbrella.SetActive(true);
                umbrellaClosed.SetActive(false);
                umbrellaOpened.SetActive(false);*/
            }
            
        }
        else if(isThrowing)
        {
            //
        }
        else
        {
            FollowPlayer();
            Idle();
            umbrellaOpened.SetActive(false);
            umbrellaClosed.SetActive(true);
            windUmbrella.SetActive(false);
        }

        if (!isFlying)
        {
            ResetFlyingSpeed(); 
        }



        /*if (umbrellaCollider.enabled)
        {
            RotateSlower(rotationSpeed);
        }
        else
        {
            RotateFaster();//Add cooldown after cancelling song
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            spirit.SetUmbrella(true);
            spirit.ThrowSpirit();
        }

        else if (collision.CompareTag("WalkerSpirit"))
        {
            WalkerSpirit walkerSpirit = collision.GetComponent<WalkerSpirit>();
            walkerSpirit.SetUmbrella(true);
            walkerSpirit.ThrowSpirit();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = (followPosition.position);
        targetPosition.x -= 0.3f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void Idle()
    {
        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        transform.position = new Vector3(transform.position.x, followPosition.position.y + floatOffset, transform.position.z);
    }

    private void Flying()
    {
        Vector3 targetPosition = flyPosition.position;
        targetPosition.x -= 0.3f;

        Vector3 currentPosition = transform.position;

        speedMultiplier += speedIncreaseRate * Time.deltaTime;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1f, maxMultiplier); 

        float newX = Mathf.Lerp(currentPosition.x, targetPosition.x, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newY = Mathf.Lerp(currentPosition.y, targetPosition.y, followSpeed * 3f * Time.deltaTime * speedMultiplier);
        float newZ = currentPosition.z;

        transform.position = new Vector3(newX, newY, newZ);

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget < 0.5f)
        {
            windUmbrella.SetActive(true);
            umbrellaClosed.SetActive(false);
            umbrellaOpened.SetActive(false);
            isReachedFlyPosition = true;
        }
        else
        {
            /*windUmbrella.SetActive(false);
            umbrellaClosed.SetActive(true);
            umbrellaOpened.SetActive(false);
            isReachedFlyPosition = false;*/
        }
    }

    private void ResetFlyingSpeed()
    {
        speedMultiplier = 1f;
    }


    // OLD SCRIPT ****************************************************************************************************
    /*void RotateSlower(float rotationSpeed)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RotateFaster()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = direction;
    }*/

    public void SetIsFlying(bool fly)
    {
        isFlying = fly;
    }

    public void SetIsThrowing(bool throwing)
    {
        isThrowing = throwing;
    }
}
