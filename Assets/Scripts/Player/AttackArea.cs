using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private List<PuncherHealth> punchersInArea = new List<PuncherHealth>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Puncher"))
        {
            PuncherHealth puncherHealth = other.GetComponent<PuncherHealth>();
            if (puncherHealth != null && !punchersInArea.Contains(puncherHealth))
            {
                punchersInArea.Add(puncherHealth);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Puncher"))
        {
            PuncherHealth puncherHealth = other.GetComponent<PuncherHealth>();
            if (puncherHealth != null && punchersInArea.Contains(puncherHealth))
            {
                punchersInArea.Remove(puncherHealth);
            }
        }
    }

    public void DealDamage(float damageAmount)
    {
        foreach (PuncherHealth puncher in punchersInArea)
        {
            if (puncher != null)
            {
                puncher.TakeDamage(damageAmount);
            }
        }
    }
}
