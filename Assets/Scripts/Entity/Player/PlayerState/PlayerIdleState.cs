using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public static PlayerIdleState Create() => new PlayerIdleState();
    public override void OnEnter(FSM<PlayerController> fsm)
    {
        base.OnEnter(fsm);
        player.Animator.SetBool("IsIdle", true);
        player.Animator.SetFloat("Speed", 0);
        //player.movement = Vector2.zero;
        //AudioManager.Instance.Stop("Step");
    }

    public override void OnUpdate(FSM<PlayerController> fsm)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        player.movement = new Vector2(h, v).normalized;

        if (player.movement.magnitude > 0.1f)
        {
            fsm.ChangeState<PlayerWalkingState>();
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
        player.Animator.SetBool("IsIdle", false);
    }
}
