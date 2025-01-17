using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPart : MonoBehaviour
{
    public Rigidbody2D rockRB;
    public GroundTrigger groundTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spirit") || collision.CompareTag("BulletSpirit"))
        {
            if(rockRB != null && groundTrigger != null)
            {
                rockRB.isKinematic = false;
                groundTrigger.SetCanParticle(true);
            }
        }
    }
}
