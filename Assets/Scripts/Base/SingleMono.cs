using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMono<T> : MonoBehaviour where T : SingleMono<T>
{
    public static T INSTANCE;

    protected virtual void Awake()
    {
        if (INSTANCE != null) 
        {
            Debug.LogError(this + "不符合单例模式");
        }
        INSTANCE = (T)this;
    }

    protected virtual void OnDestroy()
    {
        Destroy();         
    }

    /// <summary>
    /// 清除子类单例
    /// </summary>
    public void Destroy()
    {
        INSTANCE = null;
    }
}
