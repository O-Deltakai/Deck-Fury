using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialBonusSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bonusName;
    [SerializeField] TextMeshProUGUI bonusNameDropShadow;

    [SerializeField] TextMeshProUGUI bonusScoreText;
    [SerializeField] TextMeshProUGUI bonusScoreDropShadow;

    [SerializeField] GameObject descriptionPanel;
    [SerializeField] TextMeshProUGUI descriptionPanelText;


    public BonusScoreItemSO currentBonusItem;


    void Start()
    {
        //descriptionPanel.SetActive(false);
    }

    public void SetElementsToScoreItem()
    {
        SetBonusName(currentBonusItem.RewardName);
        SetScoreText(currentBonusItem.BaseScore);
        SetDescriptionText(currentBonusItem.RewardDescription);
    }

    void SetBonusName(string name)
    {
        bonusName.text = name;
        bonusNameDropShadow.text = name;
    }

    void SetScoreText(int scoreValue)
    {
        bonusScoreText.text = "+" + scoreValue.ToString();
        bonusScoreDropShadow.text = "+" + scoreValue.ToString();
    }

    void SetDescriptionText(string text)
    {
        descriptionPanelText.text = text;
    }

    public void EnableDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
    }
    public void DisableDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
    }

}
