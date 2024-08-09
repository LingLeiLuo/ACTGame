using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ״̬����
/// </summary>
public abstract class StateBase
{
    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="owner">����</param>
    public abstract void Init(IStateMachineOwner owner);

    /// <summary>
    /// �ͷ���Դ
    /// </summary>
    public abstract void UnInit();

    /// <summary>
    /// ����״̬
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// ����״̬
    /// </summary>
    public abstract void Exit();

    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();
}