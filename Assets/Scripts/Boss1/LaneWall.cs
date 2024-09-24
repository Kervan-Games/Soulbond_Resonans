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
        GameObject[] objects = { gate1, gate2, gate3 };
        int randomIndex = Random.Range(0, objects.Length);

        for (int i = 0; i < objects.Length; i++)
        {
            if (i == randomIndex)
            {
                objects[i].SetActive(false);
            }
            else
            {
                objects[i].SetActive(true);
            }
        }
    }
}
