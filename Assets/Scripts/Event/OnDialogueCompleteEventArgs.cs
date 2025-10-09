using System;

public class OnDialogueCompleteEventArgs : EventArgs
{
    public static OnDialogueCompleteEventArgs Create()
    {
        return new OnDialogueCompleteEventArgs();
    }
}
