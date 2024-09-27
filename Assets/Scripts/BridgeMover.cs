using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMover : MonoBehaviour
{
    private SpiritTarget spiritTarget;
    public SpriteRenderer renderer1;
    public SpriteRenderer renderer2;
    public GameObject jumpPad1;
    public GameObject jumpPad2;
    public GameObject bridge;
    private Color newColor;
    public float rotationSpeed = 2f;
    void Start()
    {
        newColor = HexToColor("00FFC8");
        spiritTarget = GetComponent<SpiritTarget>();
    }

    void Update()
    {
        if (spiritTarget.GetDidHit())
        {
            renderer1.color = newColor;
            renderer2.color = newColor;
            if(jumpPad1.activeSelf == false)
            {
                jumpPad1.SetActive(true);
            }
            if (jumpPad2.activeSelf == false)
            {
                jumpPad2.SetActive(true);
            }
            StartCoroutine(RotateBridge());
        }
    }

    private IEnumerator RotateBridge()
    {
        Quaternion startRotation = bridge.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, 0);

        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            bridge.transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed);
            timeElapsed += Time.deltaTime * rotationSpeed;
            yield return null; 
        }

        bridge.transform.rotation = endRotation;
    }

    private Color HexToColor(string hex)
    {
        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }

        byte r = (byte)(int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber));
        byte g = (byte)(int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber));
        byte b = (byte)(int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        return new Color32(r, g, b, 255); 
    }
}
