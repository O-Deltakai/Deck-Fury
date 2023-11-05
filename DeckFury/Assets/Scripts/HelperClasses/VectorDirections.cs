using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorDirections 
{
    public static readonly Vector2Int[] Vector2IntAll = 
    {
        new Vector2Int(0, 1),    // N
        new Vector2Int(1, 1),    // NE
        new Vector2Int(1, 0),    // E
        new Vector2Int(1, -1),   // SE
        new Vector2Int(0, -1),   // S
        new Vector2Int(-1, -1),  // SW
        new Vector2Int(-1, 0),   // W
        new Vector2Int(-1, 1)    // NW
    };

    public static readonly Vector2Int[] Vector2IntCardinal = 
    {
        new Vector2Int(0, 1),    // N
        new Vector2Int(1, 0),    // E
        new Vector2Int(0, -1),   // S
        new Vector2Int(-1, 0),   // W
    };

    public static readonly Vector3Int[] Vector3IntAll = 
    {
        new Vector3Int(0, 1, 0),    // N
        new Vector3Int(1, 1, 0),    // NE
        new Vector3Int(1, 0, 0),    // E
        new Vector3Int(1, -1, 0),   // SE
        new Vector3Int(0, -1, 0),   // S
        new Vector3Int(-1, -1, 0),  // SW
        new Vector3Int(-1, 0, 0),   // W
        new Vector3Int(-1, 1, 0)    // NW
    };
    public static readonly Vector3Int[] Vector3IntCardinal = 
    {
        new Vector3Int(0, 1, 0),    // N
        new Vector3Int(1, 0, 0),    // E
        new Vector3Int(0, -1, 0),   // S
        new Vector3Int(-1, 0, 0),   // W
    };

    public static readonly Vector2[] Vector2All = 
    {
        new Vector2(0, 1),    // N
        new Vector2(0.707f, 0.707f),    // NE (normalized diagonal for unity)
        new Vector2(1, 0),    // E
        new Vector2(0.707f, -0.707f),   // SE (normalized diagonal for unity)
        new Vector2(0, -1),   // S
        new Vector2(-0.707f, -0.707f),  // SW (normalized diagonal for unity)
        new Vector2(-1, 0),   // W
        new Vector2(-0.707f, 0.707f)    // NW (normalized diagonal for unity)
    };

    public static readonly Vector2[] Vector2Cardinal = 
    {
        new Vector2(0, 1),    // N
        new Vector2(1, 0),    // E
        new Vector2(0, -1),   // S
        new Vector2(-1, 0),   // W
    };

    public static readonly Vector3[] Vector3All = 
    {
        new Vector3(0, 1, 0),    // N
        new Vector3(0.707f, 0.707f, 0),    // NE (normalized diagonal for unity)
        new Vector3(1, 0, 0),    // E
        new Vector3(0.707f, -0.707f, 0),   // SE (normalized diagonal for unity)
        new Vector3(0, -1, 0),   // S
        new Vector3(-0.707f, -0.707f, 0),  // SW (normalized diagonal for unity)
        new Vector3(-1, 0, 0),   // W
        new Vector3(-0.707f, 0.707f, 0)    // NW (normalized diagonal for unity)
    };

    public static readonly Vector3[] Vector3Cardinal = 
    {
        new Vector3(0, 1, 0),    // N
        new Vector3(1, 0, 0),    // E
        new Vector3(0, -1, 0),   // S
        new Vector3(-1, 0, 0),   // W
    };

}
