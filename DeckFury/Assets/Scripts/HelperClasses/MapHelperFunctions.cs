using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MapHelperFunctions
{

    /// <summary>
    /// Returns a list of Vector3Ints that are the exact coordinates of every tile inside the given Tilemap.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    public static List<Vector3Int> GetTilePositions(Tilemap tilemap)
    {
        List<Vector3Int> positions = new List<Vector3Int>(100);
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                positions.Add(pos);
            }
        }

        return positions;
    }

    /// <summary>
    /// Returns a list of Vector3Ints of which are all valid for a given list of positions as determined by the StageManager.
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="stageManager"></param>
    /// <returns></returns>
    public static List<Vector3Int> GetValidPositions(List<Vector3Int> positions, StageManager stageManager)
    {
        List<Vector3Int> validPositions = new List<Vector3Int>(100);
        foreach (Vector3Int pos in positions)
        {
            if (stageManager.CheckValidTile(pos))
            {
                validPositions.Add(pos);
            }

        }

        return validPositions;
    }

    public static List<Vector3Int> GetValidTilePositions(Tilemap tilemap, StageManager stageManager)
    {
        List<Vector3Int> validTilePositions = new List<Vector3Int>(100);
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos) && stageManager.CheckValidTile(pos))
            {
                validTilePositions.Add(pos);
            }
        }

        return validTilePositions;
    }
}