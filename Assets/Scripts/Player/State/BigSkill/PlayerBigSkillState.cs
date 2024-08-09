using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class PlayerBigSkillState : PlayerStateBase
{
    CanvasGroup ui;

    public override void Enter()
    {
        base.Enter();
        ui = GameObject.Find("绝区零战斗UI").GetComponent<CanvasGroup>();
        BigSkillStateEnter();
    }

    public override void Update()
    {
        base.Update();

        switch (playerModel.currentState)
        {
            case PlayerState.BigSkillStart:
                if (IsAnimationEnd()) { playerController.SwitchState(PlayerState.BigSkill); return; } break;
            case PlayerState.BigSkill:
                if (IsAnimationEnd()) { playerController.SwitchState(PlayerState.BigSkill01); return; } break;
            case PlayerState.BigSkill01:
                if (IsAnimationEnd()) { playerController.SwitchState(PlayerState.BigSkillEnd); return; } break;
            case PlayerState.BigSkillEnd:
                SwitchToEvade();
                SwitchToNormalAttackTriggered();
                if (NormalizedTime() > 0.7f) SwitchToWalk();
                SwitchToIdle();
                break;

        }
    }

    private void BigSkillStateEnter()
    {
        playerModel.CurrentNormalAttackIndex = 0;
        playerModel.animator.speed = 1f;

        switch (playerModel.currentState)
        {
            case PlayerState.BigSkillStart:
                ui.DOFade(0, 0.25f);
                CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
                CameraManager.INSTANCE.freeLookCanmera.SetActive(false);
                playerModel.BigSkillCamera.SetActive(true);

                playerController.PlayAnimation("BigSkill_Start", 0.0f);
                break;
            case PlayerState.BigSkill:
                if (playerController.nearestEnemyCollider != null)
                {
                    // 索敌状态下，向量计算使玩家转向并位移到怪物身边
                    playerModel.transform.rotation = Quaternion.LookRotation(playerController.nearestEnemyCollider.transform.position - playerModel.transform.position);
                    Vector3 direction = (playerController.nearestEnemyCollider.transform.position - playerModel.transform.position).normalized;
                    Vector3 pos = playerController.nearestEnemyCollider.transform.position - direction * 3;
                    playerModel.characterController.Move((pos - playerModel.transform.position));
                }
                playerController.StartAttack(playerModel.bigSkillAttackConfigs[0]);
                break;
            case PlayerState.BigSkill01:
                playerController.ExPTS = 0;
                playerController.PlayAnimation("BigSkill 01", 0.0f);
                break;
            case PlayerState.BigSkillEnd:
                ui.DOFade(1, 0.75f);
                playerModel.BigSkillCamera.SetActive(false);
                CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
                CameraManager.INSTANCE.freeLookCanmera.SetActive(true);
                CameraManager.INSTANCE.ResetFreeLookCamera();

                playerController.PlayAnimation("BigSkill_End", 0.0f);
                break;

        }
    }
}
