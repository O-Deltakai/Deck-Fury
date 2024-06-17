using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnergyOnKill : ItemBase
{
    EventBinding<NPCKilledEvent> npcKilledEventBinding;

    void OnEnable()
    {   //Register a new event binding for the EventBus specific to NPCKilledEvent. Whenever an NPC is killed, or however the NPCKilledEvent is raised,
        //the HandleEventData method will be called.
        npcKilledEventBinding = new EventBinding<NPCKilledEvent>(HandleEventData);
        EventBus<NPCKilledEvent>.Register(npcKilledEventBinding);
    }

    void OnDisable()
    {
        EventBus<NPCKilledEvent>.Deregister(npcKilledEventBinding);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Proc()
    {
        EnergyController.Instance.CurrentEnergyValue +=  EnergyController.Instance.MaxEnergy * itemSO.QuantifiableEffects[0].FloatQuantity * 0.01f ;
        base.Proc();
    }

    void HandleEventData(NPCKilledEvent npcKilledEvent)
    {
        if(!Initialized) {return;}

        if(npcKilledEvent.killingBlow.attacker = player.gameObject)
        {
            Proc();
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }


}
