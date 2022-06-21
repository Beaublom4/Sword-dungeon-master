using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TMP_Text dmg;
    [SerializeField] float moveSpeed;
    [SerializeField] float timeAlive;

    [SerializeField] TMP_ColorGradient normalGradient, cridGradient;

    private void Start()
    {
        Destroy(gameObject, timeAlive);
    }
    private void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
            transform.Translate(0, moveSpeed * Time.deltaTime, 0, Space.World);
        }
    }
    public void IsCrit(bool b)
    {
        if (!b)
        {
            dmg.colorGradientPreset = normalGradient;
        }
        else
        {
            dmg.colorGradientPreset = cridGradient;
        }
    }
}
