using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A number of static helper methods used within the context of the game's cardinal aiming system.
/// </summary>
public class CardinalAimSystem
{

    public static List<Vector2Int> AimTowardsDirection(AimDirection aimDirection, List<Vector2Int> positions)
    {
        List<Vector2Int> adjustedPositions = new();

        switch (aimDirection) 
        {
            case AimDirection.Down:
                return positions;

            case AimDirection.Up:
                for (int i = 0; i < positions.Count; i++)
                {
                    Vector2Int position = positions[i];
                    adjustedPositions.Add(new Vector2Int(position.x, -position.y));
                }
                break;

            case AimDirection.Right:
                for (int i = 0; i < positions.Count; i++)
                {
                    Vector2Int position = positions[i];
                    adjustedPositions.Add(new Vector2Int(-position.y, position.x));
                }
                break;

            case AimDirection.Left:
                for (int i = 0; i < positions.Count; i++)
                {
                    Vector2Int position = positions[i];
                    adjustedPositions.Add(new Vector2Int(position.y, position.x));
                }
                break;

        }
        return adjustedPositions;
    }

    public static List<Vector2Int> AnchoredAimTowardsDirection(AimDirection aimDirection, List<Vector2Int> positions, Vector3Int anchor)
    {
        List<Vector2Int> aimedRange = AimTowardsDirection(aimDirection, positions);
        List<Vector2Int> anchoredRange = new();

        for (int i = 0; i < aimedRange.Count; i++)
        {
            Vector2Int pos = aimedRange[i];
            anchoredRange.Add(new Vector2Int(pos.x + anchor.x, pos.y + anchor.y));
        }

        return anchoredRange;
    }

    public static Quaternion GetRotationWithAimDirection(AimDirection aimDirection)
    {
        return aimDirection switch
        {
            AimDirection.Up => Quaternion.Euler(0, 0, 180),
            AimDirection.Down => Quaternion.Euler(0, 0, 0),
            AimDirection.Left => Quaternion.Euler(0, 0, -90),
            AimDirection.Right => Quaternion.Euler(0, 0, 90),
            _ => Quaternion.Euler(0, 0, 0),
        };
    }

    public static Vector3 GetVector3WithAimDirection(AimDirection aimDirection)
    {
        return aimDirection switch
        {
            AimDirection.Up => new Vector3(0, 1, 0),
            AimDirection.Down => new Vector3(0, -1, 0),
            AimDirection.Left => new Vector3(-1, 0, 0),
            AimDirection.Right => new Vector3(1, 0, 0),
            _ => new Vector3(0, 0, 0),
        };
    }


    public static AimDirection GetClosestAimDirectionByVector(Vector2Int directionVector)
    {

        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            if (directionVector.x > 0)
            {
                return AimDirection.Right;
            }
            else
            {
                return AimDirection.Left;
            }
        }
        else
        {
            if (directionVector.y > 0)
            {              
                return AimDirection.Up;
            }
            else
            {                
                return AimDirection.Down;
            }
        }
    }


}
