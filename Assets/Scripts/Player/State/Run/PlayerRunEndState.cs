using UnityEngine;

public class PlayerRunEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;

        #region 判断左右脚
        switch (playerModel.foot)
        {
            case ModelFoot.Right:
                playerController.PlayAnimation("Run_End_R", 0.1f);
                break;
            case ModelFoot.Left:
                playerController.PlayAnimation("Run_End_L", 0.1f);
                break;
        }
        #endregion
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToNormalAttackPressed();

        SwitchToEvade();

        SwitchToWalk();

        SwitchToIdle();
    }
}