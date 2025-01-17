using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritEconomy : MonoBehaviour
{
    private int spiritCount = 0;
    public float bulletSpeed = 10f;
    public Transform spawnPoint;
    public GameObject BulletSpirit;


    private void FixedUpdate()
    {
        //Debug.Log(spiritCount);
    }
    public void IncreaseSpiritCount()
    {
        spiritCount++;
    }

    public void ThrowSpirit()
    {
        if (BulletSpirit != null)
        {
            GameObject spawnedBullet = Instantiate(BulletSpirit, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.up * bulletSpeed; 
            }

            StartCoroutine(DestroyAfterTime(spawnedBullet, 5f));
        }
        else
        {
            Debug.LogWarning("BulletSpirit prefab atanmamýþ!");
        }
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj != null)
        {
            Destroy(obj);
        }
    }

}
