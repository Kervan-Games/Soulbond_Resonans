using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisabler : MonoBehaviour
{
    public GroundTrigger groundTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            groundTrigger.SetCanParticle(false);
        }
    }
}
