using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportPoint;
    public Rigidbody2D playerRB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Teleport(collision.transform);
        }
    }

    private void Teleport(Transform player)
    {
        if (teleportPoint != null)
        {
            playerRB.velocity = Vector2.zero;
            player.position = teleportPoint.position;
        }
    }
}
