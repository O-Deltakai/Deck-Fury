using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data container class that stores information on an ability an NPC has.
[System.Serializable]
public class AbilityData
{
    public string abilityName;

    //What animation should the NPC play when using this ability? 
    public AnimationClip animationToUse;

    public AttackPayload attackPayload;

    //A list of tile positions which the ability is able to affect. This list is generally used by the stage manager to correctly
    //set danger indicators on the VFXTilemap
    //Remember, the default aim direction at 0 rotation is DOWN.
    public List<Vector2Int> rangeOfInfluence;

    //Experimental: return an adjusted range of influence based on aim direction by inverting/negating values
    public List<Vector2Int> AimAbilityTowardsDirection(AimDirection aimDirection)
    {
        List<Vector2Int> adjustedRangeOfInfluence = new List<Vector2Int>();

        switch (aimDirection) {
            case AimDirection.Down:
                return rangeOfInfluence;

            case AimDirection.Up:
                foreach(Vector2Int position in rangeOfInfluence)
                {
                    adjustedRangeOfInfluence.Add(new Vector2Int(position.x, -position.y));
                }
                break;

            case AimDirection.Right:
                foreach(Vector2Int position in rangeOfInfluence)
                {
                    adjustedRangeOfInfluence.Add(new Vector2Int(-position.y, position.x));
                }
                break;

            case AimDirection.Left:
                foreach(Vector2Int position in rangeOfInfluence)
                {
                    adjustedRangeOfInfluence.Add(new Vector2Int(position.y, position.x));
                }
                break;            
   
        }
        return adjustedRangeOfInfluence;
    }

    //Experimental: Add the range of influence to an anchor point (normally the user of the ability) such that it reflects the world coordinates
    //of the ability
    public List<Vector2Int> AnchorRangeOfInfluenceToTilePosition(AimDirection aimDirection, Vector3Int anchor)
    {
        List<Vector2Int> aimedRange = AimAbilityTowardsDirection(aimDirection);
        List<Vector2Int> anchoredRange = new List<Vector2Int>();

        foreach(Vector2Int pos in aimedRange)
        {
            anchoredRange.Add(new Vector2Int(pos.x + (int)anchor.x, pos.y + (int)anchor.y));
        }

        return anchoredRange;

    }


}
