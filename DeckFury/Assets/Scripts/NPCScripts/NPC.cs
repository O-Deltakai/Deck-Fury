using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : StageEntity
{

    public delegate void NPCDefeatEventHandler(AttackPayload? killingBlow, NPC npc);
    public event NPCDefeatEventHandler OnNPCDefeat;

    [SerializeField] EnemyDataSO enemyData;


    public PlayerController player;
    [Range(1, 8)]
    [SerializeField] int enemyTier = 1;
    public int EnemyTier{get{return enemyTier;}}

    [SerializeField] protected List<AbilityData> NPCAbilities = new List<AbilityData>();

    [field:SerializeField] public Vector2 DistanceFromPlayer{get; private set;}

    protected void InitializeNPCAwakeVariables()
    {

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

    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow = null)
    {
        base.AdditionalDestructionEvents(killingBlow);
        OnNPCDefeat?.Invoke(killingBlow, this);
    }



}
