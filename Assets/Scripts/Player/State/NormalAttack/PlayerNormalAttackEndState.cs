using UnityEngine;

public class PlayerNormalAttackEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.animator.speed = 1f;

        switch (playerModel.currentState)
        {
            case PlayerState.NormalAttackEnd:
                playerController.PlayAnimation($"Attack_Normal_{playerModel.CurrentNormalAttackIndex + 1}_End", 0.1f);
                break;
            case PlayerState.PerfectNormalAttackEnd:
                playerController.PlayAnimation($"Normal_Perfect_End", 0.1f);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        #region 检测攻击

        if (playerController.inputSystem.Player.Fire.triggered)
        {
            if(playerModel.currentState != PlayerState.PerfectNormalAttackEnd)
            {
                //攻击段数累加
                playerModel.CurrentNormalAttackIndex++;
            }
            //切换到普通攻击状态
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion

        SwitchToWalk();

        SwitchToEvade();

        SwitchToIdle();
    }
}
