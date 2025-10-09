using System;
using UnityEngine;

public class ItemStateChangedEventArgs : EventArgs
{
    public InteractableObject ItemInstance { get; private set; }
    public Sprite NewIcon { get; private set; }

    public ItemStateChangedEventArgs(InteractableObject item, Sprite newIcon) {
        ItemInstance = item;
        NewIcon = newIcon;
    }
    public static ItemStateChangedEventArgs Create(InteractableObject item, Sprite newIcon)
    {
        return new ItemStateChangedEventArgs(item,newIcon);
    }
}