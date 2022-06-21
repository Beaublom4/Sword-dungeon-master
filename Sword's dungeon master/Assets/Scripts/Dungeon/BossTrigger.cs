using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    bool hasTriggered;
    [SerializeField] EnemyHealth healthScript;
    [SerializeField] EnemyPathFinding pathFinding;

    [SerializeField] GameObject splashScreenCam;
    [SerializeField] GameObject camPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;
        if(other.tag == "Player")
        {
            hasTriggered = true;
            Transform target = other.transform.root.GetComponent<PlayerMove>().camLoc;
            GameObject currentCam = Instantiate(camPrefab, target.position, target.rotation, null);
            currentCam.GetComponent<TitleCardCam>().target = splashScreenCam.transform;
            Destroy(currentCam, 3);
            healthScript.ActivateBossBar();
            Invoke("Trigger", 3);
        }
    }
    void Trigger()
    {
        pathFinding.triggered = true;
    }
}
