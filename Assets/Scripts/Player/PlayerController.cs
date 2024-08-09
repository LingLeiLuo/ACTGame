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


    //��ǰ���ƵĽ�ɫ�±�
    [HideInInspector] public int currentModelIndex;


    [Header("�Ƿ�����������")]
    public bool WalkLevelRun;
    [Header("�Ƿ�������ͷ")]
    public bool CanTurnBack;
    [Header("�Ƿ��������״���µ��չ�λ��")]
    public bool ControlNormalAttackDeltaPos;
    [Header("�Ƿ����������ܴ���")]
    public bool SuperEvade;

    //���������Ϣ
    public PlayerConfig playerConfig;

    //���
    private List<PlayerModel> controllableModels;

    // ���˵ı�ǩ
    [Header("���˼��")]
    public List<string> enemyTagList;
    public LayerMask layerMask;
    public Collider nearestEnemyCollider;
    public float detectionRadius;




    [Header("�����Ϣ")]
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

        #region ������ɫģ��
        for (int i = 0; i < playerConfig.models.Length; i++)
        {
            GameObject modle = Instantiate(playerConfig.models[i], transform);
            controllableModels.Add(modle.GetComponent<PlayerModel>());
            controllableModels[i].gameObject.SetActive(false);
        }
        #endregion

        #region �ٿص�һ����ɫ
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
        // ������ҵ��ƶ�����
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

        // ������⵽����ײ�壬�ҵ������һ��
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

    #region ģ���л�
    /// <summary>
    /// �л�����һ��ģ��
    /// </summary>
    public void SwitchNextModel()
    {
        //ˢ��״̬��
        stateMachine.Clear();
        //�˳���ǰģ��
        playerModel.Exit();
        #region ������һ��ģ��
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
        //������һ��ģ��
        playerModel.Enter(prevPos, prevRot);

        playerModel.Init(this, enemyTagList);
        //�л����볡״̬
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// �л�����һ��ģ��
    /// </summary>
    public void SwitchLastModel()
    {
        //ˢ��״̬��
        stateMachine.Clear();
        //�˳���ǰģ��
        playerModel.Exit();
        #region ������һ��ģ��
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
        //������һ��ģ��
        playerModel.Enter(prevPos, prevRot);
        //�л����볡״̬
        SwitchState(PlayerState.SwitchInNormal);
    }
    #endregion

    public void ScreenImpulse(float force)
    {
        cinemachineImpulseSource.GenerateImpulse(force);
    }

    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="animationName">��������</param>
    /// <param name="fixedTransitionDurarion">����ʱ��</param>
    public void PlayAnimation(string animationName, float fixedTransitionDurarion = 0.25f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion);
    }

    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="animationName">��������</param>
    /// <param name="fixedTransitionDurarion">����ʱ��</param>
    /// <param name="fixedTimeOffset">������ʼ����ƫ��</param>
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

    #region �����ӿ�ʵ��

    public SkillConfig currentSkillConfig;
    public int currentHitIndex = 0;
    Coroutine PauseFrameCoroutine;

    public void StartAttack(SkillConfig currentSkillConfig)
    {

        playerModel.StopSkillHit();

        currentHitIndex = 0;

        this.currentSkillConfig = currentSkillConfig;

        // ���Ŷ���
        PlayAnimation(currentSkillConfig.animationName, 0.1f);

        // 
        SpawnSkillObject(currentSkillConfig.releaseData.spawnObject);

        // ������Ч
        PlayAudio(currentSkillConfig.releaseData.audioClip);

    }

    public void StartSkillHit()
    {
        // �����ͷ�����
        SpawnSkillObject(currentSkillConfig.AttackData[currentHitIndex].spawnObject);

        // ������Ч
        PlayAudio(currentSkillConfig.AttackData[currentHitIndex].audioClip);
    }

    public void StopSkillHit()
    {
        currentHitIndex++;
    }

    /// <summary>
    /// ���ɼ�����Ч������
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
    /// ����Ч������
    /// </summary>
    /// <param name="target">����Ŀ��</param>
    /// <param name="hitPos">������</param>
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
    /// �ܻ�Ч������
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

    // �������μ�ⷶΧ�Է������
    void OnDrawGizmosSelected()
    {
        if(playerModel != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerModel.transform.position, detectionRadius);
        }
    }
}
