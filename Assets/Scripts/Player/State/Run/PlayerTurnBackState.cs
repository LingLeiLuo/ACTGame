using UnityEngine;

public class PlayerTurnBackState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;

        playerController.PlayAnimation("TurnBack", 0.1f);
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToRun();
    }

    private void SwitchToRun()
    {
        if (IsAnimationEnd() && playerController.inputMoveVector2 != Vector2.zero) { playerController.SwitchState(PlayerState.Run); return; }
    }
}
