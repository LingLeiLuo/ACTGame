
using System.Collections.Generic;
using UnityEngine;

public class AIModel : ModelBase
{
    private AIController aiController;

    public AIState currentState;

    public override void Init(ISkillOwner skillOwner, List<string> enemyTagList)
    {
        base.Init(skillOwner, enemyTagList);

        aiController = GetComponentInParent<AIController>();
        aiController.audioSource = GetComponent<AudioSource>();
    }

    private void OnAnimatorMove()
    {

    }
}
