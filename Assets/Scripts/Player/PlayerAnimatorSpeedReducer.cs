using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorSpeedReducer : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb; 
    public float maxSpeed = 7.5f; 
    private float speed;

    void Update()
    {
        speed = rb.velocity.magnitude;
        if (speed > 0.1f && speed <= maxSpeed) 
        {
            animator.speed = speed / maxSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
