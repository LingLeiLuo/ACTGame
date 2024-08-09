
using UnityEngine;

public class PlayerNormalAttackState : PlayerStateBase
{
    private bool enterNextAttack;
    private int clickCount = 0;
    protected bool enterSkill;

    public override void Enter()
    {
        base.Enter();
        clickCount = 0;
        playerModel.animator.speed = 1f;

        if(playerController.inputMoveVector2 != Vector2.zero && playerController.nearestEnemyCollider == null)
        {
            Vector3 inputMoveVector3 = new Vector3(playerController.inputMoveVector2.x, 0, playerController.inputMoveVector2.y);
            //获取相机的旋转轴Y
            float cameraAxisY = Camera.main.transform.rotation.eulerAngles.y;
            //四元数 × 向量
            Vector3 targetDic = Quaternion.Euler(0, cameraAxisY, 0) * inputMoveVector3;
            Quaternion targetQua = Quaternion.LookRotation(targetDic);

            playerModel.transform.rotation = targetQua;
        }

        enterSkill = false;
        enterNextAttack = false;

        switch (playerModel.currentState)
        {
            case PlayerState.PerfectNormalAttack:
                playerController.StartAttack(playerModel.normalAttackConfigs[playerModel.normalAttackConfigs.Length - 1]);
                break;
            case PlayerState.NormalAttack:
                playerController.StartAttack(playerModel.normalAttackConfigs[playerModel.CurrentNormalAttackIndex]);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        if (playerController.nearestEnemyCollider != null && NormalizedTime() < 0.2f)
        {
            playerModel.transform.LookAt(playerController.nearestEnemyCollider.transform.position);
        }
        SwitchToBigSkill();
        if (playerModel.currentState == PlayerState.NormalAttack)
        {
            if (NormalizedTime() > 0.5f)
            {
                if (statePlayTimeCullAnimatorSpeed >= playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeStart && statePlayTimeCullAnimatorSpeed <= playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeEnd
                    && playerController.inputSystem.Player.Branch.triggered)
                {
                    SwitchToPerfectSkill();
                }

                if (statePlayTimeCullAnimatorSpeed >= playerModel.normalAttackConfigs[playerModel.CurrentNormalAttackIndex].SwitchAttackTime 
                    && statePlayTimeCullAnimatorSpeed < playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeStart 
                    && statePlayTimeCullAnimatorSpeed > playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeEnd
                    && playerController.inputSystem.Player.Branch.triggered)
                {
                    SwitchToSmallSkill();
                }
            }

            if(NormalizedTime() > 0.1f)
            {
                NormalAttackCombo();
            }
            SwitchToEvade();
        }
        else
        {
            if (IsAnimationEnd()) { playerModel.CurrentNormalAttackIndex = 0; playerController.SwitchState(PlayerState.PerfectNormalAttackEnd); return; }
        }
    }

    private void NormalAttackCombo()
    {
        if (playerModel.animator.GetCurrentAnimatorStateInfo(0).IsName(playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].animationName))
        {
            if (statePlayTimeCullAnimatorSpeed >= playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeStart && statePlayTimeCullAnimatorSpeed <= playerModel.normalAttackConfigs[playerModel.perfectNormalIndex].perfectTimeEnd && !enterNextAttack)
            {
                PerfectNormalAttack();
            }
            else
            {
                if (playerController.inputSystem.Player.Fire.triggered)
                {
                    if (NormalizedTime() >= 0.5f)
                    {
                        clickCount++;
                        enterNextAttack = true;
                    }
                }
            }
                
        }
        else
        {
            if (playerController.inputSystem.Player.Fire.triggered)
            {
                if (NormalizedTime() >= 0.5f)
                {
                    clickCount++;
                    enterNextAttack = true;
                }
            }
        }
            

        if (playerModel.CurrentNormalAttackIndex == playerModel.normalAttackConfigs.Length - 2) FinalNormalAttack();
        else FrontMountedNormalAttack();
    }

    private void FrontMountedNormalAttack()
    {
        if (NormalizedTime() > 0.75f && !playerModel.animator.IsInTransition(0))
            if (enterNextAttack && clickCount > 0) {  playerModel.CurrentNormalAttackIndex++ ; playerController.SwitchState(PlayerState.NormalAttack); return; }

        if (IsAnimationEnd()) { playerController.SwitchState(PlayerState.NormalAttackEnd); return; }
    }

    private void FinalNormalAttack()
    {
        if (NormalizedTime() > 0.9f && !playerModel.animator.IsInTransition(0))
            if (enterNextAttack && clickCount > 0) { playerModel.CurrentNormalAttackIndex++; playerController.SwitchState(PlayerState.NormalAttack); return; }

        if (IsAnimationEnd()) { playerController.SwitchState(PlayerState.NormalAttackEnd); return; }
    }

    private void PerfectNormalAttack()
    {
        if (playerController.inputSystem.Player.Fire.triggered && clickCount == 0) { playerController.SwitchState(PlayerState.PerfectNormalAttack); return; }
    }

    private void SwitchToPerfectSkill()
    {
        if (playerController.inputSystem.Player.Branch.triggered && clickCount == 0)
        {
            if (playerModel.playerData.CurrentBranchEnergy < playerModel.playerData.BranchNeedEnergy)
            {
                playerController.SwitchState(PlayerState.SmallSkill_NoEnergyPerfect);
                return;
            }

            if (playerModel.playerData.CurrentBranchEnergy >= playerModel.playerData.BranchNeedEnergy)
            {
                playerController.SwitchState(PlayerState.SmallSkill_EnergyPerfect);
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        clickCount = 0;
    }
}
