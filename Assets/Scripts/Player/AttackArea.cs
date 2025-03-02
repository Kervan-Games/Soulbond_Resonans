using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class AttackArea : MonoBehaviour
{
    private List<PuncherHealth> punchersInArea = new List<PuncherHealth>();
    private List<GiantHealth> giantsInArea = new List<GiantHealth>();
    //private List<Breakable> breakablesInArea = new List<Breakable>();

    private Breakable breakable;

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
        else if (other.CompareTag("Giant"))
        {
            GiantHealth giantHealth = other.GetComponent<GiantHealth>();
            if (giantHealth != null && !giantsInArea.Contains(giantHealth))
            {
                giantsInArea.Add(giantHealth);
            }
        }
        else if (other.CompareTag("Breakable"))
        {
            /*Breakable breakable = other.GetComponent<Breakable>();
            if (breakable != null && !breakablesInArea.Contains(breakable))
            {
                breakablesInArea.Add(breakable);
            }*/
            breakable = other.gameObject.GetComponent<Breakable>();
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
        else if (other.CompareTag("Giant"))
        {
            GiantHealth giantHealth = other.GetComponent<GiantHealth>();
            if (giantHealth != null && giantsInArea.Contains(giantHealth))
            {
                giantsInArea.Remove(giantHealth);
            }

        }
        else if (other.CompareTag("Breakable"))
        {
            /* Breakable breakable = other.GetComponent<Breakable>();
             if (breakable != null)
             {
                 breakablesInArea.Remove(breakable);
             }*/
            breakable = null;
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
        foreach(GiantHealth giant in giantsInArea)
        {
            if (giant != null)
            {
                giant.TakeDamage(damageAmount);
            }
        }


        /*foreach (Breakable breakable in breakablesInArea)
        {
            if (breakable != null)
            {
                breakable.TakeDamage(damageAmount);
            }
        }*/

        if (breakable != null)
        {
            breakable.TakeDamage(damageAmount);
        }
    }
}
