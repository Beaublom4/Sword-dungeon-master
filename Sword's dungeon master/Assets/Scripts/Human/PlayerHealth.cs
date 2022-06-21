using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;

public class PlayerHealth : MonoBehaviour
{
    PhotonView pv;

    bool godMode = false;
    bool dead = false;
    public float currentHealth;
    [SerializeField] float regenRate = 5;

    IEnumerator coroutine;
    Volume getHitVolume;

    PlayerMove playerMove;
    PlayerLook playerLook;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        getHitVolume = GameObject.FindGameObjectWithTag("GetHitVolume")?.GetComponent<Volume>();

        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponentInChildren<PlayerLook>();
    }
    private void Start()
    {
        if (!pv.IsMine)
            return;

        SetHealth();
        InvokeRepeating("RegenRoutine", 0, regenRate);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            godMode = !godMode;
            HudManager.Instance.godMode.SetActive(godMode);
        }
    }
    public void SetHealth()
    {
        currentHealth = Stats.health;
        HudManager.Instance.SetHealth(currentHealth);
    }
    public void GetHit(float damage)
    {
        if (dead || godMode)
            return;

        float calcDamage = damage - Stats.defence;

        if (calcDamage < 0)
            calcDamage = 0;

        currentHealth -= calcDamage;

        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = HitShow();
        StartCoroutine(coroutine);

        HudManager.Instance.SetHealth(currentHealth);
        if(FindObjectOfType<DungeonManager>())
            DungeonManager.Instance.ChangePlayerInfo(PhotonNetwork.LocalPlayer, currentHealth, Stats.health);

        if (currentHealth <= 0)
        {
            if (FindObjectOfType<DungeonCreator>())
            {
                playerMove.enabled = false;
                playerLook.enabled = false;

                GetComponentInChildren<Animator>().SetBool("Dead", true);

                dead = true;
                HudManager.Instance.deadPanel.SetActive(true);
                Invoke("DeadCooldown", 2);
            }
        }
    }
    void DeadCooldown()
    {
        GetComponentInChildren<Animator>().SetBool("Dead", false);
        HudManager.Instance.deadPanel.SetActive(false);
        DungeonManager.Instance.SpawnChests(true);
        DungeonCreator.Instance.GoToChestRoom(transform);
        dead = false;

        playerMove.enabled = true;
        playerLook.enabled = true;
    }
    IEnumerator HitShow()
    {
        getHitVolume.weight = 1;
        while(getHitVolume.weight > 0)
        {
            getHitVolume.weight -= .5f * Time.deltaTime;
            yield return null;
        }
        getHitVolume.weight = 0;
    }
    public void LifeSteal(float lifeStealHealth)
    {
        if (currentHealth < Stats.health)
        {
            currentHealth += lifeStealHealth;
            if (currentHealth > Stats.health)
                currentHealth = Stats.health;

            HudManager.Instance.RegenHealth(lifeStealHealth);
            HudManager.Instance.SetHealth(currentHealth);

            if (DungeonManager.Instance != null)
                DungeonManager.Instance.ChangePlayerInfo(PhotonNetwork.LocalPlayer, currentHealth, Stats.health);
        }
    }
    void RegenRoutine()
    {
        if(currentHealth < Stats.health)
        {
            float regenHealth = Stats.healthRegen;
            currentHealth += regenHealth;
            if (currentHealth > Stats.health)
                currentHealth = Stats.health;

            HudManager.Instance.RegenHealth(regenHealth);
            HudManager.Instance.SetHealth(currentHealth);

            if (DungeonManager.Instance != null)
                DungeonManager.Instance.ChangePlayerInfo(PhotonNetwork.LocalPlayer, currentHealth, Stats.health);
        }
    }
}
