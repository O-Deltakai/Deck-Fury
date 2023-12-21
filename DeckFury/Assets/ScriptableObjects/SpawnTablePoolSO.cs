using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Spawn Table Pool", menuName = "New Spawn Table Pool", order = 0)]
public class SpawnTablePoolSO : ScriptableObject
{
    [SerializeField] List<SpawnTableSO> _spawnTablePool = new List<SpawnTableSO>();
    public IReadOnlyList<SpawnTableSO> SpawnTablePool => _spawnTablePool;



}
