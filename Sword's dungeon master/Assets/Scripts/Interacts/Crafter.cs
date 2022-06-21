using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Crafter : MonoBehaviour
{
    [System.Serializable]
    public class ItemForRecipe
    {
        public Inventory.ItemType itemType;
        public int countToRemove;
    }

    public static Crafter Instance;

    [Header("Display")]
    [SerializeField] Camera topCamera;
    [SerializeField] GameObject inventoryObj;
    bool active;
    [SerializeField] GameObject recepieButtonPrefab;
    [SerializeField] Transform recepieButtonHolder;

    [Header("Craft")]
    [SerializeField] Slider craftSlider;
    bool crafting, enoughItems;

    [SerializeField] GameObject notEnoughItems;
    List<ItemForRecipe> itemsForRecipeList = new List<ItemForRecipe>();

    [SerializeField] RecepieScrObj currentRecepie;
    [SerializeField] TMP_Text craftingTitle;
    [SerializeField] TMP_Text craftingDiscription;

    [SerializeField] GameObject resourcePrefab;
    [SerializeField] Transform craftingResources;

    [SerializeField] RecepieScrObj[] recepies;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!active)
            return;

        if (Input.GetButtonDown("Cancel"))
        {
            topCamera.enabled = false;
            active = false;
            inventoryObj.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (crafting)
        {
            if(craftSlider.value < 1)
            {
                craftSlider.value += 1 * Time.deltaTime;
            }
            else
            {
                //Craft item

                crafting = false;

                foreach (ItemForRecipe itemForRecipe in itemsForRecipeList)
                {
                    itemForRecipe.itemType.countInInv -= itemForRecipe.countToRemove;

                    if (itemForRecipe.itemType.countInInv <= 0)
                        Inventory.items.Remove(itemForRecipe.itemType);
                }

                Inventory.AddItem(currentRecepie.item);

                craftingTitle.text = "Item added to inventory";
                craftingDiscription.text = "";
                foreach (Transform t in craftingResources)
                {
                    Destroy(t.gameObject);
                }

                currentRecepie = null;
                craftSlider.value = 0;

                UpdateRecepies();
            }
        }
    }
    public void Craft()
    {
        if (currentRecepie == null)
            return;

        if (enoughItems)
        {
            craftSlider.value = 0;
            crafting = true;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            notEnoughItems.SetActive(true);
        }
    }
    public void CraftStop()
    {
        GetComponent<AudioSource>().Stop();
        notEnoughItems.SetActive(false);
        crafting = false;
        craftSlider.value = 0;
    }

    public void OpenChanger()
    {
        topCamera.enabled = true;
        active = true;
        inventoryObj.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        craftingTitle.text = "";
        UpdateRecepies();
    }
    public void UpdateRecepies()
    {
        foreach (Transform t in recepieButtonHolder)
        {
            Destroy(t.gameObject);
        }

        foreach (RecepieScrObj recepie in recepies)
        {
            foreach (RecepieScrObj.ItemNeeded itemNeeded in recepie.itemsNeeded)
            {
                bool spawned = false;
                foreach (Inventory.ItemType item in Inventory.items)
                {
                    if (item.item == itemNeeded.item)
                    {
                        spawned = true;

                        CraftingButton craft = Instantiate(recepieButtonPrefab, recepieButtonHolder).GetComponent<CraftingButton>();
                        craft.recepieName.text = recepie.name;
                        craft.recepie = recepie;

                        break;
                    }
                }
                if (spawned)
                    break;
            }
        }
    }
    public void SelectRecepie(RecepieScrObj recepie)
    {
        currentRecepie = recepie;
        craftingTitle.text = currentRecepie.recipeName;
        craftingDiscription.text = currentRecepie.info;

        enoughItems = true;

        itemsForRecipeList.Clear();
        foreach(Transform t in craftingResources)
        {
            Destroy(t.gameObject);
        }

        foreach (RecepieScrObj.ItemNeeded neededItem in currentRecepie.itemsNeeded)
        {
            bool hasItem = false;
            GameObject resource = Instantiate(resourcePrefab, craftingResources);
            resource.GetComponent<ResourceInfo>().SetUp(neededItem.item.itemName, neededItem.count);
            foreach (Inventory.ItemType item in Inventory.items)
            {
                if (item.item.id == neededItem.item.id)
                {
                    if (item.countInInv < neededItem.count)
                    {
                        enoughItems = false;
                        resource.GetComponent<ResourceInfo>().Bad();
                        hasItem = true;
                        break;
                    }
                    else
                    {
                        ItemForRecipe addItem = new ItemForRecipe();
                        addItem.itemType = item;
                        addItem.countToRemove = neededItem.count;
                        itemsForRecipeList.Add(addItem);

                        resource.GetComponent<ResourceInfo>().Good();
                        hasItem = true;
                        break;
                    }
                }
            }
            if (!hasItem)
            {
                resource.GetComponent<ResourceInfo>().Bad();
                enoughItems = false;
            }
        }
    }
}
