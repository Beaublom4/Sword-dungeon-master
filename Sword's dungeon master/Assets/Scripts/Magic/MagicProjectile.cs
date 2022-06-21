using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public MagicScrObj magic;
    public float reinforceMagic;

    [SerializeField] GameObject dmgPrefab;  

    public bool isExplosive;

    public void SetUp(MagicScrObj _magicScrObj, float _reinforceMagic)
    {
        magic = _magicScrObj;
        reinforceMagic = _reinforceMagic;
        Invoke("Destroy", _magicScrObj.timeAlive);
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        transform.Translate(0, 0, magic.speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            float calcDmg = magic.damage + reinforceMagic;

            other.GetComponentInParent<EnemyHealth>().GetHit(calcDmg, true, 0, .2f);
            DamageDisplay dmg = Instantiate(dmgPrefab, transform.position, Quaternion.identity, null).GetComponent<DamageDisplay>();
            dmg.dmg.text = calcDmg.ToString();
            Destroy(gameObject);
        }
    }
}
