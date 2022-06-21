using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInteract : MonoBehaviour
{
    PhotonView pv;

    [SerializeField] float interactOffset;
    [SerializeField] float interactSize;
    [SerializeField] LayerMask interactMask;

    [SerializeField] Camera yourCam;

    [SerializeField] PlayerMove moveScript;
    [SerializeField] PlayerLook lookScript;
    [SerializeField] SwordHolder swordHolderScript;
    [SerializeField] ArmorHolder armorHolderScript;
    public static bool inMenu;

    private void Awake()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }
    private void Start()
    {
        inMenu = false;
        HudManager.Instance.ShowCoins();
    }
    private void Update()
    {
        if (inMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                EnableMovement(true, true);
            }
            return;
        }

        Hover();
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * interactOffset, interactSize);
    }
    void Hover()
    {
        Vector3 checkPos = transform.position + transform.forward * interactOffset;
        Collider[] cols = Physics.OverlapSphere(checkPos, interactSize, interactMask);

        float currentNearestFloat = 0;
        Collider currentNearestCol = null;
        foreach (Collider col in cols)
        {
            if (currentNearestCol == null)
            {
                currentNearestFloat = Vector3.Distance(checkPos, col.transform.position);
                currentNearestCol = col;
            }
            else
            {
                float newNearestFloat = Vector3.Distance(checkPos, col.transform.position);
                if (newNearestFloat < currentNearestFloat)
                {
                    currentNearestFloat = newNearestFloat;
                    currentNearestCol = col;
                }
            }
        }
        if (currentNearestCol == null)
        {
            HudManager.Instance.interact.SetActive(false);
            return;
        }

        if (currentNearestCol.gameObject.layer == 9)
        {
            HudManager.Instance.interact.SetActive(true);
        }
        else if (HudManager.Instance.interact.activeSelf == true)
            HudManager.Instance.interact.SetActive(false);
    }
    void Interact()
    {
        Vector3 checkPos = transform.position + transform.forward * interactOffset;
        Collider[] cols = Physics.OverlapSphere(checkPos, interactSize, interactMask);

        float currentNearestFloat = 0;
        Collider currentNearestCol = null;
        foreach(Collider col in cols)
        {
            if(currentNearestCol == null)
            {
                currentNearestFloat = Vector3.Distance(checkPos, col.transform.position);
                currentNearestCol = col;
            }
            else
            {
                float newNearestFloat = Vector3.Distance(checkPos, col.transform.position);
                if(newNearestFloat < currentNearestFloat)
                {
                    currentNearestFloat = newNearestFloat;
                    currentNearestCol = col;
                }
            }
        }

        if (currentNearestCol == null)
            return;

        if (currentNearestCol.GetComponent<LoadoutTable>())
        {
            EnableMovement(false, false);
            currentNearestCol.GetComponent<LoadoutTable>().OpenChanger(swordHolderScript);
        }
        else if (currentNearestCol.GetComponent<ArmorChanger>())
        {
            EnableMovement(false, false);
            currentNearestCol.GetComponent<ArmorChanger>().OpenChanger(armorHolderScript);
        }
        else if(currentNearestCol.GetComponent<Crafter>())
        {
            EnableMovement(false, false);
            currentNearestCol.GetComponent<Crafter>().OpenChanger();
        }
        else if (currentNearestCol.GetComponent<Chest>())
        {
            currentNearestCol.GetComponent<Chest>().Interact();
        }
        else if(currentNearestCol.gameObject.tag == "DungeonDoor")
        {
            EnableMovement(false, true);
            HubManager.Instance.OpenDungeonManager();
        }
        else if(currentNearestCol.gameObject.tag == "DungeonMaster")
        { 
            DungeonManager.Instance.ReadyUp();
        }
        else if (currentNearestCol.GetComponent<Secret>())
        {
            DungeonManager.Instance.PickUpSecret(currentNearestCol.GetComponent<Secret>());
        }
        else if (currentNearestCol.GetComponent<LazerCatcher>())
        {
            currentNearestCol.GetComponent<LazerCatcher>().Rotate();
        }
        else if (currentNearestCol.GetComponent<StartingLazer>())
        {
            currentNearestCol.GetComponent<StartingLazer>().DoLazer();
        }
        else if (currentNearestCol.GetComponent<RiddleChest>())
        {
            currentNearestCol.GetComponent<RiddleChest>().OpenChest();
        }
        else if (currentNearestCol.GetComponent<Helper>())
        {
            currentNearestCol.GetComponent<Helper>().Talk();
        }
    }
    void EnableMovement(bool movement, bool camera)
    {
        inMenu = !movement;
        moveScript.enabled = movement;
        lookScript.enabled = movement;
        yourCam.enabled = camera;
    }
}
