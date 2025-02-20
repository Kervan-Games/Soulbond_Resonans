using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject[] debrisPrefabs;
    private float maxForce = 12f; 
    public bool isFacingRight = true;

    private float maxHealth = 60f;
    private float currentHealth;

    private int debrisAmount = 5;

    private PlayerMovement playerMovement;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            BreakObject();
        }

        StopAllCoroutines();
        StartCoroutine(FlashDamageEffect());
    }

    private void BreakObject()
    {
        isFacingRight = playerMovement.GetIsFacingRight();

        if (debrisPrefabs.Length == 0) return;

        for (int i = 0; i < debrisAmount; i++)
        {
            GameObject debris = Instantiate(debrisPrefabs[Random.Range(0, debrisPrefabs.Length)], transform.position, Quaternion.identity);
            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                if (isFacingRight)
                {
                    Vector2 forceDirection = Vector2.right;
                    forceDirection += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-1f, 2f));
                    forceDirection.Normalize();
                    rb.AddForce(forceDirection * Random.Range(5f, maxForce), ForceMode2D.Impulse);
                }
                else
                {
                    Vector2 forceDirection = Vector2.left;
                    forceDirection += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-1f, 2f));
                    forceDirection.Normalize();
                    rb.AddForce(forceDirection * Random.Range(1f, maxForce), ForceMode2D.Impulse);
                }
                
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator FlashDamageEffect()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.075f);
        spriteRenderer.color = originalColor;
    }
}
