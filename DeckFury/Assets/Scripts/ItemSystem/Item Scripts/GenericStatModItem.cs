using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStatModItem : ItemBase
{
    [SerializeField] int _maxHPMod;
    [SerializeField] int _currentHPMod;
    [SerializeField] int _shieldMod;
    [SerializeField] int _armorMod;
    [SerializeField] float _defenseMod;

    [Tooltip("If Permanent Upgrade is true, modifications will go to the current run's player data rather " + 
    "than only the player object in the current stage. Initialize will not be called, only the PersistentInitialize will be called.")]
    [SerializeField] bool _permanentUpgrade = false;

    public override void PersistentInitialize()
    {
        if(PersistentInitialized) { return; }

        if(PersistentLevelController.Instance)
        {
            PlayerDataContainer playerData = PersistentLevelController.Instance.PlayerData;
            playerData.CurrentHP += _currentHPMod;
            playerData.SetMaxHP(playerData.MaxHP + _maxHPMod);
            playerData.SetBaseShieldHP(playerData.BaseShieldHP + _shieldMod);
            playerData.SetBaseArmor(playerData.BaseArmor + _armorMod);
            playerData.SetBaseDefense(playerData.BaseDefense + _defenseMod);
        }

        if(player)
        {
            player.CurrentHP += _currentHPMod;
            player.ShieldHP += _shieldMod;
            player.Armor += _armorMod;
            player.Defense += _defenseMod;
        }

        base.PersistentInitialize();
    }

    public override void Initialize()
    {
        if(_permanentUpgrade) { return; }
        if(Initialized) { return; }

        player.CurrentHP += _currentHPMod;
        player.ShieldHP += _shieldMod;
        player.Armor += _armorMod;
        player.Defense += _defenseMod;

        base.Initialize();
    }


}
