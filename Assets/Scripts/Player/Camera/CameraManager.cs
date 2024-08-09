using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 相机管理器
/// </summary>
public class CameraManager : SingleMono<CameraManager>
{
    //CM的大脑组件
    public CinemachineBrain cm_brain;
    //自由相机
    public GameObject freeLookCanmera;
    //自由相机的组件
    public CinemachineFreeLook freeLook;

    /// <summary>
    /// 重置自由相机的视角
    /// </summary>
    public void ResetFreeLookCamera()
    {
        freeLook.m_YAxis.Value = 0.5f;
        freeLook.m_XAxis.Value = PlayerController.INSTANCE.playerModel.transform.eulerAngles.y;
    }
}
