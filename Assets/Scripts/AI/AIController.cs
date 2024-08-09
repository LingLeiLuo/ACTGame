using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugManager;

public class AIController : MonoBehaviour, IHurt, IStateMachineOwner, ISkillOwner
{
    private StateMachine stateMachine;
    private AIModel aiModel;

    [HideInInspector] public AudioSource audioSource;

    public Collider nearestEnemyCollider;
    public List<string> enemyTagList;

    [Header("�Ƿ��������״���µ��չ�λ��")]
    public bool ControlNormalAttackDeltaPos;


    public AIModel Model { get => aiModel; }


    private void Start()
    {
        aiModel = FindFirstObjectByType<AIModel>();
        aiModel.Init(this, enemyTagList);
        stateMachine = new StateMachine(this);
        SwitchState(AIState.Idle);
    }

    public void SwitchState(AIState aiState, bool reCurrState = false)
    {
        switch (aiState)
        {
            case AIState.Idle:
                stateMachine.EnterState<AIIdleState>(reCurrState);
                break;
            case AIState.Hurt:
                stateMachine.EnterState<AIHurtState>(reCurrState);
                break;
        }
    }

    public void Hurt()
    {
        SwitchState(AIState.Hurt, true);
    }

    public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.25f)
    {
        Model.animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null) audioSource.PlayOneShot(audioClip);
    }

    #region UnityEditor
#if UNITY_EDITOR
    [ContextMenu("SetHurtCollider")]
    private void SetHurtCollider()
    {
        // ����������ײ��ΪHurtCollider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<WeaponController>() == null)
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("HurtCollider");
            }
            else
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
        // ��ǳ����޸�
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }

#endif
    #endregion


    public void StartSkillHit()
    {

    }

    public void StopSkillHit()
    {

    }

    public void OnHit(IHurt target, Vector3 hitPos, float KaRouTime)
    {

    }
}
