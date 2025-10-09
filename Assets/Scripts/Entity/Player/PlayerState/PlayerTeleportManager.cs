using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportManager : MonoSingleton<PlayerTeleportManager>
{
    private bool isTeleporting = false;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportDelay = 0.5f;
    [SerializeField] private List<VentTarget> ventTargets = new List<VentTarget>();
    private float closestDistance = 3f;

    public async UniTask TeleportToVent(PlayerController player)
    {
        if (isTeleporting)
        {
            //Debug.LogWarning("Already teleporting. Aborting.");
            return;
        }

        Transform target = FindNearestVent(player);
        if (target != null)
        {
            await TeleportProcess(player, target, "正在进入通风管道...");
            EventManager.Instance.TriggerEvent(this, TeleportSuccessEventArgs.CreateEnterVentEvent(player));
            Debug.Log($"[Ladder] Teleport complete -> {target.name} at {target.position}");
        }
        else
        {
            Debug.LogWarning("[Ladder] No valid vent found!");
            return;
        }
    }

    public async UniTask TeleportFromVent(PlayerController player, Transform targetPosition)
    {
        if (targetPosition != null)
        {
            await TeleportProcess(player, targetPosition, "正在离开通风口...");
            EventManager.Instance.TriggerEvent(this, TeleportSuccessEventArgs.CreateExitVentEvent(player));
        }
    }

    private async UniTask TeleportProcess(PlayerController player, Transform targetPosition, string message)
    {
        isTeleporting = true;
        //Debug.Log("[TeleportManager] Teleport started...");

        UIManager.Instance.OpenPanel("MessagePanel", message).Forget();

        await UniTask.WaitForSeconds(teleportDelay);

        player.transform.position = targetPosition.position;
        //Debug.Log($"[TeleportManager] Teleport complete -> {targetPosition.name} at {targetPosition.position}");
        isTeleporting = false;
        //Debug.Log("[TeleportManager] Teleport finished.");
    }

    private Transform FindNearestVent(PlayerController player)
    {
        float minDistance = closestDistance;
        Transform result = null;

        //Debug.Log($"当前通风管道数目：{ventTargets.Count}");
        foreach (var vent in ventTargets)
        {
            if (vent.EntryPoint == null || vent.ExitPoint == null)
            {
                Debug.LogWarning("[Ladder] Invalid vent target skipped.");
                continue;
            }
            float distance = Vector2.Distance(player.transform.position, vent.EntryPoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = vent.ExitPoint;
            }
        }

        return result;
    }
}
