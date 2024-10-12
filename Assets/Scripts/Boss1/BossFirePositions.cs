using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFirePositions : MonoBehaviour
{
    public GameObject fireR;
    public GameObject fireL;

    public Transform R1;
    public Transform R2;
    public Transform R3;
    public Transform R4;
    public Transform R5;
    public Transform R6;

    public Transform L1;
    public Transform L2;
    public Transform L3;
    public Transform L4;
    public Transform L5;
    public Transform L6;

    public void SetPos1()
    {
        fireR.transform.SetParent(R1); 
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L1); 
        fireL.transform.localPosition = Vector3.zero; 
    }

    public void SetPos2()
    {
        fireR.transform.SetParent(R2);
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L2);
        fireL.transform.localPosition = Vector3.zero;
    }

    public void SetPos3()
    {
        fireR.transform.SetParent(R3);
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L3);
        fireL.transform.localPosition = Vector3.zero;
    }

    public void SetPos4()
    {
        fireR.transform.SetParent(R4);
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L4);
        fireL.transform.localPosition = Vector3.zero;
    }

    public void SetPos5()
    {
        fireR.transform.SetParent(R5);
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L5);
        fireL.transform.localPosition = Vector3.zero;
    }

    public void SetPos6()
    {
        fireR.transform.SetParent(R6);
        fireR.transform.localPosition = Vector3.zero;

        fireL.transform.SetParent(L6);
        fireL.transform.localPosition = Vector3.zero;
    }
}
