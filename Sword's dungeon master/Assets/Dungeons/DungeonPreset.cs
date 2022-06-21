using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Dungeon")]
public class DungeonPreset : ScriptableObject
{
    [System.Serializable]
    public class DropInfo
    {
        public Object drop;
        public float dropChance;
    }
    [System.Serializable]
    public class LootTables
    {
        public string lootTableName;
        public DropInfo[] specialLootTable;
        public DropInfo[] normalLootTable;
    }

    public int floorNumber;
    public Vector2 gridSize;
    public int puzzleRooms;

    [Header("Mini bosses")]
    public int miniBossCount;
    [System.Serializable]
    public class MiniBoss
    {
        public GameObject boss;
        public float spawnChance;
    }
    public MiniBoss[] miniBosses;

    [Header("Chest loot tables")]
    public LootTables[] lootTables;
}
