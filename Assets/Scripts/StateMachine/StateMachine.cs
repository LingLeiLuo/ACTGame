using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public interface IStateMachineOwner { }

/// <summary>
/// ����״̬��
/// </summary>
public class StateMachine
{
    //��ǰ״̬
    private StateBase currentState;

    //�Ƿ������ǰ״̬
    public bool HasState { get => currentState != null; }

    //����
    private IStateMachineOwner owner;

    //״̬�ֵ�
    private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();

    public StateMachine(IStateMachineOwner owner)
    {
        Init(owner);
    }

    //��ʼ��
    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    /// <typeparam name="T">״̬����</typeparam>
    /// /// <typeparam name="reLoadState">�Ƿ�ˢ��״̬</typeparam>
    public void EnterState<T>(bool reLoadState = false) where T : StateBase, new()
    {
        //���״̬һ�£��򲻽����л�
        if (HasState && currentState.GetType() == typeof(T) && !reLoadState)
        {
            return;
        }

        #region ������ǰ״̬
        if (HasState)
        {
            ExitCurrentState();
        }
        #endregion

        #region ������״̬
        currentState = LoadState<T>();
        EnterCurrentState();
        #endregion
    }

    /// <summary>
    /// ���ػ򴴽���״̬
    /// </summary>
    /// <typeparam name="T">״̬����</typeparam>
    /// <returns></returns>
    private StateBase LoadState<T>() where T : StateBase, new()
    {
        //��ȡ״̬����
        Type stateType = typeof(T);

        //����ֵ��в����ڸ�״̬
        if (!stateDic.TryGetValue(stateType, out StateBase state))
        {
            //����һ����״̬�����浽�ֵ�
            state = new T();
            state.Init(owner);
            stateDic.Add(stateType, state);
        }

        return state;
    }

    private void EnterCurrentState()
    {
        currentState.Enter();
        MonoManager.INSTANCE.AddUpdateAction(currentState.Update);
        MonoManager.INSTANCE.AddFixedUpdateAction(currentState.FixedUpdate);
        MonoManager.INSTANCE.AddLateUpdateAction(currentState.LateUpdate);
    }
    private void ExitCurrentState()
    {
        currentState.Exit();
        MonoManager.INSTANCE.RemoveUpdateAction(currentState.Update);
        MonoManager.INSTANCE.RemoveFixedUpdateAction(currentState.FixedUpdate);
        MonoManager.INSTANCE.RemoveLateUpdateAction(currentState.LateUpdate);
    }

    /// <summary>
    /// ˢ��״̬��
    /// </summary>
    public void Clear()
    {
        ExitCurrentState();
        currentState = null;
        foreach (var item in stateDic.Values)
        {
            item.UnInit();
        }
        stateDic.Clear();
    }
}