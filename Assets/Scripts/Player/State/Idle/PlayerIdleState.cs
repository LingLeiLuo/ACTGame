using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;
        switch (playerModel.currentState)
        {
            case PlayerState.Idle:
                playerController.PlayAnimation("Idle", 0.25f);
                break;
            case PlayerState.Idle_AFK:
                playerController.PlayAnimation("Idle_AFK", 0.25f);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToNormalAttackTriggered();

        SwitchToEvade();

        SwitchToWalk();

        switch (playerModel.currentState)
        {
            case PlayerState.Idle:
                if (statePlayTime > 3)
                {
                    playerController.SwitchState(PlayerState.Idle_AFK);
                    return;
                }
                break;
            case PlayerState.Idle_AFK:
                if (IsAnimationEnd())
                {
                    //ÇÐ»»µ½´ý»ú×´Ì¬
                    playerController.SwitchState(PlayerState.Idle);
                    return;
                }
                break;
        }
    }
}
