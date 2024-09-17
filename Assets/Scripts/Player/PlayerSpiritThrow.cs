using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerSpiritThrow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor = Color.yellow;
    private bool canThrowToTarget;
    private Transform targetTransform;

    private GameObject currentTarget;

    private void Start()
    {
        canThrowToTarget = false;
    }
    private void Update()
    {
        ColorUpdate();
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
            currentTarget = other.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SpiritTarget"))
        {
            //spriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
           // startColor = spriteRenderer.color;
            //spriteRenderer.color = Color.green;

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

    private void ColorUpdate()
    {
        if (currentTarget != null)
        {
            if (spriteRenderer != null && startColor != null)
            {
                if (!canThrowToTarget)
                {
                    spriteRenderer.color = startColor;
                }
                else if (canThrowToTarget)
                {
                    spriteRenderer.color = Color.green;
                }
            }
        }
        

    }
}
