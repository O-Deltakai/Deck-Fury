using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper class meant to be placed on the parent game object of StageEntities. Allows for quick access to the StageEntity object within the
//EntityWrapper's game object children.
public class EntityWrapper : MonoBehaviour
{
    [SerializeField] StageEntity _entity;
    public StageEntity Entity {get => _entity; set => _entity = value;}

    [SerializeField] GameObject _originalPrefab;
    public GameObject OriginalPrefab => _originalPrefab;

    void OnValidate()
    {
        Entity = GetComponentInChildren<StageEntity>();
    }

    void Awake()
    {
        if(Entity == null)
        {
            Entity = GetComponentInChildren<StageEntity>();
        }
    }


}
