using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    public Rigidbody2D rockRB;
    public Rigidbody2D selfRB;

    public Collider2D rockCollider;
    public Collider2D selfCollider;
    public GameObject dieCollider;

    public ParticleSystem collisionParticle;
    private bool canParticle = false;
    private bool canHit = true;

    public GameObject rocks;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rockRB.isKinematic = false;
            transform.localScale = new Vector3(1.2f, 0.4584f, 1f);
        }
        else if (collision.CompareTag("Rock"))
        {
            GroundHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GroundHit();
        }
    }


    private void ColliderDisabler()
    {
        foreach (Collider2D childCollider in rocks.GetComponentsInChildren<Collider2D>())
        {
            childCollider.isTrigger = true;
        }

        rockCollider.isTrigger = true;
        selfCollider.isTrigger= true;
        dieCollider.SetActive(false);
        Invoke("Destroyer", 3f);
    }

    private void Destroyer()
    {
        Destroy(rockRB.gameObject);
        Destroy(selfRB.gameObject);
    }

    public void SetCanParticle(bool can)
    {
        canParticle = can;
    }

    public void GroundHit()
    {
        if (canHit)
        {
            canHit = false;
            selfRB.isKinematic = false;
            selfCollider.isTrigger = true;
            selfCollider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            rocks.SetActive(true);
            if (canParticle)
            {
                collisionParticle.Play();
                canParticle = false;
            }
            Invoke("ColliderDisabler", 0.4f);
        }
    }
}
