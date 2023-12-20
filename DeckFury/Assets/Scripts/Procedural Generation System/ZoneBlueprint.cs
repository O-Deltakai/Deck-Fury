using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneBlueprint
{
    int _zoneTier;
    public int ZoneTier => _zoneTier;

    int _numberOfLevels = 6;
    public int NumberOfLevels {get { return _numberOfLevels; }
        set
        {
            if(value < GeneratorValues.MIN_LEVELS_PER_ZONE)
            {
                Debug.LogWarning("Number of levels set below minimum value, set actual value to minimum value of " + GeneratorValues.MIN_LEVELS_PER_ZONE);
                _numberOfLevels = GeneratorValues.MIN_LEVELS_PER_ZONE;
            }else
            if(value > GeneratorValues.MAX_LEVELS_PER_ZONE)
            {
                Debug.LogWarning("Number of levels set above max value, set actual value to max value of " + GeneratorValues.MAX_LEVELS_PER_ZONE);
                _numberOfLevels = GeneratorValues.MAX_LEVELS_PER_ZONE;
            }else
            {
                _numberOfLevels = value;
            }

        }
    }

    public int _numberOfStages;


    List<LevelBlueprint> _levelBlueprints;
    public IReadOnlyList<LevelBlueprint> LevelBlueprints => _levelBlueprints;


    //Expected zone values
    int _expectedNumberOfCombatStages;
    int _expectedNumberOfEliteStages;
    int _expectedNumberOfShops;
    int _expectedNumberOfRests;

    public int ExpectedNumberOfCombatStages { get => _expectedNumberOfCombatStages; private set => _expectedNumberOfCombatStages = value; }
    public int ExpectedNumberOfEliteStages { get => _expectedNumberOfEliteStages; private set => _expectedNumberOfEliteStages = value; }
    public int ExpectedNumberOfShops { get => _expectedNumberOfShops; private set => _expectedNumberOfShops = value; }
    public int ExpectedNumberOfRests { get => _expectedNumberOfRests; private set => _expectedNumberOfRests = value; }


    //Actual zone values
    int _totalNumberOfLevels;
    int _totalNumberOfStages;



    int _totalNumberOfCombatStages;
    int _totalNumberOfEliteStages;
    int _totalNumberOfShops;
    int _totalNumberOfRests;

    public int TotalNumberOfLevels { get => _totalNumberOfLevels; set => _totalNumberOfLevels = value; }
    public int TotalNumberOfStages { get => _totalNumberOfStages; set => _totalNumberOfStages = value; }
    public int TotalNumberOfCombatStages { get => _totalNumberOfCombatStages; set => _totalNumberOfCombatStages = value; }
    public int TotalNumberOfEliteStages { get => _totalNumberOfEliteStages; set => _totalNumberOfEliteStages = value; }
    public int TotalNumberOfShops { get => _totalNumberOfShops; set => _totalNumberOfShops = value; }
    public int TotalNumberOfRests { get => _totalNumberOfRests; set => _totalNumberOfRests = value; }

    List<StageType> _mandatoryStages = new List<StageType>();
    public IReadOnlyList<StageType> MandatoryStages { get => _mandatoryStages; }



    public void GenerateZone(System.Random random)
    {
        int minTotalStagesInZone = GeneratorValues.MIN_LEVELS_PER_ZONE * GeneratorValues.MIN_STAGES_PER_LEVEL;
        int maxTotalStagesInZone = GeneratorValues.MAX_LEVELS_PER_ZONE * GeneratorValues.MAX_STAGES_PER_LEVEL;


        //Randomise expected values
        NumberOfLevels = random.Next(GeneratorValues.MIN_LEVELS_PER_ZONE, GeneratorValues.MAX_LEVELS_PER_ZONE);
        _totalNumberOfStages = random.Next(minTotalStagesInZone, maxTotalStagesInZone + 1);

        List<int> stagesPerLevel = DistributeStagesAcrossLevels(random);


        ExpectedNumberOfEliteStages = random.Next(GeneratorValues.MIN_ELITES_PER_ZONE, GeneratorValues.MAX_ELITES_PER_ZONE);
        ExpectedNumberOfRests = random.Next(GeneratorValues.MIN_RESTS_PER_ZONE, GeneratorValues.MAX_RESTS_PER_ZONE);
        ExpectedNumberOfShops = random.Next(GeneratorValues.MIN_SHOPS_PER_ZONE, GeneratorValues.MAX_SHOPS_PER_ZONE);

        for (int i = 0; i < NumberOfLevels; i++)
        {
            LevelBlueprint level = new LevelBlueprint();
            SetLevelDetails(level, random);

            level.GenerateLevel(random, this, stagesPerLevel[i]);



            _levelBlueprints.Add(level);
        }

    }

    List<int> DistributeStagesAcrossLevels(System.Random random)
    {
        List<int> stagesDistribution = new List<int>();
        int remainingStages = _totalNumberOfStages;

        for (int i = 0; i < NumberOfLevels; i++)
        {
            if(i == NumberOfLevels - 1)
            {
                stagesDistribution.Add(remainingStages);
            }else
            {
                int maxStagesForThisLevel = Math.Min(GeneratorValues.MAX_STAGES_PER_LEVEL, remainingStages - (NumberOfLevels - i - 1) * GeneratorValues.MIN_STAGES_PER_LEVEL);
                int stagesInThisLevel = random.Next(GeneratorValues.MIN_STAGES_PER_LEVEL, maxStagesForThisLevel + 1);

                stagesDistribution.Add(stagesInThisLevel);
                remainingStages -= stagesInThisLevel;                
            }

        }

        return stagesDistribution;


    }


    void AllocateMandatoryStages()
    {
        for(int i = 0; i < GeneratorValues.MIN_ELITES_PER_ZONE; i++)
        {
            _mandatoryStages.Add(StageType.EliteCombat);
        }

        for(int i = 0; i < GeneratorValues.MIN_SHOPS_PER_ZONE; i++)
        {
            _mandatoryStages.Add(StageType.Shop);
        }

        for(int i = 0; i < GeneratorValues.MIN_RESTS_PER_ZONE; i++)
        {
            _mandatoryStages.Add(StageType.RestPoint);
        }

    }

    StageType GetRandomStageType(System.Random random, List<StageType> currentStages)
    {
        List<StageType> possibleTypes = new List<StageType> { StageType.Combat, StageType.Mystery };
        if (currentStages.Count(s => s == StageType.EliteCombat) < GeneratorValues.MAX_ELITES_PER_ZONE) possibleTypes.Add(StageType.EliteCombat);
        if (currentStages.Count(s => s == StageType.Shop) < GeneratorValues.MAX_SHOPS_PER_ZONE) possibleTypes.Add(StageType.Shop);
        if (currentStages.Count(s => s == StageType.RestPoint) < GeneratorValues.MAX_RESTS_PER_ZONE) possibleTypes.Add(StageType.RestPoint);

        return possibleTypes[random.Next(0, possibleTypes.Count)];        
    }



    void SetLevelDetails(LevelBlueprint level, System.Random random)
    {

    }



}
