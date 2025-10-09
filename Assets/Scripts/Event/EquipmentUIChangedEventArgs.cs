using System;

public class EquipmentUIChangedEventArgs : EventArgs
{
    public static EquipmentUIChangedEventArgs Create()
    {
        return new EquipmentUIChangedEventArgs();
    }
}
