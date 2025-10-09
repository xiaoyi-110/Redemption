using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableObject : Entity
{
    [Header("Item Data")]
    public ItemData itemData;

    public bool IsEquipped { get; private set; } = false; 
    private SpriteRenderer spriteRenderer;

    private int equippedLayer;
    private int originalLayer;

    public GameObject originalPrefab;

    public string ItemName => itemData.itemName;
    public Sprite ItemIcon => itemData.itemIcon;
    public string ItemDescription => itemData.itemDescription;
    public CarryType CarryType => itemData.carryType;
    public UseTrigger UseTrigger => itemData.useTrigger;
    public bool DestroyOnUse => itemData.destroyOnUse;
    public bool IsContinuousUse => itemData.isContinuousUse;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"{gameObject.name} is missing SpriteRenderer!");
            enabled = false; 
        }

        if (itemData == null)
        {
            Debug.LogError($"{gameObject.name} is missing ItemData!");
        }
        else
        {
            spriteRenderer.sprite = itemData.itemIcon;
        }
        originalLayer = gameObject.layer;
        equippedLayer = LayerMask.NameToLayer("Equipped");
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null) {
            EventManager.Instance.RegisterEvent<OnItemDestroyRequestEventArgs>(OnItemDestroyRequested); 
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnRegisterEvent<OnItemDestroyRequestEventArgs>(OnItemDestroyRequested);
        }
    }

    private void OnItemDestroyRequested(object sender, OnItemDestroyRequestEventArgs args)
    {
        if (args.Interactable == this)
        {
            Debug.Log($"Received destroy request for {itemData.itemName}. Destroying object.");
            PlayerEquipmentManager.Instance.UnequipDestroyedItem(this);
            Destroy(gameObject);
            EventManager.Instance?.TriggerEvent(this, EquipmentUIChangedEventArgs.Create());
        }
    }
    public virtual void OnInteract(PlayerController player)
    {
        if (InventoryData.Instance.AddItem(itemData))
            {
                Debug.Log($"Picked up {itemData.itemName}. Destroying physical object.");
                Destroy(gameObject);
            }
        else
            {
                Debug.LogWarning($"Inventory is full! Cannot pick up {itemData.itemName}.");
            }
        }

    public virtual void OnEquip(Transform parent)
    {
        if (IsEquipped) return;
        IsEquipped = true;
        transform.SetParent(parent);
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;

        if (equippedLayer != -1)
        {
            gameObject.layer = equippedLayer;
        }
        else
        {
            Debug.LogError("Can't find EquippedLayer");
        }
        HandleEquip();
    }

    public virtual void OnUnequip()
    {
        if (!IsEquipped) return;

        IsEquipped = false;
        if (originalLayer != -1)
        {
            gameObject.layer = originalLayer;
        }
        HandleUnequip();
    }

    public virtual void UseItem(PlayerController player)
    {
        if (!IsEquipped)
        {
            Debug.LogWarning($"Item {itemData.itemName} not equipped!");
            return;
        }

        HandleUse(player);

        if (DestroyOnUse)
        {
            Debug.Log($"Consuming {itemData.itemName}. 发出销毁请求。");
            EventManager.Instance.TriggerEvent(this,OnItemDestroyRequestEventArgs.Create(this) );
        }
    }


    protected virtual void HandleEquip() {
        gameObject.SetActive(true);
    }
    protected virtual void HandleUnequip() { }
    protected virtual void HandleUse(PlayerController player) { }


    void OnDrawGizmos()
    {
        if (this.gameObject.activeInHierarchy) 
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawSphere(transform.position, 0.2f); 
        }
    }

}
