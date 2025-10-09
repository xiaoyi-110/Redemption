using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea(3, 5)] public string itemDescription;

    public CarryType carryType = CarryType.None;
    public UseTrigger useTrigger;

    public bool destroyOnUse = false;
    public bool isContinuousUse = false;
}
