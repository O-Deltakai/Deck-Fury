using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardUpgradeData
{
    [SerializeField] CardSO _upgradedCard;
    public CardSO UpgradedCard => _upgradedCard;

}
