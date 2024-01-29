using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashResetOnKillItem : ItemBase
{
    PlayerDashController playerDashController;

    public override void Initialize()
    {
        playerDashController = player.DashController;
        player.OnKillEnemy += Proc;
        base.Initialize();
    }

    public override void Proc()
    {
        print("Procced DashResetOnKillItem");
        playerDashController.RefreshCooldown();
        base.Proc();
    }

    public override void Deactivate()
    {
        player.OnKillEnemy -= Proc;
        base.Deactivate();
    }

}
