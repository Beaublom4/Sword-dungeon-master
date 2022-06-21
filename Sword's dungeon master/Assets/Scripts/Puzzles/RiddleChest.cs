using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiddleChest : MonoBehaviour
{
    Animator anim;
    Riddle riddle;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        riddle = GetComponentInParent<Riddle>();
    }
    public void OpenChest()
    {
        if (riddle.locked)
            return;
        anim.SetTrigger("Open");
        GetComponentInParent<Riddle>().OpenChest(transform);
    }
}
