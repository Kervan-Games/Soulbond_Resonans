using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaneMovement : MonoBehaviour
{
    public float[] lanePositions = { 0, 5f, 10f };  
    public float moveSpeed = 5f;  
    private int currentLane = 1;  
    public float transitionSpeed = 5f;  

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W) && currentLane < lanePositions.Length - 1)
        {
            currentLane++;
        }

        if (Input.GetKeyDown(KeyCode.S) && currentLane > 0)
        {
            currentLane--;
        }

        Vector3 targetPosition = new Vector3(transform.position.x, lanePositions[currentLane], transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }
}
