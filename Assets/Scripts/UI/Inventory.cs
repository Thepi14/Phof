// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="Inventory.cs">
///   Copyright (c) 2024, Marcos Henrique, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using ItemSystem;
using UnityEngine;
using UnityEngine.UI;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;
using static LangSystem.Language;
using static GamePlayer;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

     public inventorySlot[] inventorySlots;
     public inventorySlot[] equipmentSlots;

    public Transform draggablesTransform => GameObject.Find("DraggablesTransform").GetComponent<Transform>();
    public InventoryItem itemPrefab;

    public Item[] items => Resources.LoadAll<Item>("ItemsObj");

    [SerializeField] Button lixeiraBtn;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    private void Start()
    {
        lixeiraBtn.onClick.AddListener(delegate { DestroyItem(); });
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            if(!SpawnInventoryItem())
                WarningTextManager.ShowWarning(currentLanguage.inventoryIsFull, 1f, 0.5f);
        }
        if (carriedItem == null)
            return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
       if(carriedItem != null)
        {
            /*if(item.activeSlot.myType != ItemType.None && item.activeSlot.myType != carriedItem.myItem.type)
            {
                return;
            }*/

            item.activeSlot.SetItem(carriedItem);
        }
       if(item.activeSlot.myType != ItemType.None)
        {
            EquipEquipment(item.activeSlot.myType, null);
        }

       carriedItem = item;
       carriedItem.canvasGroup.blocksRaycasts = false;
       item.transform.SetParent(draggablesTransform);
    } 

    public void EquipEquipment(ItemType type, InventoryItem item = null)
    {
        player.StartCoroutine(player.AttackTimer());
        switch (type)
        {
            case ItemType.MeleeWeapon:

                break;
        }
    }

    public bool SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(item == null)
        {
            _item = PickRandomItem();
        }
        bool placedOne = false;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(inventorySlots[i], _item);
                placedOne = true;
                break;
            }
        }

        return placedOne;
    }
    public bool SpawnInventoryItem(string ID)
    {
        Item _item = PickItemByID(ID);
        bool placedOne = false;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(inventorySlots[i], _item);
                placedOne = true;
                break;
            }
        }

        return placedOne;
    }

    public void DestroyItem()
    {
        if (carriedItem == null)
            return;

        DestroyImmediate(carriedItem.gameObject);
        carriedItem = null;
    }

    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
    Item PickItemByID(string ID)
    {
        foreach (Item item in items)
        {
            if (item.ID == ID) return item;
        }
        throw new System.Exception($"There's no item with ID {ID}");
    }
}
