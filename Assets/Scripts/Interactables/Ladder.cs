using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VentTarget
{
    public string tag;
    public Transform EntryPoint;
    public Transform ExitPoint;
}

public class Ladder : InteractableObject
{
    [Header("Ladder State")]
    [SerializeField] private bool isExtended = false;

    [Header("Visuals")]
    public Sprite retractedUIIcon;
    public Sprite extendedUIIcon;


    protected override void Start()
    {
        base.Start();
    }

    protected override void HandleUse(PlayerController player)
    {
        if (isExtended) { player.UseVentilation(); }
        ToggleLadderState();
    }

    private void ToggleLadderState()
    {
        isExtended = !isExtended;
        Sprite iconToUse=isExtended?extendedUIIcon:retractedUIIcon;
        EventManager.Instance.TriggerEvent(this, ItemStateChangedEventArgs.Create(this, iconToUse));
        Debug.Log($"[Ladder] Ladder is now {(isExtended ? "Extended" : "Retracted")}");
    }
}
