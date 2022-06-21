using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    public bool inRange;
    public List<Collider> playerColls = new List<Collider>();
    private void Update()
    {
        if (playerColls.Count > 0)
        {
            inRange = true;
        }
        else if (inRange)
            inRange = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerColls.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerColls.Remove(other);
        }
    }
}
