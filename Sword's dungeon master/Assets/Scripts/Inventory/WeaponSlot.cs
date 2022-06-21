using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlot : MonoBehaviour
{
    public TMP_Text slotName;
    public Object slotItem;
    public int interactType;

    public void SelectSlot()
    {
        if (interactType == 0)
            LoadoutTable.Instance.SelectWeaponPiece(slotItem);
        else
            ArmorChanger.Instance.SelectWeaponPiece(slotItem);
    }
}
