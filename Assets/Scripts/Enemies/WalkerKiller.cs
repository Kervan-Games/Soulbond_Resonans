using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerKiller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WalkerSpirit"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
