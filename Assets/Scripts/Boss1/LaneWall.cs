using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneWall : MonoBehaviour
{
    public GameObject gate1;
    public GameObject gate2;
    public GameObject gate3;

    void Start()
    {
        GameObject[] gates = { gate1, gate2, gate3 };

        int randomIndex = GameManager.Instance.GetRandomGateIndex(gates.Length);

        for (int i = 0; i < gates.Length; i++)
        {
            if (i == randomIndex)
            {
                gates[i].SetActive(false); 
            }
            else
            {
                gates[i].SetActive(true);  
            }
        }
    }
}
