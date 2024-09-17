using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAttach : MonoBehaviour
{
    private FollowPlayer playerFollower;
    private bool canAttach = true;

    public Rigidbody2D playerRigidbody;
    public float rotationSpeed = 10f; 
    private float maxRotation = 180f;
    private bool canRotate = false;

    private void Start()
    {
        playerFollower = GetComponent<FollowPlayer>();
        playerFollower.enabled = false;
    }


    private void Update()
    {
        if (canRotate)
        {
            if (playerRigidbody.velocity.x > 0.5 && transform.rotation.eulerAngles.z < maxRotation)
            {
                RotateBackground();
            }
            else if (transform.rotation.eulerAngles.z >= maxRotation)
            {
                DetachFromParent();
                playerFollower.enabled = false;
            }
        }
    }
    private void RotateBackground()
    {
        float newRotationZ = Mathf.Min(transform.rotation.eulerAngles.z + rotationSpeed * Time.deltaTime, maxRotation);
        transform.rotation = Quaternion.Euler(0, 0, newRotationZ);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canAttach)
        {
            transform.SetParent(collision.transform.parent);
            playerFollower.enabled = true;
            canAttach = false;
        }
    }

    public void DetachFromParent()
    {
        transform.SetParent(null);
    }


    public void SetCanRotate(bool rotate)
    {
        canRotate = rotate;
    }
}
