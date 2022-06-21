using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceInfo : MonoBehaviour
{
    public TMP_Text resourceNumber;
    public TMP_Text resourceName;

    public Color goodColor, badColor;

    public void SetUp(string _name, int _count)
    {
        resourceName.text = _name;
        resourceNumber.text = _count.ToString();
    }
    public void Good()
    {
        resourceNumber.color = goodColor;
    }
    public void Bad()
    {
        resourceNumber.color = badColor;
    }
}
