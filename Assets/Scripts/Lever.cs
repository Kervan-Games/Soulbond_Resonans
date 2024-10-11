using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private HingeJoint2D hinge;
    public float weightLimit = 5f;  
    public Transform leverCenter;   
    public float baseTorque = 500f; 
    public float baseSpeed = 50f;
    private Rigidbody2D rbLever;

    void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        rbLever = GetComponent<Rigidbody2D>();
        //rbLever.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /*void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();

        if (rb != null && rb.mass >= weightLimit)
        {
            
            float distanceFromCenter = Mathf.Abs(collision.transform.position.x - leverCenter.position.x);
            float torque = baseTorque * distanceFromCenter;
            float motorSpeed = baseSpeed * distanceFromCenter;

            if (collision.transform.position.x > leverCenter.position.x)
            {
                SetMotorDirection(motorSpeed, torque);
            }
            else
            {
                SetMotorDirection(-motorSpeed, torque);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();

        if (rb != null && rb.mass >= weightLimit)
        {
            float distanceFromCenter = Mathf.Abs(collision.transform.position.x - leverCenter.position.x);
            float torque = baseTorque * distanceFromCenter;
            float motorSpeed = baseSpeed * distanceFromCenter;

            if (collision.transform.position.x > leverCenter.position.x)
            {
                SetMotorDirection(motorSpeed, torque);
            }
            else
            {
                SetMotorDirection(-motorSpeed, torque);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        hinge.useMotor = false;
    }*/

    void SetMotorDirection(float motorSpeed, float torque)
    {
        JointMotor2D motor = hinge.motor;
        motor.motorSpeed = motorSpeed; 
        motor.maxMotorTorque = torque; 
        hinge.motor = motor;
        hinge.useMotor = true;
    }

    public void UnlockRotate()
    {
        rbLever.constraints = RigidbodyConstraints2D.None;
    }

    public void LockRotate()
    {
        rbLever.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
