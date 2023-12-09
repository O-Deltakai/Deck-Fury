using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] GameObject _mapLayoutPrefab;
    [SerializeField] bool _useMapLayoutPrefab = false;
    public bool UseMapLayoutPrefab => _useMapLayoutPrefab;
    public MapLayoutController CurrentMap { get; private set; }

    void Awake()
    {
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
