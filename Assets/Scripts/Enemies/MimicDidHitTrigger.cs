using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicDidHitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mimic"))
        {
            collision.gameObject.GetComponent<Mimic>().SetDidHit(true);
        }
    }
}
