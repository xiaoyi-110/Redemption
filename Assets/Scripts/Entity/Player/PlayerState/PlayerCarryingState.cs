using UnityEngine;

public class PlayerCarryingState : PlayerStateBase
{
    public static PlayerCarryingState Create() => new PlayerCarryingState();

    public override void OnEnter(FSM<PlayerController> fsm)
    {
        base.OnEnter(fsm);
        if (PlayerEquipmentManager.Instance.currentCarryType == CarryType.NPC)
            player.speedMultiplier = 0.6f;
        else
            player.speedMultiplier = 1f;
        player.Animator.SetBool("IsCarrying", true);       
    }

    public override void OnUpdate(FSM<PlayerController> fsm)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        player.movement = new Vector2(h, v).normalized;

        player.PlayerAnimation.UpdateMovement(player.movement);


        if (Input.GetKeyDown(KeyCode.C))
        {
            player.TryDropNPC();
            fsm.ChangeState<PlayerWalkingState>();
            return;
        }

    }

    public override void OnExit(FSM<PlayerController> fsm)
    {
        base.OnExit(fsm);
        player.Animator.SetBool("IsCarrying", false);
        //player.Animator.SetFloat("CarrySpeed", 0);
    }
}
