// --------------------------------------------------------------------------------------------------------------------
/// <copyright file="InventoryItem.cs">
///   Copyright (c) 2024, Marcos Henrique, All rights reserved.
/// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using ItemSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon => GetComponent<Image>();
    public CanvasGroup canvasGroup {  get; private set; }
    public Item myItem { get; set; }
    public inventorySlot activeSlot { get; set; }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(inventorySlot parent, Item item = null)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        if (item != null)
        {
            myItem = item;
            if (item.itemSprite == null)
                throw new System.Exception($"Item with ID {item.ID} has no sprite.");
            itemIcon.sprite = item.itemSprite;
        }
        else
        {
            myItem = null;
            itemIcon.sprite = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
            Debug.Log(Inventory.carriedItem);
        } 
    }
}
