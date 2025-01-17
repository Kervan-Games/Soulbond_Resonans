using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float bounce = 12f;
    public float speedBonus = 25f;
    private Rigidbody2D playerRB;
    private PlayerMovement playerMovement;

    private bool canJump = true;
    private bool isInCooldown = false;

    private void Start()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerMovement = playerRB.gameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.GetIsJumpPressedDown() && canJump)
        {
            Jump();
            canJump = false;

            // cooldowna girsin;
            // rengi solsun
            // canJump ve renk düzelsin coroutine'i baslat

        }
    }

    private void Jump()
    {
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
        playerRB.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        if (playerMovement.GetIsFacingRight())
        {
            playerMovement.SetCurrentSpeed(speedBonus);
        }
        else
        {
            playerMovement.SetCurrentSpeed(-speedBonus);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!isInCooldown)
                canJump = true;
            playerMovement.SetIsInJumpPad(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canJump = false;
            playerMovement.SetIsInJumpPad(false);
        }
    }
}
