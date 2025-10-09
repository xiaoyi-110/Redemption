using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : UIBase
{
    public override UIType UIType => UIType.HUD;
    public override string PanelName => "EquipmentUI";
    [Header("UI References")]
    [SerializeField] private Transform equippedItemsContainer;   
    [SerializeField] private GameObject equipmentSlotPrefab;

    private ItemSlotUI[] slots;

    private void Awake()
    {
        slots = new ItemSlotUI[2];
        for (int i = 0; i < 2; i++)
        {
            GameObject slotObj = Instantiate(equipmentSlotPrefab, equippedItemsContainer);
            slots[i] = slotObj.GetComponent<ItemSlotUI>();
            slots[i].UnloadItem(); 
        }
    }

    private void Start()
    {
        UpdateEquippedUI();
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.RegisterEvent<ItemStateChangedEventArgs>(OnItemChangeEventHandler);
            EventManager.Instance.RegisterEvent<EquipmentUIChangedEventArgs>(OnUpdateEquippedUIHandler);
        }
        else
        {
            Debug.Log("EventManager instance is null in EquipmentUI OnEnable");
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnRegisterEvent<ItemStateChangedEventArgs>(OnItemChangeEventHandler);
            EventManager.Instance.UnRegisterEvent<EquipmentUIChangedEventArgs>(OnUpdateEquippedUIHandler);
        }
    }

    private void OnUpdateEquippedUIHandler(object sender, EquipmentUIChangedEventArgs args)
    {
        UpdateEquippedUI();
    }

    private void OnItemChangeEventHandler(object sender, ItemStateChangedEventArgs args)
    {

        if (PlayerEquipmentManager.Instance.EquippedMask == args.ItemInstance)
        {
            slots[0].UpdateIcon(args.NewIcon);
        }
        else if (PlayerEquipmentManager.Instance.EquippedItem == args.ItemInstance)
        {
            slots[1].UpdateIcon(args.NewIcon);
        }
    }

    private void UpdateEquippedUI()
    {
        if (PlayerEquipmentManager.Instance.EquippedMaskData != null)
        {
            slots[0].LoadItem(PlayerEquipmentManager.Instance.EquippedMaskData);
            slots[0].gameObject.SetActive(true);
        }
        else
        {
            slots[0].UnloadItem();
            slots[0].gameObject.SetActive(false);
        }

        if (PlayerEquipmentManager.Instance.EquippedItemData != null)
        {
            slots[1].LoadItem(PlayerEquipmentManager.Instance.EquippedItemData);
            slots[1].gameObject.SetActive(true);
        }
        else
        {
            slots[1].UnloadItem();
            slots[1].gameObject.SetActive(false);
        }
    }
}
