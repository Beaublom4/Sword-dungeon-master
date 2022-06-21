using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recepie")]
public class RecepieScrObj : ScriptableObject
{
    [System.Serializable]
    public class ItemNeeded
    {
        public ItemScrObj item;
        public int count;
    }

    public Object item;
    public ItemNeeded[] itemsNeeded;
    public string recipeName;
    [TextArea]
    public string info;
}
