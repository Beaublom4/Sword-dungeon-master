using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArmorChanger : MonoBehaviour
{
    public static ArmorChanger Instance;

    [SerializeField] Transform helmetLoc, chestPlateLoc, leggingLoc, bootsLoc;
    GameObject displayHelmet, displayChestPlate, displayLeggings, displayBoots;
    [SerializeField] Camera viewCamera;
    ArmorHolder armorHolder;
    [SerializeField] GameObject inventoryObj, scrollViewObj;
    [SerializeField] TMP_Text statsText;

    bool active;

    int currentSlot;
    [SerializeField] Transform slotsHolder;
    [SerializeField] GameObject slotPrefab;

    private void Awake()
    {
        Instance = this;
    }
    public void OpenChanger(ArmorHolder armorHolderScript)
    {
        armorHolder = armorHolderScript;
        viewCamera.enabled = true;
        active = true;
        inventoryObj.SetActive(true);
        ShowStats();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Start()
    {
        if (Loadout.helmet != null)
        {
            displayHelmet = Instantiate(Loadout.helmet.prefab, helmetLoc);
            displayHelmet.layer = 0;
            foreach (Transform t in displayHelmet.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        if(Loadout.chestPlate != null)
        {
            displayChestPlate = Instantiate(Loadout.chestPlate.prefab, chestPlateLoc);
            displayChestPlate.layer = 0;
            foreach (Transform t in displayChestPlate.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        if(Loadout.leggings != null)
        {
            displayLeggings = Instantiate(Loadout.leggings.prefab, leggingLoc);
            displayLeggings.layer = 0;
            foreach (Transform t in displayLeggings.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        if(Loadout.boots != null)
        {
            displayBoots = Instantiate(Loadout.boots.prefab, bootsLoc);
            displayBoots.layer = 0;
            foreach (Transform t in displayBoots.transform)
            {
                t.gameObject.layer = 0;
            }
        }
    }
    private void Update()
    {
        if (!active)
            return;

        if (Input.GetButtonDown("Cancel"))
        {
            viewCamera.enabled = false;
            active = false;
            scrollViewObj.SetActive(false);
            inventoryObj.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void OpenInventory(int slot)
    {
        int oldSlot = currentSlot;
        currentSlot = slot;

        if (scrollViewObj.activeSelf == false || slot != oldSlot)
        {
            foreach (Transform t in slotsHolder)
            {
                Destroy(t.gameObject);
            }

            if (slot == 3)
            {
                foreach (HelmetScrObj helmet in Inventory.helmets)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = helmet.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = helmet;
                    newSlot.GetComponent<WeaponSlot>().interactType = 1;
                }
            }
            else if (slot == 4)
            {
                foreach (ChestPlateScrObj chestPlate in Inventory.chestPlates)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = chestPlate.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = chestPlate;
                    newSlot.GetComponent<WeaponSlot>().interactType = 1;
                }
            }
            else if (slot == 5)
            {
                foreach (LeggingsScrObj leggings in Inventory.leggings)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = leggings.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = leggings;
                    newSlot.GetComponent<WeaponSlot>().interactType = 1;
                }
            }
            else if (slot == 6)
            {
                foreach (BootsScrObj boots in Inventory.boots)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = boots.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = boots;
                    newSlot.GetComponent<WeaponSlot>().interactType = 1;
                }
            }
        }

        scrollViewObj.SetActive(true);
    }
    public void SelectWeaponPiece(Object slotItem)
    {
        if (slotItem is HelmetScrObj)
        {
            if(Loadout.helmet != null)
                Stats.ChangeStats(-Loadout.helmet.addHealth, -Loadout.helmet.regeneration, -Loadout.helmet.defence, -Loadout.helmet.mana, 0, 0, 0, 0);

            HelmetScrObj currentHelmet = (HelmetScrObj)slotItem;
            if (Loadout.helmet == currentHelmet)
                return;

            Loadout.helmet = currentHelmet;

            Stats.ChangeStats(currentHelmet.addHealth, currentHelmet.regeneration, currentHelmet.defence, currentHelmet.mana, 0, 0, 0, 0);

            armorHolder.ShowPiece(currentHelmet.id);

            if (displayHelmet != null)
                Destroy(displayHelmet);
            displayHelmet = Instantiate(Loadout.helmet.prefab, helmetLoc);
            displayHelmet.layer = 0;
            foreach (Transform t in displayHelmet.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        else if (slotItem is ChestPlateScrObj)
        {
            if (Loadout.chestPlate != null)
                Stats.ChangeStats(-Loadout.chestPlate.addHealth, -Loadout.chestPlate.regeneration, -Loadout.chestPlate.defence, 0, 0, -Loadout.chestPlate.protection, -Loadout.chestPlate.specialProtection, 0);

            ChestPlateScrObj currentChestPlate = (ChestPlateScrObj)slotItem;
            if (Loadout.chestPlate == currentChestPlate)
                return;

            Loadout.chestPlate = currentChestPlate;

            Stats.ChangeStats(currentChestPlate.addHealth, currentChestPlate.regeneration, currentChestPlate.defence, 0, 0, currentChestPlate.protection, currentChestPlate.specialProtection, 0);

            armorHolder.ShowPiece(currentChestPlate.id);

            if (displayChestPlate != null)
                Destroy(displayChestPlate);
            displayChestPlate = Instantiate(Loadout.chestPlate.prefab, chestPlateLoc);
            displayChestPlate.layer = 0;
            foreach (Transform t in displayChestPlate.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        else if (slotItem is LeggingsScrObj)
        {
            if (Loadout.leggings != null)
                Stats.ChangeStats(-Loadout.leggings.addHealth, -Loadout.leggings.regeneration, -Loadout.leggings.defence, 0, -Loadout.leggings.manaRegen, 0, 0, 0);

            LeggingsScrObj currentLeggins = (LeggingsScrObj)slotItem;
            if (Loadout.leggings == currentLeggins)
                return;

            Loadout.leggings = currentLeggins;

            Stats.ChangeStats(currentLeggins.addHealth, currentLeggins.regeneration, currentLeggins.defence, 0, currentLeggins.manaRegen, 0, 0, 0);

            armorHolder.ShowPiece(currentLeggins.id);

            if(displayLeggings != null)
                Destroy(displayLeggings);
            displayLeggings = Instantiate(Loadout.leggings.prefab, leggingLoc);
            displayLeggings.layer = 0;
            foreach (Transform t in displayLeggings.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        else if (slotItem is BootsScrObj)
        {
            if (Loadout.boots != null)
                Stats.ChangeStats(-Loadout.boots.addHealth, -Loadout.boots.regeneration, -Loadout.boots.defence, 0, 0, 0, 0, -Loadout.boots.moveSpeedBoost);

            BootsScrObj currentBoots = (BootsScrObj)slotItem;
            if (Loadout.boots == currentBoots)
                return;

            Loadout.boots = currentBoots;

            Stats.ChangeStats(currentBoots.addHealth, currentBoots.regeneration, currentBoots.defence, 0, 0, 0, 0, currentBoots.moveSpeedBoost);

            armorHolder.ShowPiece(currentBoots.id);

            if (displayBoots != null)
                Destroy(displayBoots);
            displayBoots = Instantiate(Loadout.boots.prefab, bootsLoc);
            displayBoots.layer = 0;
            foreach (Transform t in displayBoots.transform)
            {
                t.gameObject.layer = 0;
            }
        }
        else
            return;

        GetComponent<AudioSource>().Play();
        Debug.Log("Selected: " + slotItem.name + " to slot " + currentSlot);
        scrollViewObj.SetActive(false);
        ShowStats();
    }
    void ShowStats()
    {
        statsText.text = "Health: " + Stats.health;
        statsText.text += "<br>Health regen: " + Stats.healthRegen;
        statsText.text += "<br>Defence: " + Stats.defence;
        statsText.text += "<br>Mana: " + Stats.mana;
        statsText.text += "<br>Mana regen: " + Stats.manaRegen;
    }
}
