using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static float coins;

    public static List<BladeScrObj> blades = new List<BladeScrObj>();
    public static List<GuardScrObj> guards = new List<GuardScrObj>();
    public static List<HandleScrObj> handles = new List<HandleScrObj>();

    public static List<MagicScrObj> magics = new List<MagicScrObj>();

    public static List<HelmetScrObj> helmets = new List<HelmetScrObj>();
    public static List<ChestPlateScrObj> chestPlates = new List<ChestPlateScrObj>();
    public static List<LeggingsScrObj> leggings = new List<LeggingsScrObj>();
    public static List<BootsScrObj> boots = new List<BootsScrObj>();

    [System.Serializable]
    public class ItemType
    {
        public ItemScrObj item;
        public int countInInv;
    }
    public static List<ItemType> items = new List<ItemType>();

    public float addCoins;
    public Object addItem;

    public Object[] allItemsCheat;
    public Object[] craftingItems;

    private void Update()
    {
        if(addCoins > 0)
        {
            AddCoins(addCoins);
            addCoins = 0;
        }
        if(addItem != null)
        {
            AddItem(addItem);
            addItem = null;
        }

        if (!FindObjectOfType<PlayerHealth>())
            return;
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach(Object o in allItemsCheat)
            {
                AddItem(o);
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (Object o in craftingItems)
            {
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
                AddItem(o);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddCoins(100000);
        }
    }
    public void AddCoins(float _coins)
    {
        coins += _coins;
        HudManager.Instance.purse.text = "Purse: " + coins.ToString();
    }
    public static void AddItem(Object _addItem)
    {
        bool didAdd = false;

        if(_addItem is ItemScrObj)
        {
            didAdd = true;
            bool hasItem = false;
            ItemScrObj newItem = (ItemScrObj)_addItem;
            foreach (ItemType item in items)
            {
                if(item.item.id == newItem.id)
                {
                    hasItem = true;
                    item.countInInv++;
                    break;
                }
            }

            if (!hasItem) {
                ItemType newType = new ItemType();
                newType.item = newItem;
                newType.countInInv = 1;

                items.Add(newType);
            }
        }
        else if (_addItem is BladeScrObj)
        {
            blades.Add((BladeScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is GuardScrObj)
        {
            guards.Add((GuardScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is HandleScrObj)
        {
            handles.Add((HandleScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is HelmetScrObj)
        {
            helmets.Add((HelmetScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is ChestPlateScrObj)
        {
            chestPlates.Add((ChestPlateScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is LeggingsScrObj)
        {
            leggings.Add((LeggingsScrObj)_addItem);
            didAdd = true;
        }
        else if (_addItem is BootsScrObj)
        {
            boots.Add((BootsScrObj)_addItem);
            didAdd = true;
        }

        if (didAdd)
        {
            DropInfo currentDrop = Instantiate(HudManager.Instance.dropPrefab, HudManager.Instance.dropItemHolder).GetComponent<DropInfo>();
            currentDrop.ShowDrop(_addItem.name);
        }
    }
}