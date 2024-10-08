using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    public ParticleSystem landingParticles;


    public void PlayLandingParticles()
    {
        landingParticles.Play();
    }
}
