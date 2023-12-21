using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] bool _useMapLayoutPrefab = false;
    public bool UseMapLayoutPrefab => _useMapLayoutPrefab;

    [SerializeField] GameObject _mapLayoutPrefab;
    public MapLayoutController CurrentMap { get; private set; }

    StageStateController stageStateController;


    void Awake()
    {
        stageStateController = StageStateController.Instance;

        if(stageStateController && stageStateController.StageMapPrefab)
        {
            _mapLayoutPrefab = stageStateController.StageMapPrefab;
        }


        if(_useMapLayoutPrefab && _mapLayoutPrefab)
        {
            InstantiateMapLayout(); 
        }


    }

    void InstantiateMapLayout()
    {
        CurrentMap = GetComponentInChildren<MapLayoutController>();
        if(CurrentMap)
        {
            Destroy(CurrentMap.gameObject);
        }

        CurrentMap = Instantiate(_mapLayoutPrefab, transform).GetComponent<MapLayoutController>();

    }


}
