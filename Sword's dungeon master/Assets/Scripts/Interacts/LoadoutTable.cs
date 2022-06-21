using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadoutTable : MonoBehaviour
{
    public static LoadoutTable Instance;

    [Header("Display")]
    [SerializeField] Transform bladeLoc, guardLoc, handleLoc;
    GameObject displayBlade, displayGuard, displayHandle;
    [SerializeField] Camera topCamera;
    [SerializeField] GameObject inventoryObj, scrollViewObj;
    [SerializeField] SwordHolder swordHolder;
    [SerializeField] TMP_Text statsText;

    bool active;
    PlayerInteract currentInteract;

    [Header("InventoryDisplay")]
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotsHolder;

    int currentSlot;

    [Header("Loadout")]
    [SerializeField] BladeScrObj standardBlade;
    [SerializeField] GuardScrObj standardGuard;
    [SerializeField] HandleScrObj standardHandle;
    [SerializeField] MagicScrObj standardMagic;

    private void Awake()
    {
        Instance = this;

        if(Loadout.blade == null)
        {
            Loadout.blade = standardBlade;
            Loadout.guard = standardGuard;
            Loadout.handle = standardHandle;
            Loadout.magic = standardMagic;

            Inventory.blades.Add(standardBlade);
            Inventory.guards.Add(standardGuard);
            Inventory.handles.Add(standardHandle);
        }
    }
    private void Start()
    {
        displayBlade = Instantiate(Loadout.blade.prefab, bladeLoc);
        displayGuard = Instantiate(Loadout.guard.prefab, guardLoc);
        displayHandle = Instantiate(Loadout.handle.prefab, handleLoc);
    }
    public void OpenChanger(SwordHolder swordHolderScript)
    {
        swordHolder = swordHolderScript;
        topCamera.enabled = true;
        active = true;
        DisplayStats();
        inventoryObj.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (!active)
            return;

        if (Input.GetButtonDown("Cancel"))
        {
            topCamera.enabled = false;
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

            if (slot == 0)
            {
                foreach (BladeScrObj blade in Inventory.blades)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = blade.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = blade;
                }
            }
            else if (slot == 1)
            {
                foreach (GuardScrObj guard in Inventory.guards)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = guard.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = guard;
                }
            }
            else if (slot == 2)
            {
                foreach (HandleScrObj handle in Inventory.handles)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = handle.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = handle;
                }
            }
            else if(slot == 7)
            {
                foreach (MagicScrObj magic in Inventory.magics)
                {
                    GameObject newSlot = Instantiate(slotPrefab, slotsHolder);
                    newSlot.GetComponent<WeaponSlot>().slotName.text = magic.name;
                    newSlot.GetComponent<WeaponSlot>().slotItem = magic;
                }
            }
        }

        scrollViewObj.SetActive(true);
    }
    public void SelectWeaponPiece(Object slotItem)
    {
        GetComponent<AudioSource>().Play();
        if(currentSlot == 0)
        {
            if (slotItem is BladeScrObj)
            {
                BladeScrObj currentBlade = (BladeScrObj)slotItem;
                Loadout.blade = currentBlade;
                swordHolder.ShowPiece(currentBlade.id);

                Destroy(displayBlade);
                displayBlade = Instantiate(Loadout.blade.prefab, bladeLoc);
            }
            else
                return;
        }
        else if(currentSlot == 1)
        {
            if (slotItem is GuardScrObj)
            {
                GuardScrObj currentGuard = (GuardScrObj)slotItem;
                Loadout.guard = currentGuard;
                swordHolder.ShowPiece(currentGuard.id);

                Destroy(displayGuard);
                displayGuard = Instantiate(Loadout.guard.prefab, guardLoc);
            }
            else
                return;
        }
        else if(currentSlot == 2)
        {
            if (slotItem is HandleScrObj)
            {
                HandleScrObj currentHandle = (HandleScrObj)slotItem;
                Loadout.handle = currentHandle;
                swordHolder.ShowPiece(currentHandle.id);

                Destroy(displayHandle);
                displayHandle = Instantiate(Loadout.handle.prefab, handleLoc);
            }
            else
                return;
        }
        else if (currentSlot == 7)
        {
            if (slotItem is MagicScrObj)
            {
                MagicScrObj currentMagic = (MagicScrObj)slotItem;
                Loadout.magic = currentMagic;
                swordHolder.ShowPiece(currentMagic.id);

                //Destroy(displayMagic);
                //displayMagic = Instantiate(Loadout.magic.prefab, magicLoc);
            }
            else
                return;
        }
        Debug.Log("Selected: " + slotItem.name + " to slot " + currentSlot);
        scrollViewObj.SetActive(false);
        DisplayStats();
    }
    void DisplayStats()
    {
        statsText.text = "Blade:" + "<br>";
        statsText.text += "<br>Dmg: " + Loadout.blade.damage;
        statsText.text += "<br>Crit dmg: " + Loadout.blade.critDamage;
        statsText.text += "<br>Crit chance: " + Loadout.blade.critChance;
        float magicDmg = Loadout.magic.damage + Loadout.blade.reinforceMagic;
        statsText.text += "<br>Magic damage: " + magicDmg;
        statsText.text += "<br>Bleeding time: " + Loadout.blade.bleedTime;
        statsText.text += "<br>Lifesteel: " + Loadout.blade.lifeStealPercentage + "%";
        statsText.text += "<br>" + "<br>Guard:" + "<br>";
        statsText.text += "<br>Stagger time: " + Loadout.guard.staggerTime;
        statsText.text += "<br>Mana discount: " + Loadout.guard.manaDiscount;
        statsText.text += "<br>" + "<br>Handle:" + "<br>";
        statsText.text += "<br>Attack speed: " + Loadout.handle.attackSpeed;
        float manaRegen = Loadout.handle.manaRegen + Stats.manaRegen;
        statsText.text += "<br>Mana regen: " + manaRegen;
    }
}
