using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class InventoryPanel : UIBase
{
    public override UIType UIType => UIType.Window;
    public override string PanelName => "InventoryPanel";

    [Header("UI References")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Button closeButton;
    private ItemSlotUI hoveredSlot;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(PanelName));
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
        EquipHoveredItem();
    }

    private void OnInventoryUpdatedHandler(object sender,OnInventoryUpdatedEventArgs args)
    {
        UpdateInventoryUI();
    }
    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        TooltipManager.Instance?.gameObject.SetActive(true);
        UpdateInventoryUI();
        Time.timeScale = 0; 
    }

    public override void OnClose()
    {
        base.OnClose();
        TooltipManager.Instance?.Hide();
        TooltipManager.Instance?.gameObject.SetActive(false);
        Time.timeScale = 1; 
    }
    private void UpdateInventoryUI()
    {
        var items = InventoryData.Instance.items;
        var existingSlots = itemContainer.GetComponentsInChildren<ItemSlotUI>(true).ToList();

        int requiredSlots = items.Count;
        int currentSlots = existingSlots.Count;

        for (int i = 0; i < requiredSlots - currentSlots; i++)
        {
            GameObject newSlotObj = ObjectPoolManager.Instance.GetObject(inventorySlotPrefab);
            newSlotObj.transform.SetParent(itemContainer);
        }

        existingSlots = itemContainer.GetComponentsInChildren<ItemSlotUI>(true).ToList();

        for (int i = 0; i < existingSlots.Count; i++)
        {
            if (i < items.Count)
            {
                existingSlots[i].LoadItem(items[i]);
                existingSlots[i].gameObject.SetActive(true);
            }
            else
            {
                ObjectPoolManager.Instance.ReturnObject(inventorySlotPrefab, existingSlots[i].gameObject);
            }
        }
        hoveredSlot = null;
        DelayedLayoutRefreshAsync().Forget();
    }

    private async UniTaskVoid DelayedLayoutRefreshAsync()
    {
        await UniTask.Yield();
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemContainer.GetComponent<RectTransform>());
    }


    public void SetHoveredSlot(ItemSlotUI slot)
    {
        hoveredSlot = slot;
    }

    public void EquipHoveredItem()
    {
        if (hoveredSlot != null && hoveredSlot.ItemData != null)
        {
            PlayerEquipmentManager.Instance.EquipItem(hoveredSlot.ItemData);
        }
    }
}