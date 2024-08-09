using UnityEngine;


[CreateAssetMenu(menuName = "Config/SkillHitEFConfig")]
public class SkillHitEFConfig : ScriptableObject
{
    // 产生的粒子或物体
    public Skill_SpawnObject SpawnObject;

    public AudioClip AudioClip;
}
