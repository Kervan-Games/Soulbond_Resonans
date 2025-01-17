using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritTarget : MonoBehaviour
{
    private bool didHit = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Spirit"))
        {
            StartCoroutine(DestroyAfterDelay(collision, 0.15f));
            didHit = true;
            //Debug.Log("HIT");
        }
        else if (collision.CompareTag("BulletSpirit"))
        {
            StartCoroutine(DestroyAfterDelay(collision, 0.05f));
            didHit = true;
        }
    }

    private IEnumerator DestroyAfterDelay(Collider2D collider, float destroyTime)
    { 
        yield return new WaitForSeconds(destroyTime);
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
