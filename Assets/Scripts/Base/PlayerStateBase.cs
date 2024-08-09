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
    //��ҿ�����
    protected PlayerController playerController;
    //���ģ��
    protected PlayerModel playerModel;
    //������Ϣ
    private AnimatorStateInfo stateInfo;
    //��¼��ǰ״̬�����ʱ��
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
        //״̬����ʱ���ʱ
        statePlayTime += Time.deltaTime;

        #region ����ɫ�л�
        if (playerController.inputSystem.Player.SwitchDown.triggered &&
            playerModel.currentState != PlayerState.BigSkillStart &&
            playerModel.currentState != PlayerState.BigSkill)
        {
            //�л���ɫ
            playerController.SwitchNextModel();
        }
        if (playerController.inputSystem.Player.SwitchUp.triggered &&
        playerModel.currentState != PlayerState.BigSkillStart &&
        playerModel.currentState != PlayerState.BigSkill)
        {
            //�л���ɫ
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
            //ʩ������Ӱ��
            playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));
        }
        //ˢ�¶���״̬
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
    }

    public override void LateUpdate()
    {
        
    }

    public override void UnInit()
    {

    }

    #region ״̬�л�
    /// <summary>
    /// �л�������
    /// </summary>
    protected virtual void SwitchToIdle()
    {
        #region �����Ƿ񲥷Ž���
        if (IsAnimationEnd())
        {
            //�л�������״̬
            playerController.SwitchState(PlayerState.Idle);
            // ��ǰ������������
            playerModel.CurrentNormalAttackIndex = 0;
            return;
        }
        #endregion
    }

    /// <summary>
    /// �л�����ͨ����(Triggered)
    /// </summary>
    protected virtual void SwitchToNormalAttackTriggered()
    {
        #region ��⹥��
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            //�л�����ͨ����״̬
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion
    }
    
    /// <summary>
    /// �л�����ͨ����(Pressed)
    /// </summary>
    protected virtual void SwitchToNormalAttackPressed()
    {
        #region ��⹥��
        if (playerController.inputSystem.Player.Fire.IsPressed())
        {
            //�л�����ͨ����״̬
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion
    }

    /// <summary>
    /// �л�������
    /// </summary>
    protected virtual void SwitchToEvade()
    {
        #region �������
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            if (playerController.inputMoveVector2 == Vector2.zero)
            {
                //�л�������״̬
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            else
            {
                Vector3 inputMoveVector3 = new Vector3(playerController.inputMoveVector2.x, 0, playerController.inputMoveVector2.y);
                //��ȡ�������ת��Y
                float cameraAxisY = Camera.main.transform.rotation.eulerAngles.y;
                //��Ԫ�� �� ����
                Vector3 targetDic = Quaternion.Euler(0, cameraAxisY, 0) * inputMoveVector3;
                Quaternion targetQua = Quaternion.LookRotation(targetDic);


                playerModel.transform.rotation = targetQua;
                //�л�������״̬
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            return;
        }
        #endregion
    }

    /// <summary>
    /// �л����ƶ�
    /// </summary>
    protected virtual void SwitchToWalk()
    {
        #region ����ƶ�
        if (playerController.inputMoveVector2 != Vector2.zero)
        {
            //�л�������״̬
            playerController.SwitchState(PlayerState.Walk);
            return;
        }
        #endregion
    }

    /// <summary>
    /// �л�������
    /// </summary>
    protected virtual void SwitchToBigSkill() 
    {
        #region ������
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
    /// �ж϶����Ƿ����
    /// </summary>
    public bool IsAnimationEnd()
    {
        //ˢ�¶���״̬
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1f && !playerModel.animator.IsInTransition(0);
    }

    /// <summary>
    /// ��ȡ�������Ž���
    /// </summary>
    /// <returns>�������Ž���</returns>
    public float NormalizedTime()
    {
        //ˢ�¶���״̬
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime;
    }
}
