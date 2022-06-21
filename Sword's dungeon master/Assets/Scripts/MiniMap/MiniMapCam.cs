using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    [SerializeField] Vector3 wantedRotation;
    private void Update()
    {
        transform.rotation = Quaternion.Euler(wantedRotation);
    }
}
