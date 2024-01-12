using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Spawn Table Pool", menuName = "New Spawn Table Pool", order = 0)]
public class SpawnTablePoolSO : ScriptableObject
{
    [SerializeField] List<SpawnTableSO> _regularSpawnTables = new List<SpawnTableSO>();
    public IReadOnlyList<SpawnTableSO> RegularSpawnTables => _regularSpawnTables;

    [SerializeField] List<SpawnTableSO> _eliteSpawnTables = new List<SpawnTableSO>();
    public IReadOnlyList<SpawnTableSO> EliteSpawnTables => _eliteSpawnTables;

    [SerializeField] List<SpawnTableSO> _bossSpawnTables = new List<SpawnTableSO>();
    public IReadOnlyList<SpawnTableSO> BossSpawnTables => _bossSpawnTables;

/// <summary>
/// Given a difficulty score range, return a randomly selected spawn table that is within that difficulty range.
/// Can be given a System.Random for seeded pseudorandom values.
/// </summary>
/// <param name="minDifficulty"></param>
/// <param name="maxDifficulty"></param>
/// <param name="random"></param>
/// <returns></returns>
    public SpawnTableSO GetSpawnTableByDifficulty(int minDifficulty, int maxDifficulty, System.Random random = null)
    {
        System.Random randomizer;

        // Filter the spawn tables to those within the difficulty range
        var filteredTables = _regularSpawnTables.Where(table => table.DifficultyScore >= minDifficulty && table.DifficultyScore <= maxDifficulty).ToList();

        // Check if there are any tables in the filtered list
        if (filteredTables.Count == 0)
        {
            return null; // Or handle this case as needed
        }

        // Randomly select and return one of the filtered tables

        if(random == null)
        {
            randomizer = new System.Random();
        }else
        {
            randomizer = random;
        }

        int randomIndex = randomizer.Next(filteredTables.Count);
        return filteredTables[randomIndex];



    }

}
