using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject bossObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bossObject.SetActive(true);
    }
}
