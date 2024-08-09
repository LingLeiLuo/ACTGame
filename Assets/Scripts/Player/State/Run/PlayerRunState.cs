using UnityEngine;

public class PlayerRunState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;

        //判断移动状态
        switch (playerModel.currentState)
        {
            case PlayerState.Walk:
                #region 迈出左右脚的判断
                switch (playerModel.foot)
                {
                    case ModelFoot.Left:
                        playerController.PlayAnimation("Walk", 0.125f, 0.5f);
                        playerModel.foot = ModelFoot.Right;
                        break;
                    case ModelFoot.Right:
                        playerController.PlayAnimation("Walk", 0.125f, 0.0f);
                        playerModel.foot = ModelFoot.Left;
                        break;
                }
                #endregion
                break;
            case PlayerState.Run:
                #region 迈出左右脚的判断
                switch (playerModel.foot)
                {
                    case ModelFoot.Left:
                        playerController.PlayAnimation("Run", 0.125f, 0.5f);
                        playerModel.foot = ModelFoot.Right;
                        break;
                    case ModelFoot.Right:
                        playerController.PlayAnimation("Run", 0.125f, 0.0f);
                        playerModel.foot = ModelFoot.Left;
                        break;
                }
                #endregion
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

        #region 检测待机
        if (playerController.inputMoveVector2 == Vector2.zero)
        {
            playerController.SwitchState(PlayerState.RunEnd);
            return;
        }
        #endregion
        else
        {
            #region 处理移动方向
            Vector3 inputMoveVector3 = new Vector3(playerController.inputMoveVector2.x, 0, playerController.inputMoveVector2.y);
            //获取相机的旋转轴Y
            float cameraAxisY = Camera.main.transform.rotation.eulerAngles.y;
            //四元数 × 向量
            Vector3 targetDic = Quaternion.Euler(0, cameraAxisY, 0) * inputMoveVector3;
            Quaternion targetQua = Quaternion.LookRotation(targetDic);
            //计算旋转角度
            float angles = Mathf.Abs(targetQua.eulerAngles.y - playerModel.transform.eulerAngles.y);
            if (playerController.CanTurnBack && angles > 145f && angles < 215f && playerModel.currentState == PlayerState.Run)
            {
                playerController.SwitchState(PlayerState.TurnBack);
            }
            else
            {
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetQua, Time.deltaTime * playerController.rotationSpeed * 2);
            }
            #endregion
        }

        #region 检测慢跑升级
        if (playerController.WalkLevelRun && playerModel.currentState == PlayerState.Walk && statePlayTime > 3f)
        {
            //切换到疾跑状态
            playerController.SwitchState(PlayerState.Run);
            return;
        }
        #endregion
    }
}
