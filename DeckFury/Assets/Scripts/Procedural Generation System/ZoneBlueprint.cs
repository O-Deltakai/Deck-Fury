using System.Collections;
using System.Collections.Generic;
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

    List<LevelBlueprint> _levelBlueprints;
    public IReadOnlyList<LevelBlueprint> LevelBlueprints => _levelBlueprints;

    public void GenerateZone(System.Random random)
    {
        NumberOfLevels = random.Next(GeneratorValues.MIN_LEVELS_PER_ZONE, GeneratorValues.MAX_LEVELS_PER_ZONE);

        for (int i = 0; i < NumberOfLevels; i++)
        {
            LevelBlueprint level = new LevelBlueprint();
            SetLevelDetails(level, random);

            level.GenerateLevel(random);

            _levelBlueprints.Add(level);
        }

    }

    void SetLevelDetails(LevelBlueprint level, System.Random random)
    {

    }



}
