using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BGStartTurnTrigger : MonoBehaviour
{
    public GameObject background;
    private BackgroundAttach bgAttach;

    private void Start()
    {
        bgAttach = background.GetComponent<BackgroundAttach>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bgAttach.SetCanRotate(true);
        }
    }
}
