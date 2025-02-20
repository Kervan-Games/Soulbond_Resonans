using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject followObject;
    public float offsetY = 2.5f;

    private void FixedUpdate()
    {
        transform.position = new Vector2(followObject.transform.position.x, followObject.transform.position.y + offsetY);
    }
}
