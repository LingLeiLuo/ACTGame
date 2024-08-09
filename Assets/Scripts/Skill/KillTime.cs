using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class KillTime : MonoBehaviour
{
    public float KillDelayTime = 1;
    private PlayerController controller;

    private void Awake()
    {
        controller = FindFirstObjectByType<PlayerController>();
        Destroy(gameObject, KillDelayTime);
    }

    private void Update()
    {
        if (controller.playerModel.currentState != PlayerState.NormalAttack &&
            controller.playerModel.currentState != PlayerState.SmallSkill_NoEnergy &&
            controller.playerModel.currentState != PlayerState.SmallSkill_NoEnergyPerfect &&
            controller.playerModel.currentState != PlayerState.BigSkill &&
            controller.playerModel.currentState != PlayerState.SmallSkill_Energy &&
            controller.playerModel.currentState != PlayerState.SmallSkill_EnergyPerfect &&
            controller.playerModel.currentState != PlayerState.PerfectNormalAttack)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
