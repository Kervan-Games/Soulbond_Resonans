using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; // give numbers for each triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("LastCheckpoint", checkpointID);
            PlayerPrefs.Save();

            Vector3 checkpointPos = transform.position;
            PlayerPrefs.SetFloat("CheckpointX", checkpointPos.x);
            PlayerPrefs.SetFloat("CheckpointY", checkpointPos.y);
            PlayerPrefs.SetFloat("CheckpointZ", checkpointPos.z);
            PlayerPrefs.Save();

            //Debug.Log("Checkpoint reached: " + checkpointID);
        }
    }
}
