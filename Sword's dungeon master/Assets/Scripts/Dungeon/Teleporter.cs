using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Teleporter : MonoBehaviour
{
    [SerializeField] bool isBossTeleport;
    [SerializeField] bool isChestTeleport;
    [SerializeField] bool isHubTeleport;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (isBossTeleport)
                DungeonCreator.Instance.GoToBossRoom(other.transform.root);
            else if (isChestTeleport)
                DungeonCreator.Instance.GoToChestRoom(other.transform.root);
            else if (isHubTeleport)
                PhotonNetwork.LoadLevel(0);
        }
    }
}
