using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData : MonoSingleton<InventoryData>
{
    public int maxSlots = 50;
    public List<ItemData> items = new List<ItemData>();
    private InventoryPanel panel;
    private bool isFirstTime = true;

    private void Start()
    {
        InventoryPanel[] panels = FindObjectsOfType<InventoryPanel>(true); 
        if (panels.Length > 0)
        {
            panel= panels[0]; 
        }
    }
    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RegisterEvent<EquipItemEventArgs>(OnEquipItemEventHandler);
            EventManager.Instance.RegisterEvent<OnInventoryUpdatedEventArgs>(OnInventoryUpdatedHandler);
        }
    }


    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent<EquipItemEventArgs>(OnEquipItemEventHandler);
        EventManager.Instance.UnRegisterEvent<OnInventoryUpdatedEventArgs>(OnInventoryUpdatedHandler);
    }

    private void OnEquipItemEventHandler(object sender, EquipItemEventArgs args)
    {
        panel?.EquipHoveredItem();
    }
    private void OnInventoryUpdatedHandler(object sender, OnInventoryUpdatedEventArgs args)
    {
        ShowTips();
        panel?.UpdateInventoryUI();
    }
    private void ShowTips()
    {
        if (isFirstTime)
        {
            UIManager.Instance.OpenPanel("TipPanel", "按下Q键将物品收回背包，在背包可选择物品按下F键进行装备。").Forget();
            isFirstTime = false;
        }
    }
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
