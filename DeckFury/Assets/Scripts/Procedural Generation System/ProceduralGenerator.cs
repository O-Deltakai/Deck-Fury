using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField] MapPoolSO _mapLayoutPool;
    public MapPoolSO MapLayoutPool => _mapLayoutPool;

    int savedSeed;
    System.Random random;

    [SerializeField] ZoneBlueprint zoneBlueprint;

    void Start()
    {
        InitializeZone();
    }


    void GenerateSeed()
    {
        savedSeed = Random.Range(100000, 999999);
    }

    void InitializeZone()
    {
        GenerateSeed();
        random = new System.Random(savedSeed);

        zoneBlueprint = new ZoneBlueprint
        {
            mapLayoutPool = MapLayoutPool
        };
        zoneBlueprint.GenerateZone(random);

    }




}
