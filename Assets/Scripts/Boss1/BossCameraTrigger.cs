using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public GameObject boss; 
    public GameObject player; 
    public CinemachineVirtualCamera virtualCamera; 
    public float zoomDuration = 2f; //add new zoom duration for offset change if needs
    public float targetOrthoSize = 12f; 
    public Vector3 bossOffset = new Vector3(16, 1, 0); 
    public Vector3 playerOffset = new Vector3(0, 1, 0); 
    private float originalOrthoSize; 
    private Vector3 originalOffset; 

    private void Start()
    {
        originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        originalOffset = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCamera.Follow = boss.transform;

            StartCoroutine(SmoothZoom(targetOrthoSize));
            StartCoroutine(SmoothOffset(bossOffset));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            virtualCamera.Follow = player.transform;

            StartCoroutine(SmoothZoom(originalOrthoSize));
            StartCoroutine(SmoothOffset(playerOffset)); 
        }
    }

    IEnumerator SmoothZoom(float targetSize)
    {
        float elapsedTime = 0f;
        float startingSize = virtualCamera.m_Lens.OrthographicSize;

        while (elapsedTime < zoomDuration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startingSize, targetSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    IEnumerator SmoothOffset(Vector3 targetOffset)
    {
        float elapsedTime = 0f;
        Vector3 startingOffset = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;

        while (elapsedTime < zoomDuration)
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Vector3.Lerp(startingOffset, targetOffset, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = targetOffset;
    }
}