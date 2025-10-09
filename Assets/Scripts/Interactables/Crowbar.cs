using System.Collections.Generic;
using UnityEngine;

public class Crowbar : InteractableObject
{

    protected override void Start()
    {
        base.Start();
        //destroyOnUse = false;
        //useTrigger = UseTrigger.KeyF;
    }

    protected override void HandleUse(PlayerController player)
    {
        if (player == null) return;
        MetroDoor door = player.PlayerInteractor.GetNearestMetroDoor();

        if (door != null)
        {
            door.TryOpenWithCrowbar();
        }
        else
        {
            //Debug.Log("找不到地铁门");
        }
    }
}