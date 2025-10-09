using System;

/// <summary>
/// 当对话选项被选择时传递的事件参数。
/// </summary>
public class OnOptionSelectedEventArgs : EventArgs
{
    public int OptionIndex { get; private set; }

    public static OnOptionSelectedEventArgs Create(int optionIndex)
    {
        return new OnOptionSelectedEventArgs { OptionIndex = optionIndex };
    }
}