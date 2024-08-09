using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, Idle_AFK,
    Walk, Run, RunEnd, TurnBack,
    Evade_Front, Evade_Back, Evade_Front_End, Evade_Back_End,
    NormalAttack, NormalAttackEnd, PerfectNormalAttack, PerfectNormalAttackEnd,
    SmallSkill_NoEnergy, SmallSkill_Energy, SmallSkill_NoEnergyPerfect, SmallSkill_EnergyPerfect, SmallSkill_NoEnergyEnd, SmallSkill_EnergyEnd, SmallSkill_EnergyPerfectEnd,
    BigSkillStart, BigSkill, BigSkill01, BigSkillEnd,
    SwitchInNormal
}

public class PlayerStateBase : StateBase
{
    //玩家控制器
    protected PlayerController playerController;
    //玩家模型
    protected PlayerModel playerModel;
    //动画信息
    private AnimatorStateInfo stateInfo;
    //记录当前状态进入的时间
    protected float statePlayTime = 0;

    protected float statePlayTimeCullAnimatorSpeed = 0;

    public override void Init(IStateMachineOwner owner)
    {
        playerController = (PlayerController)owner;
        playerModel = playerController.playerModel;
    }

    public override void Enter()
    {
        statePlayTimeCullAnimatorSpeed = 0;
        statePlayTime = 0;
    }

    public override void Exit()
    {
        statePlayTimeCullAnimatorSpeed = 0;
        statePlayTime = 0;
    }

    public override void Update()
    {
        if(playerModel.animator.speed != 0)
        {
            statePlayTimeCullAnimatorSpeed += Time.deltaTime;
        }
        //状态进入时间计时
        statePlayTime += Time.deltaTime;

        #region 检测角色切换
        if (playerController.inputSystem.Player.SwitchDown.triggered &&
            playerModel.currentState != PlayerState.BigSkillStart &&
            playerModel.currentState != PlayerState.BigSkill)
        {
            //切换角色
            playerController.SwitchNextModel();
        }
        if (playerController.inputSystem.Player.SwitchUp.triggered &&
        playerModel.currentState != PlayerState.BigSkillStart &&
        playerModel.currentState != PlayerState.BigSkill)
        {
            //切换角色
            playerController.SwitchLastModel();
        }
        #endregion

    }

    public override void FixedUpdate()
    {
        if(playerModel.currentState != PlayerState.BigSkill 
            && playerModel.currentState != PlayerState.SmallSkill_Energy
            && playerModel.currentState != PlayerState.SmallSkill_EnergyPerfect)
        {
            //施加重力影响
            playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));
        }
        //刷新动画状态
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
    }

    public override void LateUpdate()
    {
        
    }

    public override void UnInit()
    {

    }

    #region 状态切换
    /// <summary>
    /// 切换至待机
    /// </summary>
    protected virtual void SwitchToIdle()
    {
        #region 动画是否播放结束
        if (IsAnimationEnd())
        {
            //切换到待机状态
            playerController.SwitchState(PlayerState.Idle);
            // 当前动机段数归零
            playerModel.CurrentNormalAttackIndex = 0;
            return;
        }
        #endregion
    }

    /// <summary>
    /// 切换至普通攻击(Triggered)
    /// </summary>
    protected virtual void SwitchToNormalAttackTriggered()
    {
        #region 检测攻击
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            //切换到普通攻击状态
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion
    }
    
    /// <summary>
    /// 切换至普通攻击(Pressed)
    /// </summary>
    protected virtual void SwitchToNormalAttackPressed()
    {
        #region 检测攻击
        if (playerController.inputSystem.Player.Fire.IsPressed())
        {
            //切换到普通攻击状态
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion
    }

    /// <summary>
    /// 切换至闪避
    /// </summary>
    protected virtual void SwitchToEvade()
    {
        #region 检测闪避
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            if (playerController.inputMoveVector2 == Vector2.zero)
            {
                //切换到闪避状态
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            else
            {
                Vector3 inputMoveVector3 = new Vector3(playerController.inputMoveVector2.x, 0, playerController.inputMoveVector2.y);
                //获取相机的旋转轴Y
                float cameraAxisY = Camera.main.transform.rotation.eulerAngles.y;
                //四元数 × 向量
                Vector3 targetDic = Quaternion.Euler(0, cameraAxisY, 0) * inputMoveVector3;
                Quaternion targetQua = Quaternion.LookRotation(targetDic);


                playerModel.transform.rotation = targetQua;
                //切换到闪避状态
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            return;
        }
        #endregion
    }

    /// <summary>
    /// 切换至移动
    /// </summary>
    protected virtual void SwitchToWalk()
    {
        #region 检测移动
        if (playerController.inputMoveVector2 != Vector2.zero)
        {
            //切换到奔跑状态
            playerController.SwitchState(PlayerState.Walk);
            return;
        }
        #endregion
    }

    /// <summary>
    /// 切换至大招
    /// </summary>
    protected virtual void SwitchToBigSkill() 
    {
        #region 检测大招
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.ExPTS == 3000)
        {
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion
    }

    protected virtual void SwitchToSmallSkill()
    {

        if (playerController.inputSystem.Player.Branch.triggered)
        {
            if (playerModel.playerData.CurrentBranchEnergy < playerModel.playerData.BranchNeedEnergy)
            {
                playerController.SwitchState(PlayerState.SmallSkill_NoEnergy);
                return;
            }

            if (playerModel.playerData.CurrentBranchEnergy >= playerModel.playerData.BranchNeedEnergy)
            {
                playerController.SwitchState(PlayerState.SmallSkill_Energy);
                return;
            }
        }
    }
    #endregion



    /// <summary>
    /// 判断动画是否结束
    /// </summary>
    public bool IsAnimationEnd()
    {
        //刷新动画状态
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1f && !playerModel.animator.IsInTransition(0);
    }

    /// <summary>
    /// 获取动画播放进度
    /// </summary>
    /// <returns>动画播放进度</returns>
    public float NormalizedTime()
    {
        //刷新动画状态
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime;
    }
}
