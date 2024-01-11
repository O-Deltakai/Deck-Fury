using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorValues : MonoBehaviour
{
    //Note: Min values are inclusive and max values are exclusive

    public const int MAX_LEVELS_PER_ZONE = 7;
    public const int MIN_LEVELS_PER_ZONE = 6; 


    public const int MAX_STAGES_PER_LEVEL = 5; 
    public const int MIN_STAGES_PER_LEVEL = 2;


    public const int MAX_ELITES_PER_ZONE = 3;
    public const int MIN_ELITES_PER_ZONE = 1;

    public const int MAX_SHOPS_PER_ZONE = 2;
    public const int MIN_SHOPS_PER_ZONE = 1;

    public const int MAX_RESTS_PER_ZONE = 2;
    public const int MIN_RESTS_PER_ZONE = 1;

    public const int MAX_MYSTERIES_PER_ZONE = 5;
    public const int MIN_MYSTERIES_PER_ZONE = 2;


[Header("Levels Per Zone")]
    [SerializeField] int _maxLevelsPerZone;
    [SerializeField] int _minLevelsPerZone;

[Header("Stages Per Level")]
    [SerializeField] int _maxStagesPerLevel;
    [SerializeField] int _minStagesPerLevel;

[Header("Elites Per Zone")]
    [SerializeField] int _maxElitesPerZone;
    [SerializeField] int _minElitesPerZone;

[Header("Shops Per Zone")]
    [SerializeField] int _maxShopsPerZone;
    [SerializeField] int _minShopsPerZone;

[Header("Rests Per Zone")]
    [SerializeField] int _maxRestsPerZone;
    [SerializeField] int _minRestsPerZone;



}
