using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类
/// </summary>
public abstract class StateBase
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="owner">宿主</param>
    public abstract void Init(IStateMachineOwner owner);

    /// <summary>
    /// 释放资源
    /// </summary>
    public abstract void UnInit();

    /// <summary>
    /// 进入状态
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 结束状态
    /// </summary>
    public abstract void Exit();

    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();
}