public class AIHurtState : AIStateBase
{
    public override void Enter()
    {
        base.Enter();

        aiController.PlayAnimation("Hit_Stay");
    }

    public override void Update()
    {
        base.Update();

        if (IsAnimationEnd())
        {
            aiController.SwitchState(AIState.Idle);
        }
    }
}
