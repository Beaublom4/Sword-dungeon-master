using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorHolder : MonoBehaviour
{
    [System.Serializable]
    public class ArmorPiece
    {
        public int id;
        public GameObject[] piece;
    }

    [SerializeField] ItemOnBody[] helmets;
    ItemOnBody currentHelmet;
    [SerializeField] ArmorPiece[] chestPlates;
    GameObject[] currentWearingChestPlate;
    GameObject currentChestPlate;
    [SerializeField] ArmorPiece[] leggings;
    GameObject[] currentWearingLeggings;
    GameObject currentLeggins;
    [SerializeField] ArmorPiece[] boots;
    GameObject[] currentWearingBoots;
    GameObject currentBoots;

    private void Start()
    {
        if(Loadout.helmet != null)
            ShowPiece(Loadout.helmet.id);
        if (Loadout.chestPlate != null)
            ShowPiece(Loadout.chestPlate.id);
        if (Loadout.leggings != null)
            ShowPiece(Loadout.leggings.id);
        if (Loadout.boots != null)
            ShowPiece(Loadout.boots.id);
    }
    public void ShowPiece(int id)
    {
        if (id > 300 && id <= 400)
        {
            //Helmets
            if (currentHelmet != null)
                currentHelmet.gameObject.SetActive(false);

            foreach (ItemOnBody item in helmets)
            {
                if (item.id == id)
                {
                    item.gameObject.layer = 8;
                    foreach(Transform t in item.transform)
                    {
                        t.gameObject.layer = 8;
                    }
                    item.gameObject.SetActive(true);
                    currentHelmet = item;
                    break;
                }
            }
        }
        else if(id > 400 && id <= 500)
        {
            //Chestplate
            if (currentWearingChestPlate != null)
                foreach (GameObject g in currentWearingChestPlate)
                {
                    g.SetActive(false);
                }

            foreach(ArmorPiece piece in chestPlates)
            {
                if(piece.id == id)
                {
                    List<GameObject> pieces = new List<GameObject>();
                    foreach (GameObject g in piece.piece)
                    {
                        g.SetActive(true);
                        pieces.Add(g);
                    }
                    currentWearingChestPlate = pieces.ToArray();
                }
            }
        }
        else if (id > 500 && id <= 600)
        {
            //Leggings
            if (currentWearingLeggings != null)
                foreach (GameObject g in currentWearingLeggings)
                {
                    g.SetActive(false);
                }

            foreach (ArmorPiece piece in leggings)
            {
                if (piece.id == id)
                {
                    List<GameObject> pieces = new List<GameObject>();
                    foreach (GameObject g in piece.piece)
                    {
                        g.SetActive(true);
                        pieces.Add(g);
                    }
                    currentWearingLeggings = pieces.ToArray();
                }
            }
        }
        else if (id > 600 && id <= 700)
        {
            //Boots
            if (currentWearingBoots != null)
                foreach (GameObject g in currentWearingBoots)
                {
                    g.SetActive(false);
                }

            foreach (ArmorPiece piece in boots)
            {
                if (piece.id == id)
                {
                    List<GameObject> pieces = new List<GameObject>();
                    foreach (GameObject g in piece.piece)
                    {
                        g.SetActive(true);
                        pieces.Add(g);
                    }
                    currentWearingBoots = pieces.ToArray();
                }
            }
        }
        GetComponentInParent<PlayerHealth>().SetHealth();
    }
}
