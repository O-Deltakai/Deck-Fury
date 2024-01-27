using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStatModItem : ItemBase
{
    [SerializeField] int _currentHPMod;
    [SerializeField] int _shieldMod;
    [SerializeField] int _armorMod;
    [SerializeField] float _defenseMod;

    public override void Initialize()
    {
        base.Initialize();

        player.CurrentHP += _currentHPMod;
        player.ShieldHP += _shieldMod;
        player.Armor += _armorMod;
        player.Defense += _defenseMod;

    }


}
