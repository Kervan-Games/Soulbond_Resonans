using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private void Start()
    {
        RespawnPlayerAtLastCheckpoint();
    }

    public void RespawnPlayerAtLastCheckpoint()
    {
        if (PlayerPrefs.HasKey("LastCheckpoint"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");

            transform.position = new Vector3(x, y, z);
        }
        else
        {
            //Debug.Log("No checkpoint, spawning default.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("RESETTED! ** Pressing 'R' resets the checkpoints.");
        }
            
    }
}
