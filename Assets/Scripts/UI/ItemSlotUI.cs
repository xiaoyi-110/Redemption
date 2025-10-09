using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData ItemData { get; private set; }
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image backgroundImage;
    private InventoryPanel parentPanel;
    private void Start()
    {
        parentPanel = GetComponentInParent<InventoryPanel>();
    }
    public void LoadItem(ItemData itemData)
    {
        ItemData = itemData;

        if (ItemData == null)
        {
            UnloadItem();
            return;
        }

        if (ItemData.itemIcon != null)
        {
            itemIcon.sprite = ItemData.itemIcon;
            itemIcon.enabled = true;
        }

        backgroundImage.enabled = true;

    }

    public void UnloadItem()
    {
        ItemData = null;
        itemIcon.enabled = false;
        backgroundImage.enabled = false;
    }

    public void UpdateIcon(Sprite newIcon)
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = newIcon;
            itemIcon.enabled = newIcon != null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log(" Û±ÍΩ¯»Î£∫" + gameObject.name);
        if (parentPanel != null)
        {
            parentPanel.SetHoveredSlot(this);
        }
        if (ItemData != null)
        {
            TooltipManager.Instance.Show(ItemData.itemDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (parentPanel != null)
        {
            parentPanel.SetHoveredSlot(null);
        }

        TooltipManager.Instance.Hide();
    }

}
