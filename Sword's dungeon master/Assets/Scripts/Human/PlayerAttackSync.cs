using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttackSync : MonoBehaviour
{
    PhotonView pv;
    [SerializeField] PlayerLook lookScript;
    [SerializeField] AudioSource walkSound;
    bool walkSoundCooldown = false;

    private void Awake()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }
    public void StartHit()
    {
        if (!pv.IsMine)
            return;
        lookScript.StartHit();
    }
    public void EndHit()
    {
        if (!pv.IsMine)
            return;
        lookScript.EndHit();
    }
    public void MagicAttack()
    {
        lookScript.MagicAttack();
    }
    public void ComboOpen()
    {
        lookScript.combo = true;
    }
    public void ComboClose()
    {
        lookScript.combo = false;
    }
    public void WalkSound()
    {
        if (!walkSoundCooldown)
        {
            walkSoundCooldown = true;
            walkSound.pitch = Random.Range(.8f, 1.2f);
            walkSound.Play();
            Invoke("WalkCooldown", .5f);
        }
    }
    void WalkCooldown()
    {
        walkSoundCooldown = false;
    }
}
