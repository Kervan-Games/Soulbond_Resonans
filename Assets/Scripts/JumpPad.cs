using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private float bounce = 12f;
    public Rigidbody2D playerRB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            playerRB.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
}
