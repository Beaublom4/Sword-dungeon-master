using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRadius : MonoBehaviour
{
    public bool inRange;
    public Transform player;
    public List<Collider> playerColls = new List<Collider>();
    private void Update()
    {
        if (playerColls.Count > 0)
        {
            inRange = true;
        }
        else if (inRange)
        {
            inRange = false;
            player = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerColls.Add(other);
            player = other.transform.root;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerColls.Remove(other);
        }
    }
}
