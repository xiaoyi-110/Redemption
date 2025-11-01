using System;
using UnityEngine;
public class OnPuzzleCompleteEventArgs : EventArgs
{

    public bool Success { get; private set; }
    public MetroDoor Door { get; private set; }

    public static OnPuzzleCompleteEventArgs Create(bool success, MetroDoor door)
    {
        return new OnPuzzleCompleteEventArgs
        {
            Success = success,
            Door = door
        };
    }
}
