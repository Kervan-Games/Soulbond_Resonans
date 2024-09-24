using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingArea : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private Collider2D singCollider;
    public GameObject umbrella;

    private void Start()
    {
        singCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(singCollider.enabled)
        {
            RotateSlower(rotationSpeed);
        }
        else
        {
            RotateFaster();//Add cooldown after cancelling song
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            spirit.SetSingPosition();
        }

        else if (collision.CompareTag("WalkerSpirit"))
        {
            WalkerSpirit walkerSpirit = collision.GetComponent<WalkerSpirit>();
            walkerSpirit.SetSingPosition();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            if (umbrella.activeSelf)
            {
                spirit.SetUmbrellaThrow();
            }
        }
        else if (collision.CompareTag("WalkerSpirit"))
        {
            WalkerSpirit wspirit = collision.GetComponent<WalkerSpirit>();
            if (umbrella.activeSelf)
            {
                wspirit.SetUmbrellaThrow();
            }
        }
    }

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
