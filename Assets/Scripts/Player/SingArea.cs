using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingArea : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private Collider2D singCollider;
    public GameObject umbrella;
    public ParticleSystem singParticles;

    public GameObject emissionPoint;
    private float moveSpeed = 8f;    
    private float destroyThreshold = 0.1f; 

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
            Spirit spirit = collision.gameObject.GetComponent<Spirit>();
            spirit.SetCanChase(false);
            spirit.SetCanHit(false);
            if(spirit.GetDidTouch())
                spirit.ThrowSpirit();
            else
                StartCoroutine(MoveToEmissionPoint(collision.gameObject));
            
        }
    }

    private IEnumerator MoveToEmissionPoint(GameObject spirit)
    {
        while (spirit != null) // Eðer obje hâlâ varsa
        {
            // Spirit'in mevcut konumundan emissionPoint'in konumuna doðru hareket et
            spirit.transform.position = Vector3.MoveTowards(
                spirit.transform.position,
                emissionPoint.transform.position,
                moveSpeed * Time.deltaTime
            );

            // Spirit hedefe yeterince yakýnsa yok et
            if (Vector3.Distance(spirit.transform.position, emissionPoint.transform.position) < destroyThreshold)
            {
                Destroy(spirit);
                yield break;
            }

            yield return null; // Bir sonraki frame'e kadar bekle
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
