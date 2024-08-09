
using UnityEngine;

public class PlayerSmallSkillState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.animator.speed = 1;
        playerModel.CurrentNormalAttackIndex = 0;

        switch (playerModel.currentState)
        {
            case PlayerState.SmallSkill_NoEnergy:
                playerController.StartAttack(playerModel.skillAttackConfigs[0]);
                break;
            case PlayerState.SmallSkill_Energy:
                playerModel.playerData.CurrentBranchEnergy -= playerModel.playerData.BranchNeedEnergy;
                playerController.StartAttack(playerModel.skillAttackConfigs[1]);
                break;
            case PlayerState.SmallSkill_NoEnergyPerfect:
                playerController.StartAttack(playerModel.skillAttackConfigs[2]);
                break;
            case PlayerState.SmallSkill_EnergyPerfect:
                playerModel.playerData.CurrentBranchEnergy -= playerModel.playerData.BranchNeedEnergy;
                playerController.StartAttack(playerModel.skillAttackConfigs[3]);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        if (IsAnimationEnd())
        {
            switch (playerModel.currentState)
            {
                case PlayerState.SmallSkill_NoEnergy:
                    playerController.SwitchState(PlayerState.SmallSkill_NoEnergyEnd);
                    return;
                case PlayerState.SmallSkill_Energy:
                    playerController.SwitchState(PlayerState.SmallSkill_EnergyEnd);
                    return;
                case PlayerState.SmallSkill_NoEnergyPerfect:
                    if(statePlayTimeCullAnimatorSpeed >= playerModel.normalAttackConfigs[playerModel.CurrentNormalAttackIndex].SwitchAttackTime)
                    {
                        SwitchToEvade();
                        SwitchToNormalAttackTriggered();
                        SwitchToWalk();
                    }
                    if (IsAnimationEnd()) { SwitchToIdle(); }
                    return;
                case PlayerState.SmallSkill_EnergyPerfect:
                    playerController.SwitchState(PlayerState.SmallSkill_EnergyPerfectEnd);
                    return;
            }
        }

        if (playerController.nearestEnemyCollider != null && NormalizedTime() < 0.2f)
        {
            playerModel.transform.LookAt(playerController.nearestEnemyCollider.transform.position);
        }
    }
}
