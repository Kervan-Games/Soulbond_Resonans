using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTrap : MonoBehaviour
{
    public float moveAmount = 2.3f;  
    public float moveSpeed = 5f;
    private bool isMoved = false;
    public bool isHorizontal = false;
    public bool fromEnd = false;

    private void Start()
    {
        //StartCoroutine(MoveChildrenSequentially());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isMoved)
        {
            StartCoroutine(MoveChildrenSequentially());
            isMoved = true;
        }
    }

    IEnumerator MoveChildrenSequentially()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            int index;
            if (fromEnd)
            {
                index = childCount - 1 - i;
            }
            else
            {
                index = i;
            }

            Transform child = transform.GetChild(index);
            
            Vector3 startPos = child.position;
            Vector3 targetPos;

            if (isHorizontal)
            {
                targetPos = startPos + new Vector3(moveAmount, 0, 0);
            }
            else
            {
                targetPos = startPos + new Vector3(0, moveAmount, 0);
            }

            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                child.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                yield return null;
            }

            child.position = targetPos; 
        }
    }
}
