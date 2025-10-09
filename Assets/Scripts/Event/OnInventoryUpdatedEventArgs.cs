using System;

public class OnInventoryUpdatedEventArgs : EventArgs
{
    public static OnInventoryUpdatedEventArgs Create()
    {
        return new OnInventoryUpdatedEventArgs();
    }
}
