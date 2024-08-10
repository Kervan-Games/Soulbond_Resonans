using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritTarget : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Spirit"))
        {
            StartCoroutine(DestroyAfterDelay(collision));
        }
    }

    private IEnumerator DestroyAfterDelay(Collider2D collider)
    { 
        yield return new WaitForSeconds(0.15f);
        if (collider != null)
        {
            Destroy(collider.gameObject);
        } 
    }
}
