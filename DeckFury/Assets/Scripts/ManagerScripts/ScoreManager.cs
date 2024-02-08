using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages tracking and calculation of score rewards as well as monetary rewards
/// </summary>
[RequireComponent(typeof(RewardConditionChecks))]
public class ScoreManager : MonoBehaviour
{

    private static ScoreManager _instance;
    public static ScoreManager Instance {get{ return _instance; }}


    public delegate void CalculatedFinalScoreEventHandler();
    public event CalculatedFinalScoreEventHandler OnCalculatedFinalScore;

    public delegate void CheckedBonusRewardsEvent(List<BonusScoreItemSO> bonusScoreItems);
    public event CheckedBonusRewardsEvent OnCheckedSpecialBonuses;

    [SerializeField] bool UseScoreManager = false;
    [field:SerializeField] public int StageScoreTotal{get; private set;}

    PlayerController player;
    PlayerCardManager playerCardManager;
    PersistentLevelController levelController;

    public bool StopRecordingEvents = false;

[Header("Base Score Rewards")]
    [SerializeField] int killEnemyReward = 100; // base score reward for killing an enemy, is modified by the enemy's tier (higher tier enemies give more score)
    [SerializeField] int noHPLostReward = 500; // reward for if the player doesn't take any HP damage (shieldHP damage doesn't count)

    [SerializeField] int doubleKillReward = 50;
    [SerializeField] int tripleKillReward = 100;
    [SerializeField] int quadraKillReward = 200;
    [SerializeField] int quadraPlusKillReward = 500;




    [SerializeField] int parTimeBaseScore = 500; // regular reward for if the player completes the stage within the stageParTime
    [SerializeField] float stageParTime = 60; // Expected amount of time to complete the stage

[Header("Money Reward Multipliers")]
    [SerializeField, Min(0)] float _baseMoneyMultiplier = 0.015f;


    SpawnManager spawnManager;
    RewardConditionChecks rewardConditionChecks;

[Header("Player Event Counters")]
    [SerializeField] int numberOfMoves = 0;
    public int NumberOfMoves{get{return numberOfMoves;}}

    //Generic Scores
    [field:SerializeField] public int HPDamageTaken {get; private set;} = 0;
    [field:SerializeField] public int TotalEnemiesKilled {get; private set;} = 0;
    [field:SerializeField] public float TimeSpentOnStage {get; private set;} = 0;

    [field:SerializeField] public int NumberOfCardsUsed {get; private set;} = 0;
    [field:SerializeField] public int NumberOfBasicAttacks {get; private set;} = 0;

    [field:SerializeField] public int DoubleKills {get; private set;} = 0;
    [field:SerializeField] public int TripleKills {get; private set;} = 0;
    [field:SerializeField] public int QuadraKills {get; private set;} = 0;
    [field:SerializeField] public int QuadraPlusKills {get; private set;} = 0;

    [field:SerializeField] public int HighestComboKill {get; private set;} = 0;
    [field:SerializeField] public int NumberOfComboKills {get; private set;} = 0;
    int currentComboKillLength = 0;
    Coroutine CR_ComboTimer = null;

    [field:SerializeField] public int EnemiesKilledWithHazards {get; private set;} = 0;
    [field:SerializeField] public int DamageDealtToEnemies {get; private set;} = 0;


    [field:SerializeField] public int TotalDamageTaken {get; private set;} = 0;


    public int EnemiesKilledScore { get; private set; } = 0;


    [SerializeField] ScoreRewardTableSO bonusRewardTable;
    [field:SerializeField] public List<BonusScoreItemSO> AppliedBonusRewards { get; private set; } = new List<BonusScoreItemSO>();

    EventBinding<NPCKilledEvent> npcKilledEventBinding;
    EventBinding<NPCDamagedEvent> npcDamagedEventBinding;

    int playerStartingHP;

    private void Awake() 
    {
        _instance = this;

        spawnManager = FindObjectOfType<SpawnManager>();

        if(spawnManager)
        {
            //spawnManager.OnSpawnNewWave += SubscribeEnemiesToEvents;
            spawnManager.OnAllWavesCleared += CalculateFinalStageScore;
            spawnManager.OnAllWavesCleared += UnsubscribePlayerToEvents;
        }else
        {
            print("Spawnmanager is missing or has been disabled. ScoreManager will not function.");
        }



        rewardConditionChecks = GetComponent<RewardConditionChecks>();
        rewardConditionChecks.OnBonusConditionMet += ApplyBonusReward;

        InitializeBonusScoreConditions(GlobalResourceManager.BonusScoreItems);
    }

    private void OnDestroy() 
    {
        _instance = null;    
    }

    void Start()
    {
        if(!UseScoreManager){return;}

        InitializeStartingVariables();



    }

