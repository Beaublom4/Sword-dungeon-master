using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLazer : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] LayerMask mask;
    [SerializeField] LazerCatcher[] lazers;

    bool isShooting;
    IEnumerator coroutine;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        foreach(LazerCatcher lazer in lazers)
        {
            lazer.RandomStart();
        }
    }
    public void End()
    {
        Debug.Log("end");
        Invoke("WaitEnding", .1f);
    }
    void WaitEnding()
    {
        StopAllCoroutines();
        DungeonManager.Instance.PuzzleFinished();
    }
    public void DoLazer()
    {
        if (isShooting)
            return;
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.tag == "LazerPuzzle")
            {
                Debug.Log(hit.transform);
                hit.collider.GetComponentInParent<LazerCatcher>()?.ShootUpdate(hit.transform, mask);
            }
        }
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hit.point);

        coroutine = LazerShoot();
        StartCoroutine(coroutine);
    }
    IEnumerator LazerShoot()
    {
        Debug.Log("Test");
        foreach(LazerCatcher catchers in lazers)
        {
            catchers.lockRot = true;
        } 
        yield return new WaitForSeconds(3);
        isShooting = false;
        foreach (LazerCatcher catchers in lazers)
        {
            catchers.lockRot = false;
            catchers.TurnOff();
        }
        TurnOff();
    }
    void TurnOff()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }
}
