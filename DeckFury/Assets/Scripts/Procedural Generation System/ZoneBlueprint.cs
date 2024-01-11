using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
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

    public MapPoolSO mapLayoutPool;
    public List<MapLayoutController> allMaps;

    [SerializeField] List<LevelBlueprint> _levelBlueprints = new List<LevelBlueprint>();
    public IReadOnlyList<LevelBlueprint> LevelBlueprints => _levelBlueprints;

    [SerializeField] List<int> stageDistribution;

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

    List<StageType> _currentStageTypes = new List<StageType>();
    public List<StageType> CurrentStageTypes { get => _currentStageTypes; }



    public void GenerateZone(System.Random random)
    {
        int minTotalStagesInZone = GeneratorValues.MIN_LEVELS_PER_ZONE * GeneratorValues.MIN_STAGES_PER_LEVEL;
        int maxTotalStagesInZone = GeneratorValues.MAX_LEVELS_PER_ZONE * GeneratorValues.MAX_STAGES_PER_LEVEL - 1;

        //Placeholder for now - zone and levels just generate using all available maps from the map pool with no regard to difficulty
        allMaps = mapLayoutPool.GetAllMaps();


        //Randomise expected values
        NumberOfLevels = random.Next(GeneratorValues.MIN_LEVELS_PER_ZONE, GeneratorValues.MAX_LEVELS_PER_ZONE);
        //_totalNumberOfStages = GenerateAverageWeightedRandomInt(random, minTotalStagesInZone, maxTotalStagesInZone, 30);
        _totalNumberOfStages = GenerateSkewedWeightedRandom(random, minTotalStagesInZone, maxTotalStagesInZone, 17, 23, 20, 0.5);
        _numberOfStages = _totalNumberOfStages;

        //SimulateWeightingAlgorithm(random, minTotalStagesInZone, maxTotalStagesInZone, 20);


        AllocateMandatoryStageTypes();
        RandomlyDistributeStageTypes(random);


        List<int> stagesPerLevel = DistributeStagesAcrossLevels(random);


        for (int i = 0; i < NumberOfLevels; i++)
        {
            LevelBlueprint level = new LevelBlueprint();
            SetLevelDetails(level, random);

            level.GenerateLevel(random, this, stagesPerLevel[i]);



            _levelBlueprints.Add(level);
        }

    }

/// <summary>
/// Returns a randomly distributed list of ints that respect minimum and maximum values which represent the number of stages per level
/// </summary>
/// <param name="random"></param>
/// <returns></returns>
    List<int> DistributeStagesAcrossLevels(System.Random random) 
    {
        List<int> stagesDistribution = new List<int>();
        int totalAssignedStages = 0;

        //1: Assign the min number of stages to each level
        for(int i = 0; i< NumberOfLevels; i++)
        {
            stagesDistribution.Add(GeneratorValues.MIN_STAGES_PER_LEVEL);
            totalAssignedStages += GeneratorValues.MIN_STAGES_PER_LEVEL;
        }

        //2: Distribute remaining stages
        int remainingStages = _totalNumberOfStages - totalAssignedStages;

        int retryCap = 300;

        while(remainingStages > 0 && retryCap > 0)
        {
            int levelIndex = random.Next(0, NumberOfLevels);
            if(stagesDistribution[levelIndex] < GeneratorValues.MAX_STAGES_PER_LEVEL)
            {
                stagesDistribution[levelIndex]++;
                remainingStages--;
            }

            retryCap--;
        }

        if(retryCap <= 0)
        {
            Debug.LogError("Retry cap for distribution reached, something has gone wrong.");
        }

        stagesDistribution = ShuffleIntList(stagesDistribution, random);
        stageDistribution = stagesDistribution;

        return stagesDistribution;
    }


    void AllocateMandatoryStageTypes()
    {
        for(int i = 0; i < GeneratorValues.MIN_ELITES_PER_ZONE; i++)
        {
            _currentStageTypes.Add(StageType.EliteCombat);
        }

        for(int i = 0; i < GeneratorValues.MIN_SHOPS_PER_ZONE; i++)
        {
            _currentStageTypes.Add(StageType.Shop);
        }

        for(int i = 0; i < GeneratorValues.MIN_RESTS_PER_ZONE; i++)
        {
            _currentStageTypes.Add(StageType.RestPoint);
        }

        for(int i = 0; i < GeneratorValues.MIN_MYSTERIES_PER_ZONE; i++)
        {
            _currentStageTypes.Add(StageType.Mystery);
        }

    }

    void RandomlyDistributeStageTypes(System.Random random)
    {
        int remainingStages = _totalNumberOfStages - CurrentStageTypes.Count;

        for (int i = 0; i < remainingStages; i++) 
        {
            StageType stageType = GetRandomStageType(random, _currentStageTypes);
            _currentStageTypes.Add(stageType);
        }

        ShuffleStageTypes(random, _currentStageTypes);
    }


    StageType GetRandomStageType(System.Random random, List<StageType> currentStages)
    {
        List<StageType> possibleTypes = new List<StageType> { StageType.Combat };
        if (currentStages.Count(s => s == StageType.EliteCombat) < GeneratorValues.MAX_ELITES_PER_ZONE) possibleTypes.Add(StageType.EliteCombat);
        if (currentStages.Count(s => s == StageType.Shop) < GeneratorValues.MAX_SHOPS_PER_ZONE) possibleTypes.Add(StageType.Shop);
        if (currentStages.Count(s => s == StageType.RestPoint) < GeneratorValues.MAX_RESTS_PER_ZONE) possibleTypes.Add(StageType.RestPoint);
        if (currentStages.Count(s => s == StageType.Mystery) < GeneratorValues.MAX_MYSTERIES_PER_ZONE) possibleTypes.Add(StageType.Mystery);


        return possibleTypes[random.Next(0, possibleTypes.Count)];        
    }

    List<int> ShuffleIntList(List<int> list, System.Random random)
    {
        for (int i = 0; i < list.Count; i++) 
        {
            int swapIndex = random.Next(i, list.Count);
            int temp = list[i];

            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }

        return list;
    }


    List<StageType> ShuffleStageTypes(System.Random random, List<StageType> list)
    {
        for (int i = 0; i < list.Count; i++) 
        {
            int swapIndex = random.Next(i, list.Count);
            StageType temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }

        return list;
    }

    void SetLevelDetails(LevelBlueprint level, System.Random random)
    {
        level.mapPool = mapLayoutPool;
        level.parentZone = this;

    }

