using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalResourceManager : MonoBehaviour
{
    static GlobalResourceManager _instance;
    public GlobalResourceManager Instance {get { return _instance; }}



    [SerializeReference] ScoreRewardTableSO scoreRewardTable;
    private static List<RuntimeBonusScoreItem> runtimeBonusScoreItems = new List<RuntimeBonusScoreItem>();
    public static List<RuntimeBonusScoreItem> BonusScoreItems{ get{ return runtimeBonusScoreItems; }}
    [SerializeField] List<RuntimeBonusScoreItem> bonusScoreItems = new List<RuntimeBonusScoreItem>();


    [SerializeReference] MapPoolSO _mapPool;
    private static MapPoolSO gameMapPool;
    public static MapPoolSO GameMapPool { get { return gameMapPool; }}


    private void Awake() 
    {

    }

    void Start()
    {
        InitializeBonusScoreItems();
        InitializeMapPool();
        _instance = this;
    }



    void InitializeMapPool()
    {
        gameMapPool = _mapPool;
    }


    void InitializeBonusScoreItems()
    {
        runtimeBonusScoreItems.Clear();
        if(!scoreRewardTable)
        {
            Debug.LogError("Score reward table was not set, cannot set runtime bonus score items.");
            return;
        }

        foreach(BonusScoreItemSO bonusScoreItem in scoreRewardTable.BonusRewardTable)
        {
            RuntimeBonusScoreItem scoreItem = new RuntimeBonusScoreItem(bonusScoreItem);
            
            runtimeBonusScoreItems.Add(scoreItem);
        }

        bonusScoreItems = runtimeBonusScoreItems;

    }


}
