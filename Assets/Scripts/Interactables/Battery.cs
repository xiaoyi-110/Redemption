using UnityEngine;

public class Battery : InteractableObject
{
    protected override void HandleUse(PlayerController player)
    {
        if (player == null) return;
        MetroDoor door = player.PlayerInteractor.GetNearestMetroDoor();

        if (door == null) return;
        door.HandlePower();
    }
}