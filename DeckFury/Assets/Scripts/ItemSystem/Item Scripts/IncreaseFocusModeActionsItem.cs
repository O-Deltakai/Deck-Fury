using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseFocusModeActionsItem : ItemBase
{
    public override void Initialize()
    {
        FocusModeController.Instance.MaxNumberOfActions += itemSO.QuantifiableEffects[0].IntegerQuantity;
        base.Initialize();
    }

    public override void Deactivate()
    {

        base.Deactivate();
    }



}
