using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeVision : MonoBehaviour
{
    private SpriteRenderer spriteRenderer_;
    public Eye eye;

    private void Start()
    {
        spriteRenderer_ = GetComponent<SpriteRenderer>();
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
            collision.gameObject.GetComponent<PlayerMovement>().SetAnimatorIsDeadTrue();
        }
    }
}
