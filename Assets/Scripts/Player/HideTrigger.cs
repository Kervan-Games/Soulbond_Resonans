using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTrigger : MonoBehaviour
{
    private PlayerHide playerHide;

    void Start()
    {
        playerHide = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHide>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerHide.SetIsHiding(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerHide.SetIsHiding(false);
    }
}
