using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField] MapPoolSO _mapLayoutPool;
    public MapPoolSO MapLayoutPool => _mapLayoutPool;

    [SerializeField] GameObject stageMap;

    [SerializeField] int savedSeed;
    System.Random random;

[Header("Prefabs")]
    [SerializeField] GameObject mapStagePrefab;
    [SerializeField] GameObject mapLevelPrefab;

    [SerializeField] Vector2 levelDimensions;


    [SerializeField] bool testGeneration = false;
    [SerializeField] bool generateUsingGivenSeed = false;

    [SerializeReference] ZoneBlueprint zoneBlueprint;



    void Start()
    {
        if(testGeneration)
        {
            InitializeZoneBlueprint();
            GenerateStageMap(zoneBlueprint);
            
        }

    }


    void GenerateSeed()
    {
        savedSeed = Random.Range(100000, 999999);
    }

    void InitializeZoneBlueprint()
    {
        if(!generateUsingGivenSeed)
        {
            GenerateSeed();
        }
        random = new System.Random(savedSeed);

        zoneBlueprint = new ZoneBlueprint
        {
            mapLayoutPool = MapLayoutPool
        };
        zoneBlueprint.GenerateZone(random);

    }

    void GenerateStageMap(ZoneBlueprint blueprint)
    {
        foreach(var levelBlueprint in blueprint.LevelBlueprints)
        {
            MapLevel mapLevel = Instantiate(mapLevelPrefab, stageMap.transform).GetComponent<MapLevel>();
            //mapLevel.GetComponent<RectTransform>().sizeDelta = levelDimensions;
            
            foreach(var stageBlueprint in levelBlueprint.StageBlueprints)
            {
                MapStage mapStage = Instantiate(mapStagePrefab, mapLevel.transform).GetComponent<MapStage>();

                mapStage.mapLayoutPrefab = stageBlueprint.MapLayoutPrefab;
                mapStage.TypeOfStage = stageBlueprint.stageType;
                mapStage.sceneToLoadName = stageBlueprint.sceneToLoad;

                mapStage.spawnTable = stageBlueprint.spawnTable;
            }

        }


    }


}
