using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float bounce = 12f;
    private Rigidbody2D playerRB;

    private void Start()
    {
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            playerRB.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
}