    void OnEnable()
    {
        npcKilledEventBinding = new EventBinding<NPCKilledEvent>(HandleNPCKilledEventData);
        EventBus<NPCKilledEvent>.Register(npcKilledEventBinding);

        npcDamagedEventBinding = new EventBinding<NPCDamagedEvent>(HandleNPCDamagedEventData);
        EventBus<NPCDamagedEvent>.Register(npcDamagedEventBinding);

    }

    void OnDisable()
    {
        EventBus<NPCKilledEvent>.Deregister(npcKilledEventBinding);
        EventBus<NPCDamagedEvent>.Deregister(npcDamagedEventBinding);
    }

    private void Update() 
    {
        if(StopRecordingEvents){ return; }



        TimeSpentOnStage += Time.deltaTime;    
    }


    void InitializeStartingVariables()
    {


        //levelController = GameErrorHandler.NullCheck(PersistentLevelController.Instance, "PersistenLevelController");


        player = GameErrorHandler.NullCheck(GameManager.Instance.player, "PlayerController");
        playerCardManager = player.GetComponent<PlayerCardManager>();
        playerStartingHP = player.CurrentHP;
        SubscribePlayerToEvents();
    }

    void InitializeBonusScoreConditions(List<RuntimeBonusScoreItem> runtimeBonusScoreItems)
    {
        print("Attempting to initialize bonus score conditions");
        foreach (var runtimeBonusScoreItem in runtimeBonusScoreItems)
        {
            // Clear existing listeners to avoid duplicates
            runtimeBonusScoreItem.RewardCondition?.RemoveAllListeners();

            // Use reflection to get the method from its name
            MethodInfo methodToAssign = typeof(RewardConditionChecks).GetMethod(runtimeBonusScoreItem.GetConditionMethodName());
            if (methodToAssign != null)
            {
                // Create a delegate from the method and add it as a listener
                UnityAction<BonusScoreItemSO> action = (UnityAction<BonusScoreItemSO>)Delegate.CreateDelegate(typeof(UnityAction<BonusScoreItemSO>), rewardConditionChecks, methodToAssign);
                runtimeBonusScoreItem.RewardCondition.AddListener(action);
            }
            else
            {
                Debug.LogWarning($"Method {runtimeBonusScoreItem.GetConditionMethodName()} not found in RewardConditionChecks!");
            }
        }        
    }

    void SubscribePlayerToEvents()
    {
        player.OnTweenMove += IncrementMoveCounter;
        player.OnBasicAttack += IncrementBasicAttackCounter;
        player.OnDamageTaken += IncrementDamageTaken;
        playerCardManager.OnRemoveCard += IncrementCardsUsedCounter;
    }
    void UnsubscribePlayerToEvents()
    {
        player.OnTweenMove -= IncrementMoveCounter;
        player.OnBasicAttack -= IncrementBasicAttackCounter;
        player.OnDamageTaken -= IncrementDamageTaken;
        playerCardManager.OnRemoveCard -= IncrementCardsUsedCounter;
        StopRecordingEvents = true;        
    }

    void SubscribeEnemiesToEvents(List<StageEntity> entities)
    {
        print("Subscribing enemies from spawn manager to score manager, number of entities: " + entities.Count);
        foreach(NPC entity in entities.Cast<NPC>())
        {
            entity.OnNPCDefeat += EnemyDeathScoreAdd;
            entity.OnNPCDefeat += EnemyKilled;
            entity.OnDamageTaken += IncrementDamageDealtToEnemies;
        }

    }

    void HandleNPCKilledEventData(NPCKilledEvent data)
    {
        EnemyDeathScoreAdd(data.killingBlow, data.npc);
        EnemyKilled(data.killingBlow, data.npc);
    }

    void HandleNPCDamagedEventData(NPCDamagedEvent data)
    {
        IncrementDamageDealtToEnemies(data.damageTaken);
    }


    void EnemyDeathScoreAdd(AttackPayload? killingBlow, NPC destroyedNPC)
    {
        int adjustedScoreReward = 0;
        adjustedScoreReward += destroyedNPC.EnemyTier * killEnemyReward;

        EnemiesKilledScore += adjustedScoreReward;

        if(CR_ComboTimer != null)
        {
            StopCoroutine(CR_ComboTimer);
        }
        CR_ComboTimer = StartCoroutine(MultiKillTimer(0.15f));

        currentComboKillLength++;


    }

    IEnumerator MultiKillTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        CR_ComboTimer = null;

        if(HighestComboKill < currentComboKillLength)
        {
            HighestComboKill = currentComboKillLength;
        }

        if(currentComboKillLength > 1)
        {
            NumberOfComboKills++;
        }

        if(currentComboKillLength == 2)
        {
            DoubleKills++;
        }else
        if(currentComboKillLength == 3)
        {
            TripleKills++;
        }else
        if(currentComboKillLength == 4)
        {
            QuadraKills++;
        }else
        if(currentComboKillLength > 4)
        {
            QuadraPlusKills++;
        }

