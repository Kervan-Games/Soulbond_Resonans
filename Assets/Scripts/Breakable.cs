using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject[] debrisPrefabs;
    public float maxForce = 10f; 
    public bool isFacingRight = true;

    private float maxHealth = 60f;
    private float currentHealth;

    private int debrisAmount = 5;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            BreakObject();
        }
    }

    private void BreakObject()
    {
        if (debrisPrefabs.Length == 0) return;

        for (int i = 0; i < debrisAmount; i++)
        {
            GameObject debris = Instantiate(debrisPrefabs[Random.Range(0, debrisPrefabs.Length)], transform.position, Quaternion.identity);
            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 forceDirection = isFacingRight ? Vector2.right : Vector2.left;
                forceDirection += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f));    
                forceDirection.Normalize(); 
                rb.AddForce(forceDirection * Random.Range(3f, maxForce), ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
