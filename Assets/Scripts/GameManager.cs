using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int previousOpenGateIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetRandomGateIndex(int gateCount)
    {
        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, gateCount);
        } 
        while (randomIndex == previousOpenGateIndex);

        previousOpenGateIndex = randomIndex;

        return randomIndex;
    }
}