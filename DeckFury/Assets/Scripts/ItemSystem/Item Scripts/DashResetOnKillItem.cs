using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashResetOnKillItem : ItemBase
{
    PlayerDashController playerDashController;

    public override void Initialize()
    {
        base.Initialize();
        playerDashController = player.DashController;
        player.OnKillEnemy += Proc;
    }

    public override void Proc()
    {
        base.Proc();
        playerDashController.RefreshCooldown();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        player.OnKillEnemy -= Proc;
    }

}
