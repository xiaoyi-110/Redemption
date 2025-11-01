using UnityEngine;

public class PlayerWalkingState : PlayerStateBase
{
    public static PlayerWalkingState Create() => new PlayerWalkingState();
    public override void OnEnter(FSM<PlayerController> fsm)
    {
        base.OnEnter(fsm);
        player.Animator.SetBool("IsWalking", true);
        player.speedMultiplier = 1f;
        AudioManager.Instance.Play("walk");
    }

    public override void OnUpdate(FSM<PlayerController> fsm)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        player.movement = new Vector2(h, v).normalized;

        player.PlayerAnimation.UpdateMovement(player.movement);

        if (player.movement.magnitude < 0.1f)
        {
            fsm.ChangeState<PlayerIdleState>();
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (player.TryCarryNPC())
            {
                fsm.ChangeState<PlayerCarryingState>();
            }
        }
    }

    public override void OnExit(FSM<PlayerController> fsm)
    {
        base.OnExit(fsm);
        player.Animator.SetBool("IsWalking", false);
        player.Animator.SetFloat("Speed", 0);
        //player.movement = Vector2.zero;
        AudioManager.Instance.Stop("walk");
    }
}
