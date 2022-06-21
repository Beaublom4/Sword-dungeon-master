using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorInfo : MonoBehaviour
{
    public TMP_Text floorName;
    public DungeonPreset floor;

    public void SelectFloor()
    {
        HubManager.Instance.StartDungeon(this);
    }
}
