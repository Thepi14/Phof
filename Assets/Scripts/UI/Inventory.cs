using ItemSystem;
using UnityEngine;
using UnityEngine.UI;
using ObjectUtils;
using static ObjectUtils.GameObjectGeneral;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

     public inventorySlot[] inventorySlots;
     public inventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform => GameObject.Find("DraggablesTransform").GetComponent<Transform>();
    [SerializeField] InventoryItem itemPrefab;

    public Item[] items;

    [SerializeField] Button giveItemBtn;
    [SerializeField] Button lixeiraBtn;

    private void Awake()
    {
        Singleton = this;
        
    }

    private void Start()
    {
        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });
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

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(_item == null)
        {
            _item = PickRandomItem();
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
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
}
