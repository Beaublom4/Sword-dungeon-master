using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropInfo : MonoBehaviour
{
    [SerializeField] TMP_Text dropName;

    public void ShowDrop(string _dropName)
    {
        dropName.text = _dropName;
        Destroy(gameObject, 3);
    }
}
