using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InteractionCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; 
    private Vector3 originalOffset; 
    private Transform playerTransform; 
    private Transform interactableTransform; 
    private bool canInteract = false;
    private bool isInteracting = false;

    void Start()
    {
        originalOffset = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
        playerTransform = GameObject.FindWithTag("Player").transform; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            interactableTransform = collision.transform; 
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            canInteract = false;
            interactableTransform = null;
        }
    }

    void Update()
    {
        if (canInteract && isInteracting)
        {
            AdjustCameraOffset();
        }
        else
        {
            ResetCameraOffset();
        }
    }

    void AdjustCameraOffset()
    {
        float offsetX = (interactableTransform.position.x - playerTransform.position.x) / 2;
        CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        Vector3 newOffset = new Vector3(originalOffset.x + offsetX, originalOffset.y, originalOffset.z);
        transposer.m_TrackedObjectOffset = newOffset;
    }

    public void ResetCameraOffset()
    {
        CinemachineFramingTransposer transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_TrackedObjectOffset = originalOffset;
    }

    public void SetIsInteracting(bool interact)
    {
        isInteracting = interact;
    }

    public void SetCanInteract(bool can)
    {
        canInteract = can;
    }
}
