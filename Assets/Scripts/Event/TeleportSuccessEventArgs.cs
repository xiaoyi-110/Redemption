using System;
using UnityEngine;
public enum TeleportType
{
    EnterVent,
    ExitVent
}
public class TeleportSuccessEventArgs : EventArgs
{
    public PlayerController Player { get; private set; }
    public TeleportType Type { get; private set; }
    private TeleportSuccessEventArgs(PlayerController player, TeleportType type)
    {
        Player = player;
        Type = type;
    }
    public static TeleportSuccessEventArgs CreateEnterVentEvent(PlayerController player)
    {
        return new TeleportSuccessEventArgs(player, TeleportType.EnterVent);
    }
    public static TeleportSuccessEventArgs CreateExitVentEvent(PlayerController player)
    {
        return new TeleportSuccessEventArgs(player, TeleportType.ExitVent);
    }
}