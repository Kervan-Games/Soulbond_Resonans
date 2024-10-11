using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightSpeedLimiter : MonoBehaviour
{
    // this script is required for platforms to avoid too much acceleration during the movement
    private Rigidbody2D rb;
    private float maxSpeedX = 4f;  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(rb.velocity.x) > maxSpeedX)
        {
            float limitedSpeedX = Mathf.Sign(rb.velocity.x) * maxSpeedX;
            rb.velocity = new Vector2(limitedSpeedX, rb.velocity.y);
        }
    }
}
