using UnityEngine;

public interface ISkillOwner
{
    void StartSkillHit();

    void StopSkillHit();

    void OnHit(IHurt target, Vector3 hitPos, float KaRouTime);
}
