using UnityEngine;

public class Clothing : InteractableObject
{
    protected override void HandleUse(PlayerController player)
    {
        ExtinguishNearbyCombustibles(player);
    }

    private void ExtinguishNearbyCombustibles(PlayerController player)
    {
        IExtinguishable target = player.PlayerInteractor.FindNearestExtinguishable();

        if (target != null)
        {      
            target.Extinguish();
        }
        else
        {
            Debug.Log("周围没有可以扑灭的物品！");
        }
    }
}
