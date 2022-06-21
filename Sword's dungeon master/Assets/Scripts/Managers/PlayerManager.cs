using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;

    public GameObject playerPrefab;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!pv.IsMine)
            return;

        Vector3 spawnPoint = FindObjectOfType<DungeonCreator>().GetSpawnPoint();
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);
    }
}
