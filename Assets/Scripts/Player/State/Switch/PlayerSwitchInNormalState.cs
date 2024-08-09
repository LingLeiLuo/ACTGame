using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitchInNormalState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;

        playerController.PlayAnimation("SwitchIn_Normal", 0f);
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToNormalAttackTriggered();

        SwitchToEvade();

        SwitchToWalk();

        SwitchToIdle();
    }
}
