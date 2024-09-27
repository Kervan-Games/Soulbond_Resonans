using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoomTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float targetZoom = 6.5f;
    public float zoomDuration = 2f;
    private bool canZoom = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canZoom)
        {
            StartCoroutine(SmoothZoom(targetZoom));
            canZoom = false;
        }
    }

    public void SetCameraOrthoSize(float targetSize)
    {
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.OrthographicSize = targetSize; 
        }
        else
        {
            Debug.LogError("Virtual camera is null!");
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
}
