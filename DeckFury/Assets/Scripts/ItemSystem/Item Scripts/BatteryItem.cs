using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryItem : ItemBase
{

    public override void Initialize()
    {
        if(_initialized){ return; }

        base.Initialize();
        EnergyController energyController = EnergyController.Instance;
        energyController.chargeRateModifier += itemSO.QuantifiableEffects[0].FloatQuantity * 0.01f;

        stageManager = StageManager.Instance;

        _initialized = true;
    }


    public override void Deactivate()
    {
        base.Deactivate();

        _initialized = false;
    }

}
