using UnityEngine;
using UnityEngine.EventSystems;
using ItemSystem;

public class inventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeReference]
    public InventoryItem myItem;
    public ItemType myType;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.carriedItem == null)
                return;

            if (myType != ItemType.None && Inventory.carriedItem.myItem.type != myType)
                return;

            SetItem(Inventory.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {
        Inventory.carriedItem = null;
        item.activeSlot.myItem = item.activeSlot.gameObject.transform.childCount > 0 ? item.activeSlot.gameObject.transform.GetComponentInChildren<InventoryItem>() : null;

        myItem = item;
        myItem.myItem = item.myItem;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        if(myType != ItemType.None)
        {
            Inventory.Singleton.EquipEquipment(myType, myItem);
        }
    }
}
