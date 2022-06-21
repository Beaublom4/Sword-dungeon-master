using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCardCam : MonoBehaviour
{
    public Transform target;
    [SerializeField] float moveInSec;
    float timeElapsed;

    private void Update()
    {
        if(timeElapsed < moveInSec)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, timeElapsed / moveInSec);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, timeElapsed / moveInSec);
            timeElapsed += Time.deltaTime;
        }
        else
        {
            transform.position = target.position;
        }
    }
}
