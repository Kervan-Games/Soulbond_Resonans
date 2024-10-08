using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    public bool isHiding = false;
    public void SetIsHiding(bool hiding)
    {
        isHiding = hiding;
    }

    public bool GetIsHiding()
    {
        return isHiding;
    }
}
