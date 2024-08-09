using Unity.VisualScripting;
using UnityEngine;

public class PlayerEvadeEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;
        switch (playerModel.currentState)
        {
            case PlayerState.Evade_Front_End:
                playerController.PlayAnimation("Evade_Front_End", 0.1f);
                break;
            case PlayerState.Evade_Back_End:
                playerController.PlayAnimation("Evade_Back_End", 0.1f);
                break;
        }
    }

    public override void Update()
    {
        base.Update();


        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToEvade();

        SwitchToNormalAttackTriggered();

        SwitchToWalk();

        SwitchToIdle();
    }

    #region 重写状态切换
    protected override void SwitchToWalk()
    {
        #region 检测移动
        if (playerController.inputMoveVector2 != Vector2.zero)
        {
            switch (playerModel.currentState)
            {
                case PlayerState.Evade_Front_End:
                    playerController.SwitchState(PlayerState.Run);
                    break;
                case PlayerState.Evade_Back_End:
                    playerController.SwitchState(PlayerState.Walk);
                    break;
            }
            return;
        }
        #endregion
    }
    #endregion
}
