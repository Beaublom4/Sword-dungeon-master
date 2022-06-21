using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadielsPillarManager : MonoBehaviour
{
    [SerializeField] YadielsPillar[] pillars;
    [SerializeField] float minTime, MaxTime;

    public void PillarCooldown()
    {
        StartCoroutine(Cooldown());
    }
    IEnumerator Cooldown()
    {
        float num = Random.Range(minTime, MaxTime);
        yield return new WaitForSeconds(num);

        int random = Random.Range(0, pillars.Length);
        pillars[random].ShowPillarReady();
    }
}
