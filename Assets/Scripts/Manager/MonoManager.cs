using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : SingleMono<MonoManager>
{
    //update���񼯺�
    public Action updateAction;
    //fixedUpdate���񼯺�
    public Action fixedUpdateAction;
    //lateUpdate���񼯺�
    public Action lateUpdateAction;

    /// <summary>
    /// ���update����
    /// </summary>
    /// <param name="task">����</param>
    public void AddUpdateAction(Action task)
    {
        updateAction += task;
    }

    /// <summary>
    /// �Ƴ�update����
    /// </summary>
    /// <param name="task">����</param>
    public void RemoveUpdateAction(Action task)
    {
        updateAction -= task;
    }

    /// <summary>
    /// ���fixedUpdate����
    /// </summary>
    /// <param name="task">����</param>
    public void AddFixedUpdateAction(Action task)
    {
        fixedUpdateAction += task;
    }

    /// <summary>
    /// �Ƴ�fixedUpdate����
    /// </summary>
    /// <param name="task">����</param>
    public void RemoveFixedUpdateAction(Action task)
    {
        fixedUpdateAction -= task;
    }

    /// <summary>
    /// ���lateUpdate����
    /// </summary>
    /// <param name="task">����</param>
    public void AddLateUpdateAction(Action task)
    {
        lateUpdateAction += task;
    }

    /// <summary>
    /// �Ƴ�lateUpdate����
    /// </summary>
    /// <param name="task">����</param>
    public void RemoveLateUpdateAction(Action task)
    {
        lateUpdateAction -= task;
    }

    void Update()
    {
        updateAction?.Invoke();
    }

    void FixedUpdate()
    {
        fixedUpdateAction?.Invoke();
    }
    void LateUpdate()
    {
        lateUpdateAction?.Invoke();
    }
}
