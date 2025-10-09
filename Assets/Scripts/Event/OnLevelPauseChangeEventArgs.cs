using System;

public class OnLevelPauseChangeEventArgs : EventArgs
{
    public bool IsPause { get; }

    private OnLevelPauseChangeEventArgs(bool isPause)
    {
        IsPause = isPause;
    }

    public static OnLevelPauseChangeEventArgs Create(bool isPause)
    {
        return new OnLevelPauseChangeEventArgs(isPause);
    }
}
