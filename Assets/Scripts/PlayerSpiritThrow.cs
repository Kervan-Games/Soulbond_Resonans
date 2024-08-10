using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerSpiritThrow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private bool canThrowToTarget;
    private Transform targetTransform;

    private void Start()
    {
        canThrowToTarget = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("SpiritTarget"))
        {
            spriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
            startColor = spriteRenderer.color;
            spriteRenderer.color = Color.green;

            targetTransform = other.transform;
            canThrowToTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SpiritTarget"))
        {
            if(spriteRenderer != null)
            {
                spriteRenderer.color = startColor;
            }
            else
            {
                Debug.LogError("Sprite Renderer is NULL!");
            }
        }

        canThrowToTarget = false;
    }

    public bool GetCanThrow() 
    { 
        return canThrowToTarget; 
    }

    public Transform GetTargetTransform() 
    { 
        return targetTransform; 
    }

    public void SetCanThrow(bool canThrow)
    {
        canThrowToTarget = canThrow;
    }
}
