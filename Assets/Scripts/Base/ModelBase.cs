using System.Collections.Generic;
using UnityEngine;


public enum ModelFoot
{
    Left, Right
}

public abstract class ModelBase : MonoBehaviour
{
    protected ISkillOwner skillOwner;

    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterController characterController;

     public WeaponController[] weapons;

    protected AnimatorStateInfo stateInfo;

    [Header("角色普攻配置")] public SkillConfig[] normalAttackConfigs;
    [Header("角色技能配置")] public SkillConfig[] skillAttackConfigs;
    [Header("角色大招配置")] public SkillConfig[] bigSkillAttackConfigs;
    public int currentNormalAttackIndex = 0;

    [HideInInspector] public float PauseFrameTime;

    [HideInInspector] public ModelFoot foot = ModelFoot.Left;
    [Header("——————————————————————————————————————————————————————————————————————————————————————")]
    [Header("角色重力配置")] public float gravity = -9.8f;


    public int CurrentNormalAttackIndex
    {
        get => currentNormalAttackIndex;
        set
        {
            if (value > normalAttackConfigs.Length - 2)
                currentNormalAttackIndex = 0;
            else
                currentNormalAttackIndex = value;
        }
    }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    public virtual void Init(ISkillOwner skillOwner, List<string> enemyTagList)
    {
        weapons = GetComponentsInChildren<WeaponController>();
        this.skillOwner = skillOwner;
        foreach (WeaponController weaponController in weapons)
            weaponController.Init(enemyTagList, skillOwner.OnHit);
    }

    /// <summary>
    /// 判断动画是否结束
    /// </summary>
    public bool IsAnimationEnd()
    {
        //刷新动画状态
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0);
    }

    protected void OnEnable()
    {
        CurrentNormalAttackIndex = 0;
    }

    #region 动画事件判断左右脚
    /// <summary>
    /// 迈出左脚
    /// </summary>
    public void SetOutLeftFoot()
    {
        foot = ModelFoot.Left;
    }

    /// <summary>
    /// 迈出右脚
    /// </summary>
    public void SetOutRightFoot()
    {
        foot = ModelFoot.Right;
    }
    #endregion

    
}

