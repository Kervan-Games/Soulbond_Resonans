using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSpawner : MonoBehaviour
{
    public GameObject eye;
    public GameObject vision;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eye.SetActive(true);
            Invoke("ActivateVision", 0.1f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(eye.activeSelf != true)
                eye.SetActive(true);
        }
    }

    private void ActivateVision()
    {
        vision.GetComponent<SpriteRenderer>().enabled = true;
    }
}
