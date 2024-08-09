using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public TMP_Text ExEnergy;
    public TMP_Text HPText;

    public Button EvadeBtn;
    public Button SkillBtn;
    public Button BigSkillBtn;

    public List<Sprite> skillSprites;
    public List<Sprite> bigSkillSprites;

    public List<Image> characterImage;
    public List<Slider> characterHPSlider;
    public List<Slider> characterEnergySlider;

    private int character2Index;
    private int character3Index;

    public PlayerController playerController;
    private List<PlayerData> playerData = new List<PlayerData>(3);

    private void Awake()
    {
        playerData.Add(null);
        playerData.Add(null);
        playerData.Add(null);
    }

    private void Update()
    {
        if (playerController.currentModelIndex + 1 == playerController.playerConfig.models.Length) character2Index = 0;
        else character2Index = playerController.currentModelIndex + 1;

        if (playerController.currentModelIndex - 1 < 0) character3Index = playerController.playerConfig.models.Length - 1;
        else character3Index = playerController.currentModelIndex - 1;

        playerData[0] = playerController.playerModel.playerData;
        playerData[1] = playerController.playerConfig.models[character2Index].GetComponent<PlayerModel>().playerData;
        playerData[2] = playerController.playerConfig.models[character3Index].GetComponent<PlayerModel>().playerData;

        for (int i = 0; i < 3; i++)
        {
            characterImage[i].sprite = playerData[i].characterImage;
            characterHPSlider[i].value = (float)playerData[i].CurrentHP / playerData[i].MaxHP;
            characterEnergySlider[i].value = (float)playerData[i].CurrentBranchEnergy / playerData[i].MaxBranchEnergy;

            if(playerData[i].CurrentBranchEnergy < playerData[i].BranchNeedEnergy)
            {
                characterEnergySlider[i].fillRect.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            if(playerData[i].CurrentBranchEnergy >= playerData[i].BranchNeedEnergy)
            {
                characterEnergySlider[i].fillRect.GetComponent<Image>().color = new Color(0.8f, 0.65f, 0.95f, 1);
            }
        } 




        ExEnergy.text = $"{playerController.ExPTS}PTS";
        HPText.text = $"{playerData[0].CurrentHP} / {playerData[0].MaxHP}";

        if (playerController.playerModel.EvadeCount <= 0) EvadeBtn.image.color = Color.gray;
        else EvadeBtn.image.color = Color.white;

        if (playerController.ExPTS >= 3000) BigSkillBtn.image.sprite = bigSkillSprites[1];
        else BigSkillBtn.image.sprite = bigSkillSprites[0];

        if (playerData[0].CurrentBranchEnergy >= playerData[0].BranchNeedEnergy) SkillBtn.image.sprite = skillSprites[1];
        else SkillBtn.image.sprite = skillSprites[0];

    }
}
