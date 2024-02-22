using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Handles common resources for entities, such as VFX and SFX.
/// </summary>
public class EntityResourceManager : MonoBehaviour
{


[Header("VFX")]
    [SerializeField] GameObject _entitySpawnVFX;
    public GameObject EntitySpawnVFX => _entitySpawnVFX;

    [SerializeField] GameObject _takeCritVFX;
    public GameObject TakeCritVFX => _takeCritVFX;

    [SerializeField] GameObject _resistDamageVFX;
    public GameObject ResistDamageVFX => _resistDamageVFX;


[Header("SFX")]
    [SerializeField] EventReference _entitySpawnSFX;
    public EventReference EntitySpawnSFX => _entitySpawnSFX;

    [SerializeField] EventReference _entityDeathSFX;
    public EventReference EntityDeathSFX => _entityDeathSFX;

    [SerializeField] EventReference _entityHurtSFX;
    public EventReference EntityHurtSFX => _entityHurtSFX;

    [SerializeField] EventReference _entityTakeCritSFX;
    public EventReference EntityTakeCritSFX => _entityTakeCritSFX;

    [SerializeField] EventReference _entityResistDamageSFX;
    public EventReference EntityResistDamageSFX => _entityResistDamageSFX;

[Header("Miscellaneous")]
    [SerializeField] GameObject _markedFireExplosion;
    public GameObject MarkedFireExplosion => _markedFireExplosion;


}
