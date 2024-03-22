using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Observes the parent entity for damage events and performs various actions depending on the damage taken.
/// </summary>
public class EntityDamageObserver : MonoBehaviour
{
    [SerializeField] StageEntity _entity;

    [Header("VFX")]
    [SerializeField] GameObject _takeCritVFX;
    [SerializeField] GameObject _resistDamageVFX;

    [Header("SFX")]
    [SerializeField] EventReference takeCritSFX;
    [SerializeField] EventReference resistDamageSFX;

    void OnValidate()
    {
        _entity = GetComponentInParent<StageEntity>();

        if(_entity && TryGetComponent(out EntityResourceManager resourceManager))
        {
            takeCritSFX = resourceManager.EntityTakeCritSFX;
            resistDamageSFX = resourceManager.EntityResistDamageSFX;
        }

    }

    void Awake()
    {

    }


    void Start()
    {
        if (_entity == null)
        {
            _entity = GetComponentInParent<StageEntity>();
            if(_entity == null)
            {
                Debug.LogError("EntityDamageObserver must be a child of a StageEntity.");
            }
        }

        if(_entity)
        {
            takeCritSFX = _entity.GetComponent<EntityResourceManager>().EntityTakeCritSFX;
            resistDamageSFX = _entity.GetComponent<EntityResourceManager>().EntityResistDamageSFX;
        }  

        _entity.OnTakeCritDamage += TakeCritDamage;
        _entity.OnResistDamage += ResistDamage;
    }


    void TakeCritDamage()
    {
        if (_takeCritVFX)
        {
            _takeCritVFX.SetActive(false);
            _takeCritVFX.SetActive(true);
        }


        RuntimeManager.PlayOneShot(takeCritSFX, _entity.transform.position);
    }

    void ResistDamage()
    {
        if (_resistDamageVFX)
        {
            _resistDamageVFX.SetActive(false);
            _resistDamageVFX.SetActive(true);
        }


        RuntimeManager.PlayOneShot(resistDamageSFX, _entity.transform.position);
    
    }

}
