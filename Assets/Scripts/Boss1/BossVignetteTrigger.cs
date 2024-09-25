using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BossVignetteTrigger : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    public float fadeDuration = 2f;

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out vignette);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVignetteIntensity(0f, 0.40f));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeVignetteIntensity(0.40f, 0f));
        }
    }

    IEnumerator ChangeVignetteIntensity(float startValue, float endValue)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            vignette.intensity.value = Mathf.Lerp(startValue, endValue, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.value = endValue;
    }
}
