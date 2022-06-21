using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [HideInInspector] public float damageMultiplier = 1;
    [HideInInspector] public PlayerLook lookScript;

    bool canHit;
    List<Transform> hits = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit)
            return;

        Transform root = other.GetComponentInParent<EnemyHealth>()?.transform;

        foreach (Transform t in hits)
        {
            if (t == root)
            {
                return;
            }
        }
        if (other.tag == "Dummy")
        {
            float calcDmg = Loadout.blade.damage * damageMultiplier;

            other.transform.root.GetComponent<Dummy>().GetHit();
            DamageDisplay dmg = Instantiate(lookScript.dmgShowPrefab, other.transform.position, Quaternion.identity, null).GetComponent<DamageDisplay>();
            dmg.dmg.text = calcDmg.ToString();
            hits.Add(other.transform.root);
        }
        else if(other.tag == "Enemy")
        {
            if (other.GetComponentInParent<EnemyHealth>().death)
                return;

            float calcDmg;
            float isCrit = Random.Range(0, 100);
            bool isCritBool = false;

            if(isCrit < Loadout.blade.critChance)
            {
                calcDmg = Loadout.blade.critDamage * damageMultiplier;
                isCritBool = true;
            }
            else
            {
                calcDmg = Loadout.blade.damage * damageMultiplier;
            }

            if(Loadout.blade.lifeStealPercentage > 0)
            {
                float lifeSteal = calcDmg * (Loadout.blade.lifeStealPercentage * 0.01f);
                GetComponentInParent<PlayerHealth>().LifeSteal(lifeSteal);
            }

            //sound
            lookScript.HitSound();

            other.GetComponentInParent<EnemyHealth>().GetHit(calcDmg, true, Loadout.blade.bleedTime, Loadout.guard.staggerTime);
            DamageDisplay dmg = Instantiate(lookScript.dmgShowPrefab, other.transform.position, Quaternion.identity, null).GetComponent<DamageDisplay>();
            dmg.dmg.text = calcDmg.ToString();
            dmg.IsCrit(isCritBool);
            hits.Add(root);
        }
        else if (other.tag == "Pillar")
        {
            other.GetComponent<YadielsPillar>().DoPillarAttack();
        }
    }
    public void StartHit()
    {
        canHit = true;
    }
    public void EndHit()
    {
        canHit = false;
        hits.Clear();
    }
}
