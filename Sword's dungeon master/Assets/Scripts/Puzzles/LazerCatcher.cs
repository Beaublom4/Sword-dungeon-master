using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerCatcher : MonoBehaviour
{
    [System.Serializable]
    public class PointCombos
    {
        public Transform point1;
        public Transform point2;
    }
    [SerializeField] PointCombos[] combos;
    [HideInInspector] public bool lockRot;

    [SerializeField] bool endPoint;
    [SerializeField] StartingLazer startPoint;
    public void RandomStart()
    {
        int randomRot = Random.Range(0, 9);
        transform.localRotation = Quaternion.Euler(0, randomRot * 45, 0);
    }
    public void ShootUpdate(Transform t, LayerMask mask)
    {
        if (endPoint)
        {
            startPoint.End();
            return;
        }

        foreach(PointCombos combo in combos)
        {
            RaycastHit hit;
            if (t == combo.point2)
            {
                if (Physics.Raycast(combo.point1.position, combo.point1.forward, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.tag == "LazerPuzzle")
                    {
                        hit.collider.GetComponentInParent<LazerCatcher>().ShootUpdate(hit.transform, mask);
                    }
                }
                combo.point1.GetComponent<LineRenderer>().SetPosition(0, combo.point1.position);
                combo.point1.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            }
            else if (t == combo.point1)
            {
                if (Physics.Raycast(combo.point2.position, combo.point2.forward, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.tag == "LazerPuzzle")
                    {
                        hit.collider.GetComponentInParent<LazerCatcher>().ShootUpdate(hit.transform, mask);
                    }
                }
                combo.point2.GetComponent<LineRenderer>().SetPosition(0, combo.point2.position);
                combo.point2.GetComponent<LineRenderer>().SetPosition(1, hit.point);
            }
        }
    }
    public void Rotate()
    {
        if (lockRot)
            return;
        transform.Rotate(0, 45, 0);
    }
    public void TurnOff()
    {
        foreach (PointCombos combo in combos)
        {
            combo.point1.GetComponent<LineRenderer>().SetPosition(0, combo.point1.position);
            combo.point1.GetComponent<LineRenderer>().SetPosition(1, combo.point1.position);
            combo.point2.GetComponent<LineRenderer>().SetPosition(0, combo.point2.position);
            combo.point2.GetComponent<LineRenderer>().SetPosition(1, combo.point2.position);
        }
    }
}
