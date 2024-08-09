
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerModel : ModelBase
{
    private PlayerController playerController;
    public PlayerData playerData;
    [HideInInspector] public int EvadeCount = 2;

    private float resetEvadeTime = 1.25f;
    private float currentTime;

    // 当前角色状态
    [HideInInspector] public PlayerState currentState;

    public GameObject BigSkillCamera;

    [HideInInspector] public int perfectNormalIndex;

    protected override void Awake()
    {
        base.Awake();

        perfectNormalIndex = normalAttackConfigs.Length - 1;
    }

    private void Update()
    {
        if (playerController.currentSkillConfig != null)
            if (playerController.currentSkillConfig.hasPerfectPoint)
                perfectNormalIndex = Array.IndexOf(normalAttackConfigs, playerController.currentSkillConfig);

        EvadeCountControl();
    }

    /// <summary>
    /// 计算闪避次数
    /// </summary>
    private void EvadeCountControl()
    {
        if (EvadeCount <= 0)
        {
            currentTime += Time.unscaledDeltaTime;
            if (currentTime >= resetEvadeTime)
                EvadeCount = 2;
        }
        else if (0 < EvadeCount && EvadeCount <= 1)
        {
            currentTime += Time.unscaledDeltaTime;
            if (currentTime >= 0.8f)
                EvadeCount = 2;
        }
        else
        {
            currentTime = 0;
        }
    }

    public override void Init(ISkillOwner skillOwner, List<string> enemyTagList)
    {
        base.Init(skillOwner, enemyTagList);

        playerController = GetComponentInParent<PlayerController>();
        playerController.audioSource = GetComponent<AudioSource>();
    }


    #region 角色切换
    /// <summary>
    /// 模型入场
    /// </summary>
    public void Enter(Vector3 pos, Quaternion rot)
    {
        //强行移除退场逻辑
        MonoManager.INSTANCE.RemoveUpdateAction(OnExit);

        #region 设置角色出场位置
        //计算向右的向量
        Vector3 rightDirection = rot * Vector3.right;
        //向右偏移0.8个单位
        pos += rightDirection * 0.8f;
        //计算向后的向量
        Vector3 backDirection = rot * Vector3.back;
        //向后偏移3个单位
        pos += backDirection * 4f;

        characterController.Move(pos - transform.position);
        transform.rotation = rot;
        #endregion
    }

    /// <summary>
    /// 模型退场
    /// </summary>
    public void Exit()
    {
        animator.speed = 1f;
        currentState = PlayerState.Idle;
        animator.CrossFade("SwitchOut_Normal", 0.1f);
        foreach( WeaponController weaponController in weapons)
            weaponController.UnInit(skillOwner.OnHit);
        MonoManager.INSTANCE.AddUpdateAction(OnExit);
    }

    /// <summary>
    /// 退场逻辑
    /// </summary>
    public void OnExit()
    {
        if (IsAnimationEnd())
        {
            gameObject.SetActive(false);

            MonoManager.INSTANCE.RemoveUpdateAction(OnExit);
        }
    }
    #endregion

   

    /// <summary>
    /// 控制根运动
    /// </summary>
    private void OnAnimatorMove()
    {
        RootMotion();
    }

    private void RootMotion()
    {
        if (playerController.nearestEnemyCollider != null)
        {
            Vector3 EnemyPos = playerController.nearestEnemyCollider.transform.root.gameObject.GetComponentInChildren<AIModel>().gameObject.transform.position;
            Vector2 EnemyV2Pos = new Vector2(EnemyPos.x, EnemyPos.z);
            Vector2 PlayerV2Pos = new Vector2(transform.position.x, transform.position.z);

            #region 索敌在指定距离内
            if (Vector3.Distance(EnemyV2Pos, PlayerV2Pos) < 2f && playerController.ControlNormalAttackDeltaPos)
            {
                //switch (currentState)
                //{
                //    case PlayerState.BigSkill:
                //    case PlayerState.NormalAttack:
                //    case PlayerState.PerfectNormalAttack:
                //    case PlayerState.SmallSkill_NoEnergy:
                //    case PlayerState.SmallSkill_NoEnergyPerfect:
                //    case PlayerState.SmallSkill_EnergyPerfect:

                //        Vector2 movementV2 = new Vector2(animator.deltaPosition.x, animator.deltaPosition.z);
                //        Vector2 direction = EnemyV2Pos - PlayerV2Pos;
                //        float dotProduct = Vector2.Dot(movementV2, direction.normalized);
                //        Vector2 projection = direction.normalized * dotProduct;
                //        Vector2 resultVector = movementV2 - projection;


                //        characterController.Move(new Vector3(0, animator.deltaPosition.y, resultVector.y));
                //        return;
                //    default:
                //        characterController.Move(animator.deltaPosition);
                //        transform.localRotation *= animator.deltaRotation;
                //        return;
                //}

                characterController.Move(new Vector3(0, animator.deltaPosition.y, 0));

                Vector2 direction_01 = (EnemyV2Pos - PlayerV2Pos).normalized;
                Vector2 pos = EnemyV2Pos - direction_01 * 2;
                Vector2 movement = pos - PlayerV2Pos;
                Vector3 movmentV3 = new Vector3(movement.x, animator.deltaPosition.y, movement.y);

                characterController.Move(movmentV3);
                transform.localRotation *= animator.deltaRotation;
                return;
            }
            #endregion

            #region 索敌但在指定距离外
            else
            {
                if (currentState == PlayerState.SmallSkill_NoEnergy) { characterController.Move(animator.deltaPosition * 2); }
                else { characterController.Move(animator.deltaPosition); }

                if (currentState != PlayerState.BigSkill && currentState != PlayerState.SmallSkill_NoEnergy && currentState != PlayerState.SmallSkill_Energy)
                    transform.localRotation *= animator.deltaRotation;
            }
            #endregion
        }
        #region 没索敌
        else
        {
            if (currentState == PlayerState.BigSkill && currentState == PlayerState.SmallSkill_Energy) { characterController.Move(new Vector3(0, animator.deltaPosition.y, 0)); }
            else
            {
                characterController.Move(animator.deltaPosition);
                transform.localRotation *= animator.deltaRotation;
            }
        }
        #endregion
    }



    #region 动画事件
    Coroutine Coroutine;

    /// <summary>
    /// 动画事件播放音频
    /// </summary>
    /// <param name="audioClip"></param>
    private void PlayAudio(AudioClip audioClip)
    {
        playerController.PlayAudio(audioClip);
    }

    /// <summary>
    /// 开始攻击
    /// </summary>
    /// <param name="PauseFrameTime">顿帧时间</param>
    protected void StartSkillHit(float PauseFrameTime)
    {
        if(Coroutine != null) { StopCoroutine(Coroutine); }
        skillOwner.StartSkillHit();
        Coroutine = StartCoroutine(Hit(PauseFrameTime));
    }

    private IEnumerator Hit(float PauseFrameTime)
    {
        List<float> intervals = playerController.currentSkillConfig.AttackData[playerController.currentHitIndex].HitIntervals;
        int hitCount = playerController.currentSkillConfig.AttackData[playerController.currentHitIndex].HitCount;
        Vector3 hitBoxSize = playerController.currentSkillConfig.AttackData[playerController.currentHitIndex].hitBoxSize;
        Vector3 hitBoxCenter = playerController.currentSkillConfig.AttackData[playerController.currentHitIndex].hitBoxCenter;

        base.PauseFrameTime = PauseFrameTime;

        for (int i = 0; i < hitCount; i++)
        {
            foreach (WeaponController weaponController in weapons)
                weaponController.StartSkillHit(hitBoxSize, hitBoxCenter);

            yield return new WaitForSeconds(intervals[i]);
        }
        StopSkillHit();
    }

    /// <summary>
    /// 停止攻击
    /// </summary>
    public void StopSkillHit()
    {
        foreach (WeaponController weaponController in weapons)
            weaponController.StopSkillHit();
        skillOwner.StopSkillHit();
    }

    /// <summary>
    /// 时间减速
    /// </summary>
    /// <param name="time">减速的倍数</param>
    protected void SlowTime(float time)
    {
        StartCoroutine(SlowTimeAndQuickTime(time));
    }

    IEnumerator SlowTimeAndQuickTime(float time)
    {
        Time.timeScale = time;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
    }

    #endregion
}
