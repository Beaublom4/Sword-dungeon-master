using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitColor : MonoBehaviour
{
    Material startMaterial;
    private void Awake()
    {
        startMaterial = GetComponent<MeshRenderer>().material;
    }
    public void NormalMat()
    {
        GetComponent<MeshRenderer>().material = startMaterial;
    }
}
