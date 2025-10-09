using UnityEngine;

public class VentExit : InteractableObject
{
    [Header("出口对应的外部位置")]
    [SerializeField] private Transform targetExitPoint;

    [Header("是否需要手电筒才能离开")]
    [SerializeField] public bool RequiresFlashlight = false;

    public Transform TargetExitPoint => targetExitPoint;
    public override void OnInteract(PlayerController player)
    {
        player.UseVentExit(this, TargetExitPoint);
    }
}