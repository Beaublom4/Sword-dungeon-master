using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingOrb : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] GameObject dmgPrefab;
    public void Go()
    {
        Invoke("Destroy", 10);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponentInParent<EnemyHealth>().GetHit(damage, true, 0, .2f);
            DamageDisplay dmg = Instantiate(dmgPrefab, transform.position, Quaternion.identity, null).GetComponent<DamageDisplay>();
            dmg.dmg.text = damage.ToString();
            Destroy(gameObject);
        }
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
}
