using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritEconomy : MonoBehaviour
{
    private int spiritCount = 0;

    public void IncreaseSpiritCount()
    {
        spiritCount++;
    }

    private void FixedUpdate()
    {
        //Debug.Log(spiritCount);
    }
}
