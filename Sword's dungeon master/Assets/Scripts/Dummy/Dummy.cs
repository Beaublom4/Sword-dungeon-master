using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] MeshRenderer[] renderers;
    [SerializeField] Material hitMat;

    IEnumerator hitCoroutine;

    public void GetHit()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);
        hitCoroutine = HitFlash();
        StartCoroutine(hitCoroutine);
    }
    IEnumerator HitFlash()
    {
        foreach (MeshRenderer mr in renderers)
        {
            mr.material = hitMat;
        }
        yield return new WaitForSeconds(.2f);
        foreach (MeshRenderer mr in renderers)
        {
            mr.GetComponent<HitColor>().NormalMat();
        }
    }
}
