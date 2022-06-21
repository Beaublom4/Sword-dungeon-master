using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    public DungeonPreset preset;
    [SerializeField] int lootTableType;

    [SerializeField] GameObject coinsPrefab;
    [SerializeField] int coinsDropMin, coinsDropMax;

    [SerializeField] float cost;
    [SerializeField] GameObject costObj;
    [SerializeField] Object[] drops;
    [SerializeField] Transform[] dropLocs;
    [SerializeField] TMP_Text[] dropPriceTexts;

    bool opened, collected;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Interact()
    {
        if (!opened)
            Open();
        else if (!collected)
            Buy();

    }
    public void Open()
    {
        opened = true;

        anim.SetTrigger("Open");

        int startDrop = 0;
        //Special drop
        float latestDropChance = 0;
        float randomDrop = Random.Range(0f, 100f);
        
        foreach (DungeonPreset.DropInfo drop in preset.lootTables[lootTableType].specialLootTable)
        {
            if (randomDrop >= latestDropChance && randomDrop < latestDropChance + drop.dropChance)
            {
                if (drop.drop != null)
                {
                    drops[0] = drop.drop;
                    InstantiateDrop(drop.drop, dropLocs[0]);
                    AddCost(drop.drop, 0);
                    startDrop++;
                }
                break;
            }
            latestDropChance += drop.dropChance;
        }
        //Other drops
        for (int i = startDrop; i < drops.Length; i++)
        {
            latestDropChance = 0;
            randomDrop = Random.Range(0f, 100f);
            foreach (DungeonPreset.DropInfo drop in preset.lootTables[lootTableType].normalLootTable)
            {
                if (randomDrop >= latestDropChance && randomDrop < latestDropChance + drop.dropChance)
                {
                    if (drop.drop != null)
                    {
                        drops[i] = drop.drop;
                        InstantiateDrop(drop.drop, dropLocs[i]);
                        AddCost(drop.drop, i);
                    }
                    else
                    {
                        Debug.Log("Money fill");
                        int coins = Random.Range(coinsDropMin, coinsDropMax);
                        Instantiate(coinsPrefab, dropLocs[i]);
                        dropPriceTexts[i].text = "+" + coins;
                        cost -= coins;
                    }
                    break;
                }
                latestDropChance += drop.dropChance;
            }
        }
        float showCost = cost;
        if (showCost < 0)
            showCost = 0;
        foreach(TMP_Text text in dropPriceTexts)
        {
            text.gameObject.SetActive(true);
        }
        costObj.GetComponent<TMP_Text>().text = showCost.ToString();
        costObj.SetActive(true);
        }
    void AddCost(Object drop, int i)
    {
        float dropCost = 0;
        if(drop is BladeScrObj)
        {
            BladeScrObj filteredDrop = (BladeScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is GuardScrObj)
        {
            GuardScrObj filteredDrop = (GuardScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is HandleScrObj)
        {
            HandleScrObj filteredDrop = (HandleScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is HelmetScrObj)
        {
            HelmetScrObj filteredDrop = (HelmetScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is ChestPlateScrObj)
        {
            ChestPlateScrObj filteredDrop = (ChestPlateScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is LeggingsScrObj)
        {
            LeggingsScrObj filteredDrop = (LeggingsScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        else if (drop is BootsScrObj)
        {
            BootsScrObj filteredDrop = (BootsScrObj)drop;
            dropCost = filteredDrop.cost;
        }
        cost += dropCost;
        dropPriceTexts[i].text = dropCost.ToString();
    }
    void InstantiateDrop(Object drop, Transform loc)
    {
        GameObject prefab = null;
        if (drop is BladeScrObj)
        {
            BladeScrObj filteredDrop = (BladeScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is GuardScrObj)
        {
            GuardScrObj filteredDrop = (GuardScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is HandleScrObj)
        {
            HandleScrObj filteredDrop = (HandleScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is HelmetScrObj)
        {
            HelmetScrObj filteredDrop = (HelmetScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is ChestPlateScrObj)
        {
            ChestPlateScrObj filteredDrop = (ChestPlateScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is LeggingsScrObj)
        {
            LeggingsScrObj filteredDrop = (LeggingsScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        else if (drop is BootsScrObj)
        {
            BootsScrObj filteredDrop = (BootsScrObj)drop;
            prefab = filteredDrop.prefab;
        }
        Instantiate(prefab, loc.position, loc.rotation, loc);
    }
    IEnumerator coroutine;
    public void ShowCost()
    {
        if (!opened || collected)
            return;

        if(coroutine != null)
            StopCoroutine(coroutine);
        coroutine = ShowCostRoutine();
        StartCoroutine(coroutine);
    }
    IEnumerator ShowCostRoutine()
    {
        costObj.SetActive(true);
        foreach(TMP_Text t in dropPriceTexts)
        {
            t.transform.parent.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        costObj.SetActive(false);
        foreach (TMP_Text t in dropPriceTexts)
        {
            t.transform.parent.gameObject.SetActive(false);
        }
    }
    public void Buy()
    {
        if (Inventory.coins < cost)
            return;

        collected = true;

        costObj.SetActive(false);
        foreach (TMP_Text t in dropPriceTexts)
        {
            t.transform.parent.gameObject.SetActive(false);
        }

        Inventory.coins -= cost;
        HudManager.Instance.ShowCoins();

        foreach (Transform t in dropLocs)
        {
            Destroy(t.gameObject);
        }
        foreach(Object o in drops)
        {
            Inventory.AddItem(o);
        }
    }
}
