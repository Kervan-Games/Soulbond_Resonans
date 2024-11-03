using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritBox : MonoBehaviour
{
    public Transform InitialLocation;  
    public Transform TargetLocation;  
    public GameObject SpiritVisual;    

    public float risingSpeed = 2f;     

    private bool isRising = false;

    public GameObject Spirit;
    public ParticleSystem SpawnParticle;

    private PlayerMovement playerMovement;
    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (isRising)
        {
            SpiritVisual.transform.position = Vector3.MoveTowards(SpiritVisual.transform.position, TargetLocation.position, risingSpeed * Time.deltaTime);

            if (Vector3.Distance(SpiritVisual.transform.position, TargetLocation.position) < 0.01f)
            {
                ResetRising();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartRising();
        }
    }

    private void StartRising()
    {
        SpiritVisual.transform.position = InitialLocation.position;
        isRising = true;
    }

    private void ResetRising()
    {
        isRising = false;

        if (SpawnParticle != null)
        {
            SpawnParticle.Play();
        }

        Instantiate(Spirit, TargetLocation.position, Quaternion.identity);
        SpiritVisual.transform.position = InitialLocation.position;
        playerMovement.UpdateSpiritAmounts();
    }
}
