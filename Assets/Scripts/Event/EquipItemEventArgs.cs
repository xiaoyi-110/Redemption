using System;


public class EquipItemEventArgs : EventArgs
{
    public static EquipItemEventArgs Create()
    {
        return new EquipItemEventArgs();
    }
}
