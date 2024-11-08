using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EyeVision : MonoBehaviour
{
    private SpriteRenderer spriteRenderer_;
    public Eye eye;
    private bool hasVisual = false;

    private void Start()
    {
        spriteRenderer_ = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (hasVisual)
        {
            eye.MoveTowardsPlayer();
            eye.RotateTowardsPlayer();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision != null)
        {
            eye.StopCoroutineBlink();
            spriteRenderer_.color = Color.red;
            Color visionColor = spriteRenderer_.color;
            visionColor.a = 0.55f;
            spriteRenderer_.color = visionColor;
            hasVisual = true;
            eye.AnimatorSetChaseTrigger();

            //eye.MoveTowardsPlayer();
            //eye.RotateTowardsPlayer();
            //collision.gameObject.GetComponent<PlayerMovement>().SetAnimatorIsDeadTrue();
        }
    }
    public bool GetHasVisual() { return hasVisual; }


    
}
