using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTriggerB : MonoBehaviour
{
    private WalkerSpirit walkerSpirit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WalkerSpirit"))
        {
            walkerSpirit = collision.GetComponent<WalkerSpirit>();
            walkerSpirit.WalkToPointA();
        }
    }
}
