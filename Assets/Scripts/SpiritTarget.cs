using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritTarget : MonoBehaviour
{
    private bool didHit = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Spirit") || collision.CompareTag("BulletSpirit"))
        {
            StartCoroutine(DestroyAfterDelay(collision));
            didHit = true;
            //Debug.Log("HIT");
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

    public bool GetDidHit()
    {
        return didHit;
    }
}
