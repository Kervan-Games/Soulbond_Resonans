using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiritBoxInteraction : MonoBehaviour
{
    private bool canInteract = false;
    private bool didInteract = false;
    private GameObject interactedObject;
    private SpiritBox box;
    public void SpiritBoxOnInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed && canInteract && !didInteract)
        {
            if(box != null)
            {
                
            }
            else
            {
                Debug.LogError("SpiritBox is NULL!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpiritBox"))
        {
            interactedObject = collision.gameObject;
            canInteract = true;
            box = collision.gameObject.GetComponent<SpiritBox>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SpiritBox"))
        {
            canInteract = false ;
            interactedObject = null;
            box = null;
        }
    }

    public void SetCanInteract(bool interact) // ruh yükselirken can interact false olsun, yükselme tamamlanýnca tekrar true olsun ki tekrar çalýþtýrabilsin.
    {
        canInteract = interact;
    }

    public void SetDidInteract(bool did)
    {
        didInteract = did;
    }
}
