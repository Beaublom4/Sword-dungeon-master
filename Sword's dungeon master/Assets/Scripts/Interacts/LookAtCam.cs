using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private void Update()
    {
        if(Camera.main != null)
            transform.LookAt(Camera.main.transform.position);
    }
}
