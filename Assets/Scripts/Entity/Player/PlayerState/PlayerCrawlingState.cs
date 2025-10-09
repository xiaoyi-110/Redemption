using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrawlingState : PlayerStateBase
{
    public static PlayerCrawlingState Create() => new PlayerCrawlingState();

    public override void OnEnter(FSM<PlayerController> fsm)
    {
        base.OnEnter(fsm);
        player.speedMultiplier = 0.6f;
        player.Animator.SetBool("IsCrawling", true);
        player.Animator.SetFloat("CrawlSpeed", 0);
    }

    public override void OnUpdate(FSM<PlayerController> fsm)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        player.movement = new Vector2(h, v).normalized;

        player.PlayerAnimation.UpdateMovement(player.movement);
    }

    public override void OnExit(FSM<PlayerController> fsm)
    {
        base.OnExit(fsm);
        player.Animator.SetBool("IsCrawling", false);
    }
}