        currentComboKillLength = 0;
    }

    void IncrementMoveCounter(Vector3Int startPosition, Vector3Int destination)
    {
        numberOfMoves++;
    }
    void IncrementCardsUsedCounter()
    {
        NumberOfCardsUsed++;
    }
    void IncrementBasicAttackCounter()
    {
        NumberOfBasicAttacks++;
    }
    void IncrementDamageTaken(int damageTaken)
    {
        TotalDamageTaken += damageTaken;
    }

    void IncrementDamageDealtToEnemies(int damageDealt)
    {
        DamageDealtToEnemies += damageDealt;
    }
    void EnemyKilled(AttackPayload? killingBlow, NPC destroyedNPC)
    {
        if(killingBlow.HasValue)
        {
            if(killingBlow.Value.attacker)
            {
                if(killingBlow.Value.attacker.CompareTag(Tags.ENVIRONMENT_HAZARD))
                {
                    TotalEnemiesKilled++;
                    EnemiesKilledWithHazards++;
                    return;
                }
            }
            
        }
        TotalEnemiesKilled++;
    }


    public int CalculateHPDamageTakenScore(int startingHP, int endingHP)
    {
        if(startingHP == endingHP)
        {
            return noHPLostReward;
        }

        HPDamageTaken = startingHP - endingHP;

        float percentageRemaining = (float)endingHP / startingHP;
        print("precentage HP remaining: " + percentageRemaining);
        float hpScore = noHPLostReward * 0.5f * percentageRemaining;
        print(hpScore);
        return (int)hpScore;
    }
    public int CalculateHPDamageTakenScore()
    {
        if(playerStartingHP == player.CurrentHP)
        {
            return noHPLostReward;
        }

        HPDamageTaken = playerStartingHP - player.CurrentHP;

        float percentageRemaining = (float)player.CurrentHP / playerStartingHP;
        print("precentage HP remaining: " + percentageRemaining);
        float hpScore = noHPLostReward * 0.5f * percentageRemaining;
        print(hpScore);
        return (int)hpScore;
    }

    public int CalculateTimeTakenScore(float stageCompletedTime)
    {
        int finalScore;

        if (stageCompletedTime < stageParTime)
        {
            // Player completed faster than parTime
            // Use exponential scaling for bonus points
            float timeRatio = stageCompletedTime / stageParTime;
            float bonusMultiplier = Mathf.Lerp(2f, 1f, Mathf.Sqrt(timeRatio));
            finalScore = Mathf.RoundToInt(parTimeBaseScore * bonusMultiplier);
        }
        else if (stageCompletedTime <= 2 * stageParTime)
        {
            // Player took up to twice the parTime
            // Use linear scaling to decrease points
            float timeOverPar = (stageCompletedTime - stageParTime) / stageParTime;
            float penaltyMultiplier = Mathf.Lerp(1f, 0.25f, timeOverPar);
            finalScore = Mathf.RoundToInt(parTimeBaseScore * penaltyMultiplier);
        }
        else
        {
            // Player took more than twice the parTime
            // Set score to minimum 0.25x base score reward
            finalScore = Mathf.RoundToInt(parTimeBaseScore * 0.25f);
        }

        return finalScore;
    }



    public int AddScore(int scoreReward)
    {
        StageScoreTotal += scoreReward;

        return StageScoreTotal;
    }

    void CheckApplicableBonusRewards(List<RuntimeBonusScoreItem> bonusScoreItems)
    {
        foreach(RuntimeBonusScoreItem bonusReward in bonusScoreItems)
        {
            bonusReward.RewardCondition?.Invoke(bonusReward.ConcreteBonusScoreItemReference);
        }

        OnCheckedSpecialBonuses?.Invoke(AppliedBonusRewards);
        
    }

    void ApplyBonusReward(BonusScoreItemSO bonusScoreItem)
    {
        AppliedBonusRewards.Add(bonusScoreItem);
    }

    void CalculateFinalStageScore()
    {
        AddScore(CalculateHPDamageTakenScore(playerStartingHP, player.CurrentHP));
        AddScore(CalculateTimeTakenScore(TimeSpentOnStage));
        AddScore(EnemiesKilledScore);
        CheckApplicableBonusRewards(GlobalResourceManager.BonusScoreItems);
        OnCalculatedFinalScore?.Invoke();
    }

    public int CalculateMoneyEarned(int score)
    {
        if(StageStateController.Instance._stageType == StageType.EliteCombat)
        {
            return (int)(score * _baseMoneyMultiplier * 2f);
        }

        return (int)(score * _baseMoneyMultiplier);
    }

    public void AddBonusRewardsToScore()
    {
        foreach(var bonusReward in AppliedBonusRewards)
        {
            AddScore(bonusReward.BaseScore);
        }
    }

    public void AddScoreToPlayerData(PlayerDataContainer playerData)
    {
        playerData.CurrentScore += StageScoreTotal;
    }


}
