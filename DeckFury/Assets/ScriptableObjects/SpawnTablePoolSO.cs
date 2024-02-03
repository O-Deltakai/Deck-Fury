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

    public SpawnTableSO GetSpawnTableByDifficulty(int minDifficulty, int maxDifficulty, System.Random random = null, bool findClosestToMax = false)
    {
        if (_regularSpawnTables.Count == 0)
        {
            Debug.LogWarning("RegularSpawnTables is empty. Returning null.");
            return null;
        }

        System.Random randomizer;

        // Filter the spawn tables to those within the difficulty range
        var filteredTables = _regularSpawnTables.Where(table => table.DifficultyScore >= minDifficulty && table.DifficultyScore <= maxDifficulty).ToList();

        // Check if there are any tables in the filtered list
        if (filteredTables.Count == 0)
        {
            // Find the closest table to maxDifficulty
            var closestToMax = _regularSpawnTables.OrderBy(table => Mathf.Abs(table.DifficultyScore - maxDifficulty)).FirstOrDefault();

            // Find the closest table to minDifficulty
            var closestToMin = _regularSpawnTables.OrderBy(table => Mathf.Abs(table.DifficultyScore - minDifficulty)).FirstOrDefault();

            // Choose the closest table based on the difference from maxDifficulty and minDifficulty
            if (findClosestToMax)
            {
                return closestToMax;
            }
            else
            {
                return closestToMin;
            }
        }

        // Randomly select and return one of the filtered tables
        if (random == null)
        {
            randomizer = new System.Random();
        }
        else
        {
            randomizer = random;
        }

        int randomIndex = randomizer.Next(filteredTables.Count);
        return filteredTables[randomIndex];
    }



}
