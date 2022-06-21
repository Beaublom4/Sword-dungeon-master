using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLook : MonoBehaviour
{
    PhotonView pv;

    [Header("Looking")]
    public float sensitivity;
    public static float staticSens;
    [SerializeField] Transform parent;
    [SerializeField] Animator anim;
    [SerializeField] Camera[] cameras;

    [Header("Attacking")]
    public Blade currentBlade;
    bool canAttack = true;
    public bool combo = false;
    IEnumerator cooldown;
    [SerializeField] AudioSource hit;

    public GameObject dmgShowPrefab;

    [Header("Magic")]
    public float currentMana;
    bool canMagic = true;
    [SerializeField] float manaRegenRate;
    [SerializeField] Transform magicLoc;

    [Header("CharacterVisuality")]
    [SerializeField] GameObject[] objInvisForPlayer;
    [SerializeField] GameObject[] objRenderInfrontOff;

    Vector2 mouseInput;
    float xRot;
    private void Awake()
    {
        pv = transform.root.GetComponent<PhotonView>();
        if (staticSens == 0)
            staticSens = sensitivity;
        else
            sensitivity = staticSens;
    }
    private void Start()
    {
        if (!pv.IsMine)
        {
            foreach (Camera c in cameras)
            {
                c.enabled = false;
            }
            GetComponentInChildren<AudioListener>().enabled = false;
            foreach(GameObject g in objRenderInfrontOff)
            {
                g.layer = 7;
            }
            return;
        }

        transform.GetChild(0).tag = "MainCamera";

        foreach (GameObject g in objInvisForPlayer)
        {
            g.layer = 8;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentMana = Stats.mana;
        HudManager.Instance.SetMana(currentMana);

        InvokeRepeating("ManaRegenRoutine", 0, manaRegenRate);
    }
    private void Update()
    {
        if (!pv.IsMine)
            return;

        LookRotation();
        AttackInputs();
    }
    void ManaRegenRoutine()
    {
        if(currentMana < Stats.mana)
        {
            float regenMana = Stats.manaRegen + Loadout.handle.manaRegen;
            currentMana += regenMana;
            if (currentMana > Stats.mana)
                currentMana = Stats.mana;

            HudManager.Instance.RegenMana(regenMana);
            HudManager.Instance.SetMana(currentMana);
        }
    }
    void LookRotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        xRot += mouseInput.y * sensitivity * Time.deltaTime;

        xRot = Mathf.Clamp(xRot, -90, 90);

        parent.Rotate(0, mouseInput.x * sensitivity * Time.deltaTime, 0, 0);
        anim.SetFloat("XRot", xRot);
        //transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
    void AttackInputs()
    {
        if(!combo)
            if (!canAttack || !canMagic)
                return;

        if (Input.GetButtonDown("Fire1"))
        {
            combo = false;

            currentBlade.damageMultiplier = 1;
            currentBlade.lookScript = this;
            anim.SetFloat("AttackSpeed", Loadout.handle.attackSpeed);
            anim.SetInteger("AttackType", Loadout.handle.attackType);
            anim.SetTrigger("Lite");
            if (cooldown != null)
                StopCoroutine(cooldown);
            cooldown = AttackCooldown(1f / Loadout.handle.attackSpeed);
            StartCoroutine(cooldown);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            combo = false;

            currentBlade.damageMultiplier = 2;
            anim.SetFloat("AttackSpeed", Loadout.handle.attackSpeed);
            anim.SetInteger("AttackType", Loadout.handle.attackType);
            anim.SetTrigger("Heavy");
            if (cooldown != null)
                StopCoroutine(cooldown);
            cooldown = AttackCooldown(2f / Loadout.handle.attackSpeed);
            StartCoroutine(cooldown);
        }
        else if (Input.GetButtonDown("Magic") && canMagic)
        {
            anim.SetFloat("MagicType", Loadout.magic.magicType);
            anim.SetFloat("MagicCastSpeed", .5f / Loadout.magic.castTime);
            anim.SetTrigger("MagicAttack");
            StartCoroutine(MagicCooldown());
        }
    }
    public void StartHit()
    {
        currentBlade.StartHit();
    }
    public void EndHit()
    {
        currentBlade.EndHit();
    }
    IEnumerator AttackCooldown(float cooldownTime)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }
    IEnumerator MagicCooldown()
    {
        canMagic = false;
        yield return new WaitForSeconds(Loadout.magic.castTime);
        canMagic = true;
    }
    public void MagicAttack()
    {
        MagicScrObj currentMagic = Loadout.magic;
        float calcMana = currentMagic.manaUsage - Loadout.guard.manaDiscount;
        if (calcMana < 0)
            calcMana = 0;
        if (currentMana >= calcMana)
        {
            currentMana -= calcMana;
            HudManager.Instance.SetMana(currentMana);

            MagicProjectile currentProj = Instantiate(currentMagic.magicPrefab, magicLoc.position, transform.rotation, null).GetComponent<MagicProjectile>();
            currentProj.SetUp(currentMagic, Loadout.blade.reinforceMagic);
        }
    }
    public void HitSound()
    {
        hit.Play();
    }
}
