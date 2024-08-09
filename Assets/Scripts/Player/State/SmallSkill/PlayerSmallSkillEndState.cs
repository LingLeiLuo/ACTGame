public class PlayerSmallSkillEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerModel.animator.speed = 1f;
        playerModel.CurrentNormalAttackIndex = 0;

        switch (playerModel.currentState)
        {
            case PlayerState.SmallSkill_NoEnergyEnd:
                playerController.PlayAnimation("Branch_NoEnergyEnd", 0f);
                break;
            case PlayerState.SmallSkill_EnergyEnd:
                playerController.PlayAnimation("Branch_EnergyEnd", 0f);
                break;
            case PlayerState.SmallSkill_EnergyPerfectEnd:
                playerController.PlayAnimation("Branch_EnergyPerfectEnd", 0f);
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        SwitchToBigSkill();

        SwitchToSmallSkill();

        SwitchToEvade();

        SwitchToWalk();

        SwitchToIdle();
    }
}
