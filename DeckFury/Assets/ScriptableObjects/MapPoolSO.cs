using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapLayout Pool", menuName = "New MapLayout Pool", order = 0)]
public class MapPoolSO : ScriptableObject
{

    [SerializeField] string _mapPoolName;
    public string MapPoolName{get{ return _mapPoolName; }}
 

    [SerializeField] List<MapLayoutController> _standardMaps;
    public List<MapLayoutController> StandardMaps{get{ return _standardMaps; }}

    [SerializeField] List<MapLayoutController> _intermediateMaps;
    public List<MapLayoutController> IntermediateMaps{get{ return _intermediateMaps; }}

    [SerializeField] List<MapLayoutController> _advancedMaps; 
    public List<MapLayoutController> AdvancedMaps{get{ return _advancedMaps; }}




}
