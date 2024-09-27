using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLightTrigger : MonoBehaviour
{
    public GameObject darknessObject;  
    private SpriteRenderer spriteRenderer;  
    public float fadeDuration = 0.5f;

    public ParticleSystem particle;

    private void Start()
    {
        spriteRenderer = darknessObject.GetComponent<SpriteRenderer>();

        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            if (particle != null)
            {
                particle.Stop();
            }

            StartCoroutine(FadeOut());
        }
            
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);  
            spriteRenderer.color = color;  
            yield return null;  
        }
        color.a = 1f;
        spriteRenderer.color = color;
    }
}
