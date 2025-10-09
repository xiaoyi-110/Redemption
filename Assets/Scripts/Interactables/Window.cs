using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static UIManager;

public class Window : InteractableObject
{
    [Header("窗口设置")]
    public Collider2D windowCollider;
    //public GameObject escapePoint;

    private bool isBroken = false;

    public override void OnInteract(PlayerController player) {

        if (!isBroken) return;

        player.RescueNPCFromWindow(this);
    }
    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        //Debug.Log("窗户被砸开，可以跳出！");
    }

    public bool IsBroken()
    {
        return isBroken;
    }
}

