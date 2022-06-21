using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance;
    public GameObject hudObj;

    [Header("Loading")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("Health")]
    public TMP_Text currentHealthText;
    public TMP_Text addHealthText;
    public TMP_Text maxHealthText;
    public Slider healthSlider;

    public GameObject godMode;
    public GameObject deadPanel;

    IEnumerator regenCoroutine;

    [Header("Boss Bar")]
    public GameObject splashScreenObj;
    public TMP_Text splashScreenName;
    [Space]
    public GameObject bossHealthBarObj;
    public TMP_Text bossNameText;
    public TMP_Text bossMaxHealthText;
    public TMP_Text bossCurrentHealthText;
    public Slider bossHealthSlider;

    [Header("Mana")]
    public TMP_Text currentManaText;
    public TMP_Text addManaText;
    public TMP_Text maxManaText;
    public Slider manaSlider;

    IEnumerator manaRegenCoroutine;

    [Header("Info bar")]
    public TMP_Text currentLobbyText;
    public TMP_Text purse;
    public TMP_Text currentObjective;
    public TMP_Text percentageCleared;
    public TMP_Text secretsFound;
    public TMP_Text timeExpired;

    [Header("Players")]
    public GameObject playerName;
    public Transform playerHolder;
    public GameObject interact;

    [Header("Drops")]
    public GameObject dropPrefab;
    public Transform dropItemHolder;

    [Header("Mini map")]
    [SerializeField] GameObject miniMapPanel;

    [Header("Settings")]
    [SerializeField] GameObject menuObj;
    [SerializeField] GameObject optionsObj;
    bool menu;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_InputField sensText;
    [SerializeField] Slider brightnessSlider;
    [Space]
    [SerializeField] AudioMixer mixer;
    [SerializeField] Volume volume;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if(miniMapPanel != null)
        {
            if (Input.GetButtonDown("Map"))
            {
                miniMapPanel.SetActive(!miniMapPanel.activeSelf);
            }
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (!FindObjectOfType<PlayerMove>() || PlayerInteract.inMenu)
                return;
            Menu(!menu);
        }
    }
    public void SetHealth(float currentHealth)
    {
        currentHealthText.text = currentHealth.ToString("F0");
        maxHealthText.text = Stats.health.ToString("F0");
        healthSlider.maxValue = Stats.health;
        healthSlider.value = currentHealth;
    }
    public void SetMana(float currentMana)
    {
        currentManaText.text = currentMana.ToString("F0");
        maxManaText.text = Stats.mana.ToString("F0");
        manaSlider.maxValue = Stats.mana;
        manaSlider.value = currentMana;
    }
    public void RegenHealth(float addHealth)
    {
        addHealthText.text = "+" + addHealth.ToString("F0");

        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);
        regenCoroutine = RegenShow();
        StartCoroutine(regenCoroutine);
    }
    public void RegenMana(float addMana)
    {
        addManaText.text = "+" + addMana.ToString("F0");

        if (manaRegenCoroutine != null)
            StopCoroutine(manaRegenCoroutine);
        manaRegenCoroutine = RegenShow();
        StartCoroutine(manaRegenCoroutine);
    }
    IEnumerator RegenShow()
    {
        addHealthText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        addHealthText.gameObject.SetActive(false);
    }
    IEnumerator ManaRegenShow()
    {
        addManaText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        addManaText.gameObject.SetActive(false);
    }
    public void ShowCoins()
    {
        purse.text = "Purse: " + Inventory.coins.ToString();
    }
    public void SetUpPercentage()
    {
        percentageCleared.text = "Rooms cleared: 0%";
    }
    public void PercentageCleared(int roomsToClear, int roomsCleared)
    {
        float percentage = (roomsCleared / roomsToClear);
        percentage *= 100;
        percentageCleared.text = "Rooms cleared: " + percentage.ToString("F2") + "%";
    }
    public void ShowSecrets(int totalSecrets, int secretsCollected)
    {
        secretsFound.text = "Secrets: " + secretsCollected + "/" + totalSecrets.ToString();
    }
    public void EnableBossBar(bool b)
    {
        bossHealthBarObj.SetActive(b);
    }
    public void SplashScreen(string bossName)
    {
        hudObj.SetActive(false);
        splashScreenName.text = bossName;
        splashScreenObj.SetActive(true);
        Invoke("DisableSplashScreen", 3);
    }
    void DisableSplashScreen()
    {
        hudObj.SetActive(true);
        splashScreenObj.SetActive(false);
    }
    public void SetBossHealthBar(float currentHealth, float maxHealth, string bossName)
    {
        bossNameText.text = bossName;
        bossHealthSlider.maxValue = maxHealth;
        bossHealthSlider.value = currentHealth;
    }

    public void Menu(bool b)
    {
        menu = b;
        if (b)
        {
            menuObj.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            FindObjectOfType<PlayerMove>().enabled = false;
            FindObjectOfType<PlayerLook>().enabled = false;
        }
        else
        {
            menuObj.SetActive(false);
            optionsObj.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            FindObjectOfType<PlayerMove>().enabled = true;
            FindObjectOfType<PlayerLook>().enabled = true;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SetUpMenu()
    {
        sensitivitySlider.value = PlayerLook.staticSens;
        sensText.text = PlayerLook.staticSens.ToString("F0");
        LiftGammaGain color;
        volume.profile.TryGet<LiftGammaGain>(out color);
        brightnessSlider.value = color.gamma.value.z - 1;
    }
    public void Sounds(float sound)
    {
        mixer.SetFloat("Master", Mathf.Log10(sound) * 20);
    }
    public void Sens(string _sens)
    {
        float sens = float.Parse(_sens);
        Sensitivity(sens);
        sensitivitySlider.value = sens;
    }
    public void Sensitivity(float sens)
    {
        FindObjectOfType<PlayerLook>().sensitivity = sens;
        PlayerLook.staticSens = sens;
        sensText.text = sens.ToString("F0");
    }
    public void Brightness(float brightness)
    {
        LiftGammaGain color;
        volume.profile.TryGet<LiftGammaGain>(out color);
        color.gamma.value = new Vector4(0, 0, 0, brightness);
    }
}
