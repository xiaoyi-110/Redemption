using System.Collections.Generic;
using UnityEngine;

public class InventoryData : MonoSingleton<InventoryData>
{
    public int maxSlots = 50;
    public List<ItemData> items = new List<ItemData>();

    public bool AddItem(ItemData itemData)
    {
        if (items.Count >= maxSlots)
        {
            //Debug.LogWarning("Inventory is full!");
            return false;
        }

        items.Add(itemData);
        EventManager.Instance?.TriggerEvent(this, OnInventoryUpdatedEventArgs.Create());
        return true;
    }

    public void RemoveItem(ItemData itemData)
    {
        if (items.Remove(itemData))
        {
            EventManager.Instance?.TriggerEvent(this, OnInventoryUpdatedEventArgs.Create());
        }
    }
}
