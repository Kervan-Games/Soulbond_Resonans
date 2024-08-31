using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritDetectRange : MonoBehaviour
{
    private WalkerSpirit walkerSpirit;
    private void Start()
    {
        walkerSpirit = GetComponentInParent<WalkerSpirit>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            walkerSpirit.SetCanPatrol(false);
            walkerSpirit.SetInRange(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            walkerSpirit.SetCanPatrol(true);
            walkerSpirit.SetInRange(false);
        }
    }
}
