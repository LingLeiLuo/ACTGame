using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] public new Collider collider;

    [SerializeField] private List<string> enemyTagList;

    private List<IHurt> enemyList = new List<IHurt>();

    private Action<IHurt, Vector3, float> onHitAction;

    private PlayerController playerController;
   

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    public void Init(List<string> enemyTagList, Action<IHurt, Vector3, float> onHitAction)
    {
        this.enemyTagList.Add(enemyTagList[0]);
        this.onHitAction += onHitAction;
        
    }

    public void UnInit(Action<IHurt, Vector3, float> onHitAction)
    {
        enemyTagList.Clear();
        this.onHitAction -= onHitAction;

    }

    public void StartSkillHit(Vector3 hitBoxSize, Vector3 hitBoxCenter)
    {
        collider.GetComponent<BoxCollider>().size = hitBoxSize;
        collider.GetComponent<BoxCollider>().center = hitBoxCenter;
        collider.enabled = true;
        enemyList.Clear();
    }

    public void StopSkillHit()
    {
        collider.GetComponent<BoxCollider>().size = Vector3.zero;
        collider.GetComponent<BoxCollider>().center = Vector3.zero;
        collider.enabled = false;
        enemyList.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemyTagList == null) { return; }

        if (enemyTagList.Contains(other.transform.root.gameObject.GetComponentInChildren<AIModel>()?.gameObject.tag))
        {
            IHurt enemy = other.GetComponentInParent<IHurt>();

            // 若此次攻击已经攻击过这个单位，则不产生攻击
            if (enemy != null && !enemyList.Contains(enemy))
            {
                onHitAction?.Invoke(enemy, other.ClosestPoint(transform.position), playerController.playerModel.PauseFrameTime);
                enemyList.Add(enemy);
            }
        }
    }
}