/// <summary>
/// Generates a weighted random value which is skewed towards the average value of the given min and max value.
/// skewStrength dictates how strongly the returned value will be skewed towards the average, with higher skewStrength
/// giving a greater chance of returning an average value.
/// </summary>
/// <param name="random"></param>
/// <param name="min"></param>
/// <param name="max"></param>
/// <param name="skewStrength"></param>
/// <returns></returns>
    int GenerateAverageWeightedRandomInt(System.Random random, int min, int max, int skewStrength)
    {
        int total = 0;
        for (int i = 0; i < skewStrength; i++) {
            total += random.Next(min, max + 1);
        }
        return total / skewStrength;
    }

/// <summary>
/// Test method that simulates the weighting algorithm and the probability of getting a value within a range given a number of samples
/// for the GenerateAverageWeightedRandomInt method.
/// </summary>
/// <param name="random"></param>
/// <param name="min"></param>
/// <param name="max"></param>
/// <param name="numberOfSamples"></param>
    void SimulateWeightingAlgorithm(System.Random random, int min, int max, int numberOfSamples)
    {
        int simulations = 100000; // Number of simulations
        int countInRange = 0;
        int desiredMin = 18; // Your desired range minimum
        int desiredMax = 23; // Your desired range maximum

        for (int i = 0; i < simulations; i++) 
        {
            int result = GenerateAverageWeightedRandomInt(random, min, max, numberOfSamples);
            if (result >= desiredMin && result <= desiredMax) 
            {
                countInRange++;
            }
        }

        double probability = (double)countInRange / simulations;
        Debug.Log($"Probability of getting a value in the range [{desiredMin}, {desiredMax}] with number of samples [{numberOfSamples}]: {probability}");        
    }

/// <summary>
/// Generates a random int that is skewed towards the given range of skewMin and skewMax. numberOfSamples dictates how smooth the distribution is
/// and skewProportion (0 - 1) dictates how strong the skew effect is, with higher proportion meaning more numbers will come from the skew range.
/// </summary>
/// <param name="random"></param>
/// <param name="min"></param>
/// <param name="max"></param>
/// <param name="skewMin"></param>
/// <param name="skewMax"></param>
/// <param name="numberOfSamples"></param>
/// <param name="skewProportion"></param>
/// <returns></returns>
    int GenerateSkewedWeightedRandom(System.Random random, int min, int max, int skewMin, int skewMax, int numberOfSamples, double skewProportion) 
    {
        if (skewProportion < 0) skewProportion = 0;
        else if (skewProportion > 1) skewProportion = 1;

        int total = 0;
        int skewedSamples = (int)(numberOfSamples * skewProportion);
        int normalSamples = numberOfSamples - skewedSamples;

        // Generate skewed samples
        for (int i = 0; i < skewedSamples; i++) 
        {
            total += random.Next(skewMin, skewMax + 1);
        }

        // Generate normal samples
        for (int i = 0; i < normalSamples; i++) 
        {
            total += random.Next(min, max + 1);
        }

        return total / numberOfSamples;
    }


}
