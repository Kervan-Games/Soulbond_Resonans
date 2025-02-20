using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuncherHealth : MonoBehaviour
{
    public Image healthImage;
    public Image damageImage;
    private float maxHealth = 120f;
    private float currentHealth;
    private float damageLerpSpeed = 2f;

    private Puncher puncher;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;

    private void Start()
    {
        puncher = GetComponent<Puncher>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;

        UpdateHealthUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isDead)
        {
            TakeDamage(20f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isDead)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (currentHealth <= 0)
            {
                puncher.SetIsDead(true);
                isDead = true;
            }

            UpdateHealthUI();

            StopAllCoroutines();
            StartCoroutine(FlashDamageEffect());
            StartCoroutine(SmoothDamageFill());
        }
    }

    private void UpdateHealthUI()
    {
        healthImage.fillAmount = currentHealth / maxHealth;
    }

    private IEnumerator SmoothDamageFill()
    {
        float startFill = damageImage.fillAmount;
        float targetFill = currentHealth / maxHealth;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * damageLerpSpeed;
            damageImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime);
            yield return null;
        }

        damageImage.fillAmount = targetFill;
        if (targetFill == 0)
            Destroy(transform.parent.gameObject);
    }

    private IEnumerator FlashDamageEffect()
    {
        spriteRenderer.color = Color.white; 
        yield return new WaitForSeconds(0.075f); 
        spriteRenderer.color = originalColor; 
    }
}
