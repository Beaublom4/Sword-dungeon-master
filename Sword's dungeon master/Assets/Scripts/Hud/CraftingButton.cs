using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingButton : MonoBehaviour
{
    public TMP_Text recepieName;
    public RecepieScrObj recepie;
    public void SelectRecepie()
    {
        Crafter.Instance.SelectRecepie(recepie);
    }
}
