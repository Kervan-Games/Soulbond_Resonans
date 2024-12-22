using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicVision : MonoBehaviour
{
    public Mimic mimic;

    private void Update()
    {
        transform.position = mimic.gameObject.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mimic.SetHasVisual(true);
        }
    }

    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mimic.SetHasVisual(false);
        }
    }*/
}
