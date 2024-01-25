using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPToShieldConverterItem : ItemBase
{
    public PlayerDataContainer playerData = new PlayerDataContainer();

    public override void Initialize()
    {
        base.Initialize();
        if(PersistentLevelController.Instance)
        {
            playerData = PersistentLevelController.Instance.PlayerData;
        }else
        if(StageStateController.Instance)
        {
            playerData = StageStateController.Instance.PlayerData;
        }else
        {
            Debug.LogWarning("HP To Shield Converter could not find player data to modify, using default player data for modifications.");
            
        }

        player.ShieldHP += (int)(playerData.MaxHP * (itemSO.QuantifiableEffects[0].FloatQuantity * 0.01f));
        player.CurrentHP = DivideAndRoundUp(player.CurrentHP, (int)(1f / (itemSO.QuantifiableEffects[1].FloatQuantity * 0.01f)));
        
    }

    //Used to make sure the player current hp rounds up at all times to avoid the edge case where being at 1 hp would kill the player
    //due to rounding down to zero.
    int DivideAndRoundUp(int number, int divisor)
    {
        return (number / divisor) + ((number % divisor) > 0 ? 1 : 0);
    }

}
