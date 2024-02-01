using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAmpOnDash : ItemBase
{
    PlayerDashController playerDashController;
    EventBinding<UseCardEvent> useCardEvent;

    public override void Initialize()
    {
        useCardEvent = new EventBinding<UseCardEvent>(RemoveBuff);
        EventBus<UseCardEvent>.Register(new EventBinding<UseCardEvent>(RemoveBuff));

        playerDashController = player.DashController;
        player.DashController.OnDash += Proc;
        base.Initialize();
    }


    public override void Proc()
    {
        base.Proc();
    }

    void RemoveBuff()
    {

    }

    public override void Deactivate()
    {
        EventBus<UseCardEvent>.Deregister(useCardEvent);
        player.DashController.OnDash -= Proc;
        base.Deactivate();
    }

}
