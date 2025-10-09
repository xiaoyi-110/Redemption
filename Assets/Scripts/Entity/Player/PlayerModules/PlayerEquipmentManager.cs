using UnityEngine;
using static InteractableObject;
using static UnityEditor.Progress;
public class PlayerEquipmentManager : MonoSingleton<PlayerEquipmentManager>
{
    [SerializeField] private Transform maskEquipPoint;
    [SerializeField] private Transform itemEquipPoint;

    public InteractableObject EquippedItem { get; private set; }
    public ItemData EquippedItemData => EquippedItem?.itemData;

    public InteractableObject EquippedMask { get; private set; }
    public ItemData EquippedMaskData => EquippedMask?.itemData;


    public CarryType currentCarryType = CarryType.None;

    protected override void Awake()
    {
        base.Awake();
    }

    public void EquipItem(ItemData itemData)
    {
        if (itemData == null || !InventoryData.Instance.items.Contains(itemData)) return;

        GameObject itemPrefab = Resources.Load<GameObject>($"Prefabs/Items/{itemData.itemName}");
        if (itemPrefab == null)
        {
            Debug.LogError($"Item prefab for {itemData.itemName} not found!");
            return;
        }

        InteractableObject item = Instantiate(itemPrefab).GetComponent<InteractableObject>();
        if (item == null)
        {
            Debug.LogError($"Instantiated object {itemPrefab.name} is missing InteractableObject component!");
            Destroy(item.gameObject);
            return;
        }

        InventoryData.Instance.RemoveItem(itemData);

        if (item.CarryType == CarryType.Mask)
        {
            if (EquippedMask != null && EquippedMask != item)
            {
                UnequipItem(EquippedMask, true);
            }
            EquippedMask = item;
            //Debug.Log($"current equipped mask is{item.name}");
        }
        else if (item.CarryType == CarryType.Item)
        {
            if (EquippedItem != null && EquippedItem != item)
            {
                UnequipItem(EquippedItem, true);
            }
            EquippedItem = item;
            //Debug.Log($"current equipped item is{item.name}");
        }

        Transform targetPoint = GetAttachPoint(item);
        if (targetPoint == null)
        {
            Debug.LogError("Equip point is null.");
            return;
        }
        item.OnEquip(targetPoint);

        EventManager.Instance?.TriggerEvent(this, EquipmentUIChangedEventArgs.Create());
    }

    public void UnequipItem(InteractableObject item, bool returnToInventory = true)
    {
        if (item == null || !item.IsEquipped) return;

        switch (item.CarryType)
        {
            case CarryType.Mask:
                EquippedMask = null;
                break;
            case CarryType.Item:
                EquippedItem = null;
                break;
        }

        item.OnUnequip();

        if (returnToInventory)
        {
            ReturnToInventory(item);
        }
        else
        {
            DropItem(item);
        }

        EventManager.Instance?.TriggerEvent(this, EquipmentUIChangedEventArgs.Create());
    }

    private Transform GetAttachPoint(InteractableObject item)
    {
        return item.CarryType switch
        {
            CarryType.Mask => maskEquipPoint,
            CarryType.Item => itemEquipPoint,
            _ => transform
        };
    }

    private void ReturnToInventory(InteractableObject item)
    {
        InventoryData.Instance.AddItem(item.itemData);
        Destroy(item.gameObject);
    }

    private void DropItem(InteractableObject item)
    {
        // Re-enable the object in the world and place it at the player's feet
        item.gameObject.SetActive(true);
        item.transform.position = transform.position; // Get the player's position
        //Debug.Log($"Dropped {item.name} in the world.");
    }

    public void UnequipDestroyedItem(InteractableObject interactable)
    {
        if (EquippedItem != null && EquippedItem==interactable)
        {
            UnequipItem(EquippedItem, false);
        }
        if (EquippedMask != null && EquippedMask==interactable)
        {
            UnequipItem(EquippedMask, false);
        }

    }
}
