using ItemSystem;
using UnityEngine;
using UnityEngine.UI;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;
using static LangSystem.Language;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

     public inventorySlot[] inventorySlots;
     public inventorySlot[] equipmentSlots;

    public Transform draggablesTransform => GameObject.Find("DraggablesTransform").GetComponent<Transform>();
    public InventoryItem itemPrefab;

    public Item[] items => Resources.LoadAll<Item>("ItemsObj");

    [SerializeField] Button giveItemBtn;
    [SerializeField] Button lixeiraBtn;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }

    private void Start()
    {
        giveItemBtn.onClick.AddListener(delegate { if (!SpawnInventoryItem()) WarningTextManager.ShowWarning(currentLanguage.inventoryIsFull, 1f, 0.5f); });
        lixeiraBtn.onClick.AddListener(delegate { DestroyItem(); });
    }

    private void Update()
    {
        if (carriedItem == null)
            return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
       if(carriedItem != null)
        {
            if(item.activeSlot.myType != ItemType.None && item.activeSlot.myType != carriedItem.myItem.type)
            {
                return;
            }

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
        switch (type)
        {
            case ItemType.MeleeWeapon:
                if(item == null)
                {
                    Debug.Log("Removeu um item do tipo MeleeWeapon");
                }
                else
                {
                    Debug.Log("Equipou um item do tipo MeleeWeapon");
                }
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
