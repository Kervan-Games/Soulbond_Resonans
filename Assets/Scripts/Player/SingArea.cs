using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingArea : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private Collider2D singCollider;
    public GameObject umbrella;
    public ParticleSystem singParticles;

    private void Start()
    {
        singCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        //OldScream();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            spirit.SetSingPosition();
            var emission = singParticles.emission;
            emission.rateOverTime = 400f;

            var mainModule = singParticles.main;
            mainModule.simulationSpeed = 1.25f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //OldOnTriggerStay(collision);
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

    private void OldScreamUpdate()
    {
        if (singCollider.enabled)
        {
            RotateSlower(rotationSpeed);
            if (singParticles.isPlaying == false)
            {
                //singParticles.Play();
            }
        }
        else
        {
            RotateFaster();//Add cooldown after cancelling song
            if (singParticles.isPlaying == true)
            {
                //singParticles.Stop();
                var emission = singParticles.emission;
                emission.rateOverTime = 100f;

                var mainModule = singParticles.main;
                mainModule.simulationSpeed = 1f;
            }
        }
    }

    private void OldOnTriggerEnter(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            spirit.SetSingPosition();
            var emission = singParticles.emission;
            emission.rateOverTime = 400f;

            var mainModule = singParticles.main;
            mainModule.simulationSpeed = 1.25f;
        }
    }

    private void OldOnTriggerStay(Collider2D collision)
    {
        if (collision.CompareTag("Spirit"))
        {
            Spirit spirit = collision.GetComponent<Spirit>();
            if (umbrella.activeSelf)
            {
                spirit.SetUmbrellaThrow();
            }
        }
    }
}
