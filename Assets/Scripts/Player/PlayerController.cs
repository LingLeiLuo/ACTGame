using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : SingleMono<PlayerController>,IStateMachineOwner,ISkillOwner
{
    public InputSystem inputSystem;

    [SerializeField] public CinemachineImpulseSource cinemachineImpulseSource;

    [HideInInspector] public AudioSource audioSource;

    [HideInInspector] public Vector2 inputMoveVector2;

    public PlayerModel playerModel;

    private StateMachine stateMachine;

    public float rotationSpeed = 8f;


    //当前控制的角色下标
    [HideInInspector] public int currentModelIndex;


    [Header("是否开启慢跑升级")]
    public bool WalkLevelRun;
    [Header("是否开启急回头")]
    public bool CanTurnBack;
    [Header("是否控制索敌状况下的普攻位移")]
    public bool ControlNormalAttackDeltaPos;
    [Header("是否开启无限闪避次数")]
    public bool SuperEvade;

    //玩家配置信息
    public PlayerConfig playerConfig;

    //配队
    private List<PlayerModel> controllableModels;

    // 敌人的标签
    [Header("敌人检测")]
    public List<string> enemyTagList;
    public LayerMask layerMask;
    public Collider nearestEnemyCollider;
    public float detectionRadius;




    [Header("玩家信息")]
    private int exPTS = 0;
    public int ExPTS 
    { 
        get => exPTS;
        set
        {
            if (value >= 3000)
                exPTS = 3000;
            else 
                exPTS = value;
        }
    }



    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine(this);
        inputSystem = new InputSystem();

        controllableModels = new List<PlayerModel>();

        #region 生产角色模型
        for (int i = 0; i < playerConfig.models.Length; i++)
        {
            GameObject modle = Instantiate(playerConfig.models[i], transform);
            controllableModels.Add(modle.GetComponent<PlayerModel>());
            controllableModels[i].gameObject.SetActive(false);
        }
        #endregion

        #region 操控第一个角色
        currentModelIndex = 0;
        controllableModels[currentModelIndex].gameObject.SetActive(true);
        playerModel = controllableModels[currentModelIndex];
        #endregion

    }

    private void Start()
    {
        playerModel.Init(this, enemyTagList);
        SwitchState(PlayerState.Idle);

    }

    private void Update()
    {
        FindNearestColliderInRadius();
        // 更新玩家的移动输入
        inputMoveVector2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;
    }
    private void FixedUpdate()
    {
        
    }

    void FindNearestColliderInRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(playerModel.transform.position, detectionRadius, layerMask);

        float nearestDistance = Mathf.Infinity;
        nearestEnemyCollider = null;

        // 遍历检测到的碰撞体，找到最近的一个
        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(playerModel.transform.position, col.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemyCollider = col;
            }
        }
    }

    public void SwitchState(PlayerState playerState)
    {
        playerModel.currentState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Idle_AFK:
                stateMachine.EnterState<PlayerIdleState>(true);
                break;
            case PlayerState.Walk:
            case PlayerState.Run:
                stateMachine.EnterState<PlayerRunState>(true);
                break;
            case PlayerState.RunEnd:
                stateMachine.EnterState<PlayerRunEndState>();
                break;
            case PlayerState.TurnBack:
                stateMachine.EnterState<PlayerTurnBackState>();
                break;
            case PlayerState.Evade_Front:
            case PlayerState.Evade_Back:
                if (playerModel.EvadeCount <= 0) return;
                stateMachine.EnterState<PlayerEvadeState>();
                break;
            case PlayerState.Evade_Front_End:
            case PlayerState.Evade_Back_End:
                stateMachine.EnterState<PlayerEvadeEndState>();
                break;
            case PlayerState.NormalAttack:
            case PlayerState.PerfectNormalAttack:
                stateMachine.EnterState<PlayerNormalAttackState>(true);
                break;
            case PlayerState.NormalAttackEnd:
            case PlayerState.PerfectNormalAttackEnd:
                stateMachine.EnterState<PlayerNormalAttackEndState>();
                break;
            case PlayerState.SmallSkill_NoEnergy:
            case PlayerState.SmallSkill_Energy:
            case PlayerState.SmallSkill_EnergyPerfect:
            case PlayerState.SmallSkill_NoEnergyPerfect:
                stateMachine.EnterState<PlayerSmallSkillState>();
                break;
            case PlayerState.SmallSkill_NoEnergyEnd:
            case PlayerState.SmallSkill_EnergyEnd:
            case PlayerState.SmallSkill_EnergyPerfectEnd:
                stateMachine.EnterState<PlayerSmallSkillEndState>();
                break;
            case PlayerState.SwitchInNormal:
                stateMachine.EnterState<PlayerSwitchInNormalState>();
                break;
            case PlayerState.BigSkill:
            case PlayerState.BigSkill01:
            case PlayerState.BigSkillStart:
            case PlayerState.BigSkillEnd:
                stateMachine.EnterState<PlayerBigSkillState>(true);
                break;

        }
    }

    #region 模型切换
    /// <summary>
    /// 切换到下一个模型
    /// </summary>
    public void SwitchNextModel()
    {
        //刷新状态机
        stateMachine.Clear();
        //退出当前模型
        playerModel.Exit();
        #region 控制下一个模型
        currentModelIndex++;
        if (currentModelIndex >= controllableModels.Count)
        {
            currentModelIndex = 0;
        }
        PlayerModel nextModel = controllableModels[currentModelIndex];
        nextModel.gameObject.SetActive(true);

        Vector3 prevPos = playerModel.transform.position;
        Quaternion prevRot = playerModel.transform.rotation;

        playerModel = nextModel;
        #endregion
        //进入下一个模型
        playerModel.Enter(prevPos, prevRot);

        playerModel.Init(this, enemyTagList);
        //切换到入场状态
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// 切换到上一个模型
    /// </summary>
    public void SwitchLastModel()
    {
        //刷新状态机
        stateMachine.Clear();
        //退出当前模型
        playerModel.Exit();
        #region 控制下一个模型
        currentModelIndex--;
        if (currentModelIndex < 0)
        {
            currentModelIndex = controllableModels.Count - 1;
        }
        PlayerModel nextModel = controllableModels[currentModelIndex];
        nextModel.gameObject.SetActive(true);

        Vector3 prevPos = playerModel.transform.position;
        Quaternion prevRot = playerModel.transform.rotation;

        playerModel = nextModel;
        #endregion
        //进入下一个模型
        playerModel.Enter(prevPos, prevRot);
        //切换到入场状态
        SwitchState(PlayerState.SwitchInNormal);
    }
    #endregion

    public void ScreenImpulse(float force)
    {
        cinemachineImpulseSource.GenerateImpulse(force);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="fixedTransitionDurarion">过渡时间</param>
    public void PlayAnimation(string animationName, float fixedTransitionDurarion = 0.25f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="fixedTransitionDurarion">过渡时间</param>
    /// <param name="fixedTimeOffset">动画起始播放偏移</param>
    public void PlayAnimation(string animationName, float fixedTransitionDurarion = 0.25f, float fixedTimeOffset = 0f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion, 0, fixedTimeOffset);
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Disable();
    }

    #region 攻击接口实现

    public SkillConfig currentSkillConfig;
    public int currentHitIndex = 0;
    Coroutine PauseFrameCoroutine;

    public void StartAttack(SkillConfig currentSkillConfig)
    {

        playerModel.StopSkillHit();

        currentHitIndex = 0;

        this.currentSkillConfig = currentSkillConfig;

        // 播放动画
        PlayAnimation(currentSkillConfig.animationName, 0.1f);

        // 
        SpawnSkillObject(currentSkillConfig.releaseData.spawnObject);

        // 技能音效
        PlayAudio(currentSkillConfig.releaseData.audioClip);

    }

    public void StartSkillHit()
    {
        // 技能释放物体
        SpawnSkillObject(currentSkillConfig.AttackData[currentHitIndex].spawnObject);

        // 技能音效
        PlayAudio(currentSkillConfig.AttackData[currentHitIndex].audioClip);
    }

    public void StopSkillHit()
    {
        currentHitIndex++;
    }

    /// <summary>
    /// 生成技能特效等物体
    /// </summary>
    /// <param name="spawnObject"></param>
    private void SpawnSkillObject(Skill_SpawnObject spawnObject)
    {
        if (spawnObject != null && spawnObject.prefab != null)
        {
            StartCoroutine(DoSpawnObject(spawnObject));
        }
    }

    private IEnumerator DoSpawnObject(Skill_SpawnObject spawnObject)
    {
        yield return new WaitForSeconds(spawnObject.time);
        GameObject skillObject = Instantiate(spawnObject.prefab, playerModel.transform);
        skillObject.transform.localPosition = spawnObject.position;
        skillObject.transform.localEulerAngles = spawnObject.rotation;
    }

    /// <summary>
    /// 攻击效果方法
    /// </summary>
    /// <param name="target">攻击目标</param>
    /// <param name="hitPos">攻击点</param>
    /// <param name="pauseFrameTime"></param>
    public void OnHit(IHurt target, Vector3 hitPos, float pauseFrameTime)
    {
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitIndex];

        StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig, hitPos));

        if (attackData.ScreenImpulseValue != 0) ScreenImpulse(attackData.ScreenImpulseValue);
        if (attackData.ChromaticAberrationValue != 0) PostProcessingManager.INSTANCE.ChromaticAberrationEF(attackData.ChromaticAberrationValue);

        if (PauseFrameCoroutine != null) 
        {
            StopCoroutine(PauseFrameCoroutine);
        }
        PauseFrameCoroutine = StartCoroutine(PauseFrame(pauseFrameTime));

        ExPTS += 100;
        playerModel.playerData.CurrentBranchEnergy += 6;

        target.Hurt();
    }

    /// <summary>
    /// 受击效果生成
    /// </summary>
    /// <param name="hitEFConfig"></param>
    /// <param name="spawnPoint"></param>
    /// <returns></returns>
    private IEnumerator DoSkillHitEF(SkillHitEFConfig hitEFConfig, Vector3 spawnPoint)
    {
        if (hitEFConfig == null) yield break;

        PlayAudio(hitEFConfig.AudioClip);

        if (hitEFConfig != null && hitEFConfig.SpawnObject.prefab != null)
        {
            yield return new WaitForSeconds(hitEFConfig.SpawnObject.time);
            GameObject temp = Instantiate(hitEFConfig.SpawnObject.prefab);
            temp.transform.position = spawnPoint;
            temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles += hitEFConfig.SpawnObject.rotation;
            PlayAudio(hitEFConfig.SpawnObject.audioClip);
        }
    }

    IEnumerator PauseFrame(float pauseFrameTime)
    {
        playerModel.animator.speed = 0f;
        yield return new WaitForSeconds(pauseFrameTime);
        playerModel.animator.speed = 1f;
    }
    #endregion



    public void PlayAudio(AudioClip audioClip)
    {
        if(audioClip != null) audioSource.PlayOneShot(audioClip);
    }

    // 绘制球形检测范围以方便调试
    void OnDrawGizmosSelected()
    {
        if(playerModel != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerModel.transform.position, detectionRadius);
        }
    }
}
