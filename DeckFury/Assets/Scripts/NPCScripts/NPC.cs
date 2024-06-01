using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : StageEntity
{

    public delegate void NPCDefeatEventHandler(AttackPayload? killingBlow, NPC npc);
    public event NPCDefeatEventHandler OnNPCDefeat;

    [SerializeField] EnemyDataSO _enemyData;
    public EnemyDataSO EnemyData => _enemyData;

    public PlayerController player;
    [Range(1, 8)]
    [SerializeField, Min(0)] int enemyTier = 1;
    public int EnemyTier{get{return enemyTier;}}

    public int spawnPointCost {get { return _enemyData.EnemyTier * 100; }}

    [SerializeField] protected List<AbilityData> NPCAbilities = new List<AbilityData>();

    [field:SerializeField] public Vector2 DistanceFromPlayer{get; private set;}

    protected void InitializeNPCAwakeVariables()
    {
        if(_enemyData)
        {
            AssignData(_enemyData);
        }
    }

    protected void InitializeNPCStartVariables()
    {
        player = GameErrorHandler.NullCheck(GameManager.Instance.player, "Player Controller");

    }

    protected override void Awake()
    {
        InitializeNPCAwakeVariables();
        base.Awake();
    }

    protected override void Start()
    {
        InitializeNPCStartVariables();
        base.Start();

    }

    void AssignData(EnemyDataSO data)
    {
        foreach(AttackElement weakness in data.Weaknesses)
        {
            weaknesses.Add(weakness);
        }
        foreach(AttackElement resist in data.Resistances)
        {
            resistances.Add(resist);
        }

        CurrentHP = _enemyData.MaxHP;
        ShieldHP = _enemyData.ShieldHP;
        Armor = _enemyData.Armor;
        Defense = _enemyData.Defense;
        enemyTier = _enemyData.EnemyTier;

    }

    protected override void AdditionalOnHurtEvents(AttackPayload? payload = null)
    {
        base.AdditionalOnHurtEvents(payload);
        if(payload.HasValue)
        {
            EventBus<NPCDamagedEvent>.Raise(new NPCDamagedEvent{damageTaken = payload.Value.damage, npc = this, attackPayload = payload.Value});

            if(payload.Value.attacker != null)
            {
                if(payload.Value.attacker.GetComponent<PlayerController>())
                {
                    PlayerController player = payload.Value.attacker.GetComponent<PlayerController>();
                    player.HurtEnemyTrigger(this, payload);
                }
            }        
        }
    }



    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow = null)
    {
        base.AdditionalDestructionEvents(killingBlow);
        OnNPCDefeat?.Invoke(killingBlow, this);

        if(killingBlow.HasValue && killingBlow.Value.attacker != null)
        {
            //print("Killing blow has value, attacker: " + killingBlow.Value.attacker.name);
            if(killingBlow.Value.attacker.CompareTag(TagNames.Player.ToString()))
            {
                player.KillEnemyTrigger(this, killingBlow);
            }
        }

        EventBus<NPCKilledEvent>.Raise(new NPCKilledEvent {killingBlow = killingBlow.Value, npc = this, positionAtDeath = worldTransform.position});

    }



}
