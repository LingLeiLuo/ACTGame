using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���������
/// </summary>
public class CameraManager : SingleMono<CameraManager>
{
    //CM�Ĵ������
    public CinemachineBrain cm_brain;
    //�������
    public GameObject freeLookCanmera;
    //������������
    public CinemachineFreeLook freeLook;

    /// <summary>
    /// ��������������ӽ�
    /// </summary>
    public void ResetFreeLookCamera()
    {
        freeLook.m_YAxis.Value = 0.5f;
        freeLook.m_XAxis.Value = PlayerController.INSTANCE.playerModel.transform.eulerAngles.y;
    }
}
