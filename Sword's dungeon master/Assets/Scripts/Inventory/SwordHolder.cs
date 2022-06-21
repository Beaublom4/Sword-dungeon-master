using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordHolder : MonoBehaviour
{
    PhotonView pv;

    [SerializeField] PlayerLook lookScript;

    [SerializeField] ItemOnBody[] blades;
    ItemOnBody currentBlade;
    [SerializeField] ItemOnBody[] guards;
    ItemOnBody currentGuard;
    [SerializeField] ItemOnBody[] handles;
    ItemOnBody currentHandle;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!pv.IsMine)
            return;

        ShowPiece(Loadout.blade.id);
        ShowPiece(Loadout.guard.id);
        ShowPiece(Loadout.handle.id);
    }
    public void ShowPiece(int id)
    {
        Choose(id);
        pv.RPC("RPC_SyncWeapon", RpcTarget.OthersBuffered, id);
    }
    [PunRPC]
    void RPC_SyncWeapon(int id)
    {
        Choose(id);
    }
    void Choose(int id)
    {
        if (id <= 100)
        {
            //Blades
            if (currentBlade != null)
                currentBlade.gameObject.SetActive(false);
            foreach (ItemOnBody item in blades)
            {
                if (item.id == id)
                {
                    item.gameObject.SetActive(true);
                    lookScript.currentBlade = item.GetComponent<Blade>();
                    currentBlade = item;
                    return;
                }
            }
        }
        else if (id > 100 && id <= 200)
        {
            //Guards
            if (currentGuard != null)
                currentGuard.gameObject.SetActive(false);
            foreach (ItemOnBody item in guards)
            {
                if (item.id == id)
                {
                    item.gameObject.SetActive(true);
                    currentGuard = item;
                    return;
                }
            }
        }
        else if (id > 200 && id <= 300)
        {
            //Handles
            if (currentHandle != null)
                currentHandle.gameObject.SetActive(false);
            foreach (ItemOnBody item in handles)
            {
                if (item.id == id)
                {
                    item.gameObject.SetActive(true);
                    currentHandle = item;
                    return;
                }
            }
        }
    }
}
