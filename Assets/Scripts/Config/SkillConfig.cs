using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill")]
public class SkillConfig : ScriptableObject
{
    public string animationName;

    public bool hasPerfectPoint;

    public float perfectTimeStart;

    public float perfectTimeEnd;

    public Skill_ReleaseData releaseData;

    public Skill_AttackData[] AttackData;

    public float SwitchAttackTime;
}

// �����ͷ�����
[Serializable]
public class Skill_ReleaseData
{
    // �������ӻ���������
    public Skill_SpawnObject spawnObject;
    // ��Ч
    public AudioClip audioClip;
}

// ��������
[Serializable]
public class Skill_AttackData
{
    // �������ӻ���������
    public Skill_SpawnObject spawnObject;

    public AudioClip audioClip;

    public float DamageValue;

    public float HardTime;

    public Vector3 RepelVelocity;

    public float RepelTime;

    public float ScreenImpulseValue;

    public float ChromaticAberrationValue;

    [Header("�������")]
    public int HitCount;

    [Header("�ü����µĹ������")]
    public float AttackInterval;

    [Header("������")]
    public List<float> HitIntervals;

    public Vector3 hitBoxSize;

    public Vector3 hitBoxCenter;

    // ���в�����Ч��
    public SkillHitEFConfig SkillHitEFConfig;

}

// ��������
[Serializable]
public class Skill_SpawnObject
{
    // ���ɵ�Ԥ����
    public GameObject prefab;
    // ���ɵ���Ч
    public AudioClip audioClip;
    // λ��
    public Vector3 position;
    // ��ת
    public Vector3 rotation;
    // �ӳ�ʱ��
    public float time;
}

// �չ�����
