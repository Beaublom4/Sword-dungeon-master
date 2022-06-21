using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadielsPillar : MonoBehaviour
{
    [SerializeField] Transform spawnLoc;
    [SerializeField] GameObject ballProjectilePrefab;
    [SerializeField] float projectileSpeed;

    GameObject currentOrb;
    Transform orbChild;
    bool canAttack;

    private void Update()
    {
        if(currentOrb != null)
        {
            float rotSpeed = 45 * Time.deltaTime;
            orbChild.transform.Rotate(rotSpeed, rotSpeed, rotSpeed);
        }
    }
    public void ShowPillarReady()
    {
        canAttack = true;
        currentOrb = Instantiate(ballProjectilePrefab, spawnLoc);
        orbChild = currentOrb.transform.GetChild(0);
    }
    public void DoPillarAttack()
    {
        if (!canAttack)
            return;
        canAttack = false;

        currentOrb.GetComponent<Rigidbody>().useGravity = true;
        currentOrb.GetComponent<Rigidbody>().AddRelativeForce(0, 0, projectileSpeed);
        currentOrb.GetComponent<FlyingOrb>().Go();

        GetComponentInParent<YadielsPillarManager>().PillarCooldown();
    }
    
}
