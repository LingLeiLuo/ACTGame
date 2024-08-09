using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    #region 血量
    [SerializeField] private float currentHP;
    [SerializeField] private float maxHP;
    #endregion

    #region 技能能量
    [SerializeField] private float currentBranchEnergy;
    [SerializeField] private float maxBranchEnergy;
    [SerializeField] private float branchNeedEnergy;
    #endregion

    #region 大招能量
    [SerializeField] private float exNeedEnergy;
    #endregion

    public Sprite characterImage;

    public float CurrentHP { get => currentHP; set => currentHP = value; }
    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float CurrentBranchEnergy 
    { 
        get => currentBranchEnergy; 
        set 
        {
            if (value >= MaxBranchEnergy)
                currentBranchEnergy = MaxBranchEnergy;
            else
                currentBranchEnergy = value;
        } 
    }
    public float MaxBranchEnergy { get => maxBranchEnergy; set => maxBranchEnergy = value; }
    public float BranchNeedEnergy { get => branchNeedEnergy; set => branchNeedEnergy = value; }
    public float ExNeedEnergy { get => exNeedEnergy; set => exNeedEnergy = value; }
}
