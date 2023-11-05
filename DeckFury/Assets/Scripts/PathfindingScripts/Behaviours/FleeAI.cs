using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FleeAI : MonoBehaviour
{
    public NPC attacker;
    public StageEntity Target;
    public Ease movementType;
    public bool pauseAI = false;
    public int tileRange;

    private void Awake()
    {
        if (!attacker)
        {
            attacker = GetComponent<NPC>();
        }
    }

    public bool ShouldFlee()
    {
        Vector3Int diff = attacker.currentTilePosition - Target.currentTilePosition;
        return Math.Abs(diff.x) <= tileRange && Math.Abs(diff.y) <= tileRange;
    }

    private bool IsAwayFromTarget(Vector3Int location)
    {
        Vector3Int diff = location - Target.currentTilePosition;
        return Math.Abs(diff.x) > tileRange || Math.Abs(diff.y) > tileRange;
    }

    public Vector3Int? CanFlee()
    {
        Vector3Int nextPositiion = new Vector3Int(99, 99, 0);
        Vector3Int? anyValidLocation = null;

        Vector3Int[] cardinalDirections = VectorDirections.Vector3IntCardinal.OrderBy(x => Random.value).ToArray();
        foreach (Vector3Int direction in cardinalDirections)
        {
            Vector3Int temp = attacker.currentTilePosition + direction;
            if (StageManager.Instance.CheckValidTile(temp))
            {
                anyValidLocation = temp;
                if (IsAwayFromTarget(temp))
                {
                    nextPositiion = temp;
                    break;
                }
            }
        }

        if (!nextPositiion.Equals(new Vector3Int(99, 99, 0)))
        {
            return nextPositiion;
        }

        return anyValidLocation;
    }

    public void Flee(Vector3Int nextPositiion)
    {
        if (pauseAI) { return; }
        if (!attacker.CanInitiateMovementActions) { return; }

        if (!nextPositiion.Equals(new Vector3Int(99, 99, 0)))
        {
            Vector3Int moveLocation = nextPositiion - attacker.currentTilePosition;
            StartCoroutine(attacker.TweenMove(moveLocation.x, moveLocation.y, 0.1f, movementType));
        }
    }
}
