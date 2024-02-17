using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper class meant to be placed on the parent game object of StageEntities. Allows for quick access to the StageEntity object within the
//EntityWrapper's game object children.
public class EntityWrapper : MonoBehaviour
{
    [field:SerializeField] public StageEntity stageEntity {get; private set;}

    [SerializeField] GameObject _originalPrefab;
    public GameObject OriginalPrefab => _originalPrefab;



}
