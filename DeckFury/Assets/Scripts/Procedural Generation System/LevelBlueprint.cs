using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelBlueprint
{
    public int tier = 0;

    [SerializeReference] public ZoneBlueprint parentZone;


    int _numberOfStages = 3;
    public int NumberOfStages {get { return _numberOfStages; }
        set
        {
            if(value < GeneratorValues.MIN_STAGES_PER_LEVEL)
            {
                Debug.LogWarning("Number of stages set below minimum value, set actual value to minimum value of " + GeneratorValues.MIN_STAGES_PER_LEVEL);
                _numberOfStages = GeneratorValues.MIN_STAGES_PER_LEVEL;
            }else
            if(value > GeneratorValues.MAX_STAGES_PER_LEVEL + 1)
            {
                Debug.LogWarning("Number of stages: "  + value + " is above max value, set actual value to max value of " + GeneratorValues.MAX_STAGES_PER_LEVEL);
                _numberOfStages = GeneratorValues.MAX_STAGES_PER_LEVEL;
            }else
            {
                _numberOfStages = value;
            }

        }
    }

    public MapPoolSO mapPool;   


    [SerializeField] List<StageBlueprint> _stageBlueprints = new List<StageBlueprint>();
    public IReadOnlyList<StageBlueprint> StageBlueprints => _stageBlueprints;

    public void GenerateLevel(System.Random random, ZoneBlueprint zone, int stagesInLevel)
    {
        NumberOfStages = stagesInLevel;

        for (int i = 0; i < NumberOfStages; i++)
        {
            StageBlueprint stage = new StageBlueprint();
            SetStageDetails(stage, random);

            stage.GenerateStage(random, zone);

            _stageBlueprints.Add(stage);
        }


    }

    public void InitializeBlankLevel(ZoneBlueprint zone, int stagesInLevel)
    {
        NumberOfStages = stagesInLevel;
        parentZone = zone;

        for (int i = 0; i < stagesInLevel; i++)
        {
            StageBlueprint stage = new StageBlueprint();
            stage.parentLevel = this;
            _stageBlueprints.Add(stage);
        }


    }


    void SetStageDetails(StageBlueprint stage, System.Random random)
    {
        stage.MapLayoutPrefab = parentZone.allMaps[random.Next(0, parentZone.allMaps.Count)].gameObject;
        stage.parentLevel = this;
    }


}
