using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpiritThrow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;

    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("SpiritTarget"))
        {
            spriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
            startColor = spriteRenderer.color;
            spriteRenderer.color = Color.green;
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
    }
}
