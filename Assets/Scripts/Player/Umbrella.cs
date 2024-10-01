using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private Collider2D umbrellaCollider;

    public Transform followPosition; 
    private float followSpeed = 6f;

    private float floatAmplitude = 0.2f;  
    private float floatFrequency = 2f;    
    private float floatTimer = 0f;

    private bool isFlying = false;
    private bool isThrowing = false;

    private void Start()
    {
        umbrellaCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if(isFlying)
        {
            //
        }
        else if(isThrowing)
        {
            //
        }
        else
        {
            FollowPlayer();
            Idle();
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

    void Idle()
    {
        floatTimer += Time.fixedDeltaTime * floatFrequency;
        float floatOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        // Sadece y eksenine salýným ekle
        transform.position = new Vector3(transform.position.x, followPosition.position.y + floatOffset, transform.position.z);
    }


    // OLD SCRIPT ****************************************************************************************************
    void RotateSlower(float rotationSpeed)
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
    }
}
