using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : InteractableObject
{
    [Header("Flashlight Settings")]
    public bool isFlashlightOn = false; 
    [SerializeField] private Light flashlight;
    public Sprite onIcon;
    public Sprite offIcon;
    protected override void Start()
    {
        base.Start();
        if (flashlight == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                flashlight = player.FollowLight;
            }
            if (flashlight == null)
            {
                Debug.LogError($"{gameObject.name} is missing a Light component!");
                return;
            }
        }
        flashlight.enabled = false;
    }
    public override void OnUnequip()
    {
        base.OnUnequip();
        flashlight.enabled = false;
        isFlashlightOn = false;
        EventManager.Instance.TriggerEvent(this, ItemStateChangedEventArgs.Create(this, offIcon));
    }
    protected override void HandleUse(PlayerController player)
    {        
        isFlashlightOn = !isFlashlightOn;
        flashlight.enabled = isFlashlightOn;
        
        Sprite iconToUse = isFlashlightOn ? onIcon : offIcon;

        EventManager.Instance.TriggerEvent(this, ItemStateChangedEventArgs.Create(this, iconToUse));
    }
}
   
