public class AIIdleState : AIStateBase
{
    public override void Enter()
    {
        base.Enter();

        aiController.PlayAnimation("Idle");
    }
}
