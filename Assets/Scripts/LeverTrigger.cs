using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    private Lever lever;
    void Start()
    {
        lever = GetComponentInParent<Lever>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weight"))
        {
            lever.UnlockRotate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Weight"))
        {
            lever.LockRotate();
        }
    }
}
