using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorSpeedReducer : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb; 
    public float maxSpeed = 7.5f; 
    private float speed;

    public PlayerMovement playerMovement;
    private bool isDead = false;

    void Update()
    {
        isDead = playerMovement.GetIsDead();
        speed = rb.velocity.magnitude;
        if (speed > 0.1f && speed <= maxSpeed && !isDead) 
        {
            animator.speed = speed / maxSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
