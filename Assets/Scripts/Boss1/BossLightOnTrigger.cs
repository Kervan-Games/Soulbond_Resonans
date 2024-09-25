using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLightOnTrigger : MonoBehaviour
{
    public GameObject darknessObject;
    private SpriteRenderer spriteRenderer;
    public float fadeDuration = 0.5f;

    private void Start()
    {
        spriteRenderer = darknessObject.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        spriteRenderer.color = color;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }
        color.a = 0f;
        spriteRenderer.color = color;
    }
}
