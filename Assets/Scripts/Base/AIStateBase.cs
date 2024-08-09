
using UnityEditor;
using UnityEngine;

public enum AIState
{
    Idle,
    NormalAttack,
    Hurt
}

public class AIStateBase : StateBase
{
    protected AIController aiController;
    protected AIModel aiModel;

    //动画信息
    private AnimatorStateInfo stateInfo;

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void Init(IStateMachineOwner owner)
    {
        aiController = (AIController)owner;
        aiModel = aiController.Model;
    }

    public override void UnInit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void LateUpdate()
    {
        
    }

    /// <summary>
    /// 判断动画是否结束
    /// </summary>
    public bool IsAnimationEnd()
    {
        //刷新动画状态
        stateInfo = aiModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1f && !aiModel.animator.IsInTransition(0);
    }
}
