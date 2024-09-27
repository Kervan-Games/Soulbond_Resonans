using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection = Vector2.right; 
    public float windForce = 5f;
    public GameObject umbrella;
    public PlayerMovement playerMovement;
    public Rigidbody2D playerRB;
    private bool inWind = false;
    private bool isFlying = false;

    private Coroutine windCoroutine; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (windCoroutine == null && playerMovement.GetCanUmbrella())
            {
                windCoroutine = StartCoroutine(ApplyWindForce(collision.GetComponent<Rigidbody2D>()));
                isFlying = true;
                umbrella.SetActive(true);
                
            }
            inWind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (windCoroutine != null)
            {
                StopCoroutine(windCoroutine);
                umbrella.SetActive(false); 
                windCoroutine = null;
                inWind = false;
                isFlying = false;
            }
        }
    }

    private void Update()
    {
        
    }


    private IEnumerator ApplyWindForce(Rigidbody2D playerRb)
    {
        while (true)
        {
            playerRb.AddForce(windDirection.normalized * windForce);
            yield return null; 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)windDirection.normalized * 2f); 
    }
}
