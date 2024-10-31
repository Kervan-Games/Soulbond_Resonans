using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    private Vector2 windDirection;
    public float windForce = 25f;
    public float horizontalForce = 1f;
    public GameObject windUmbrella;
    public PlayerMovement playerMovement;
    public Rigidbody2D playerRB;
    private bool inWind = false;
    public Umbrella umbrellaScript;
    private bool isWeightFlying = false;
    private Rigidbody2D weightRB;

    private Coroutine windCoroutine;

    private void Start()
    {
        windDirection = transform.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inWind = true;
            umbrellaScript.SetIsFlying(true);
            playerMovement.SetInWind(true);
        }
        else if (collision.CompareTag("Weight"))
        {
            weightRB = collision.GetComponent<Rigidbody2D>();
            isWeightFlying = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           // if (windCoroutine != null)
            //{
               // StopCoroutine(windCoroutine);
                windUmbrella.SetActive(false); 
                //windCoroutine = null;
                inWind = false;
                umbrellaScript.SetIsFlying(false);
                playerMovement.SetInWind(false);
           // }
        }
        else if (collision.CompareTag("Weight"))
        {
            isWeightFlying = false;
            weightRB = null;
        }
    }

    private void FixedUpdate()
    {
        if (inWind)
        {
            Vector2 windForceVector = windDirection.normalized * windForce;
            playerRB.AddForce(new Vector2(windForceVector.x * horizontalForce, windForceVector.y));
        }
        if (isWeightFlying)
        {
            if (weightRB != null)
                weightRB.AddForce(windDirection.normalized * windForce * 20f);
            else
                Debug.LogError("WeightRB is null!");
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
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)windDirection.normalized * 5f); 
    }
}
