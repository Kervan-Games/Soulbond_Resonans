using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRockCollision : MonoBehaviour
{
    public GroundTrigger groundTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RockGround"))
        {
            groundTrigger = collision.GetComponent<GroundTrigger>();
        }

        if (collision.CompareTag("Rock"))
        {
            if(groundTrigger != null)
            {
                groundTrigger.GroundHit();
            }
        }
    }
}
