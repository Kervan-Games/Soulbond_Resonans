using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

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

    void Start()
    {
        isHoldingSpirit = false;
        maxHealth = 100f;
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        speedMultiplier = playerMovement.GetMoveSpeed() / maxHealth;
        jumpMultiplier = playerMovement.GetJumpForce() / maxHealth;
    }

    void Update()
    {
        TakeDamageBySpiritHolding();
        HandleDie();
        Debug.Log(currentHealth);
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
            }
            else
            {
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
            }
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                didThrowSpirit = false;
            }
        }
        //else --->>> movespeed = original speed
    }

    private void Die()
    {
        playerMovement.SetIsDead(true);
        currentHealth = 0;
        playerMovement.SetMoveSpeed(0.0f);
        rb.velocity = Vector2.zero;
        playerMovement.enabled = false;
        enabled = false; 
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
    }
}
