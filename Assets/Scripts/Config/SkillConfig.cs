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

// 技能释放数据
[Serializable]
public class Skill_ReleaseData
{
    // 播放粒子或生成物体
    public Skill_SpawnObject spawnObject;
    // 音效
    public AudioClip audioClip;
}

// 技能数据
[Serializable]
public class Skill_AttackData
{
    // 播放粒子或生成物体
    public Skill_SpawnObject spawnObject;

    public AudioClip audioClip;

    public float DamageValue;

    public float HardTime;

    public Vector3 RepelVelocity;

    public float RepelTime;

    public float ScreenImpulseValue;

    public float ChromaticAberrationValue;

    [Header("击打次数")]
    public int HitCount;

    [Header("该技能下的攻击间隔")]
    public float AttackInterval;

    [Header("击打间隔")]
    public List<float> HitIntervals;

    public Vector3 hitBoxSize;

    public Vector3 hitBoxCenter;

    // 命中产生的效果
    public SkillHitEFConfig SkillHitEFConfig;

}

// 技能生成
[Serializable]
public class Skill_SpawnObject
{
    // 生成的预制体
    public GameObject prefab;
    // 生成的音效
    public AudioClip audioClip;
    // 位置
    public Vector3 position;
    // 旋转
    public Vector3 rotation;
    // 延迟时间
    public float time;
}

// 普攻数据
