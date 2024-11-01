using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;
    private float decreaseRate = 15f;
    private float increaseRate = 30f;

    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private bool isHoldingSpirit;
    private bool didThrowSpirit;
    private float speedMultiplier;
    private float jumpMultiplier;

    public Image healthBar;

    public ParticleSystem spiritHoldParticle;

    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private float initialVignette;
    private float vignetteTarget = 0.8f;

    void Start()
    {
        isHoldingSpirit = false;
        maxHealth = 100f;
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        speedMultiplier = playerMovement.GetMoveSpeed() / maxHealth;
        jumpMultiplier = playerMovement.GetJumpForce() / maxHealth;

        postProcessVolume.profile.TryGetSettings(out vignette);
        initialVignette = vignette.intensity.value;
    }

    private void FixedUpdate()
    {
        UpdateVignette();
    }

    void Update()
    {
        TakeDamageBySpiritHolding();
        HandleDie();
        
        //Debug.Log(currentHealth);
        //** HEALTH REGEN IS NOT INCLUDED EXCEPT SPIRIT THROW, contact game designer for health regen mechanics.
    }

    private void TakeDamageBySpiritHolding()
    {
        if (isHoldingSpirit)
        {
            didThrowSpirit = false;

            if (currentHealth > 0)
            {
                currentHealth -= decreaseRate * Time.deltaTime;
                playerMovement.SetMoveSpeed(currentHealth * speedMultiplier);
                playerMovement.SetJumpForce(currentHealth * jumpMultiplier);
                healthBar.fillAmount = currentHealth / 100f;
            }
            else
            {
                if(playerMovement.GetIsDead() && playerMovement != null)
                    Die();
            }
        }
        else if(!isHoldingSpirit && didThrowSpirit)
        {
            if (currentHealth < maxHealth) 
            {
                currentHealth += increaseRate * Time.deltaTime;
                playerMovement.SetMoveSpeed(currentHealth * speedMultiplier);
                playerMovement.SetJumpForce(currentHealth * jumpMultiplier);
                healthBar.fillAmount = currentHealth / 100f;
            }
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                healthBar.fillAmount = currentHealth / 100f;
                didThrowSpirit = false;
            }
        }
        //else --->>> movespeed = original speed
    }

    public void Die()
    {
        currentHealth = 0;
        healthBar.fillAmount = 0;
        enabled = false;// optional ?
        playerMovement.enabled = false;// optional ?
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void DieVisual()
    {
        currentHealth = 0;
        healthBar.fillAmount = 0;
        enabled = false;// optional ?
        playerMovement.enabled = false;// optional ?
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth / 100f;
        }
        else
        {
            Die();
        }
    }

    public void Heal(float heal)
    {
        if(currentHealth < maxHealth)
        {
            currentHealth += heal;
            healthBar.fillAmount = currentHealth / 100f;
        } 
        else if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            healthBar.fillAmount = currentHealth / 100f;
        }
    }

    private void HandleDie()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetDidThrowSpirit(bool didThrow)
    {
        didThrowSpirit = didThrow;  
    }

    public void SetIsHoldingSpirit(bool isHolding)
    {
        isHoldingSpirit = isHolding;

        if(isHolding == true)
        {
            spiritHoldParticle.Play();
        }
        else
        {
            spiritHoldParticle.Stop();
        }
    }

    private void UpdateVignette()
    {
        float healthPercentage = currentHealth / maxHealth;

        vignette.intensity.value = Mathf.Lerp(vignetteTarget, initialVignette, healthPercentage);

        Color vignetteColor = vignette.color.value;
        vignetteColor.r = Mathf.Lerp(0.15f, 0f, healthPercentage);
        vignette.color.value = vignetteColor;
    }
}
