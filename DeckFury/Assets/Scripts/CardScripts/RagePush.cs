using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

public class RagePush : CardEffect
{
    [SerializeField] AnimationClip VFXAnimationClip;

    public override void ActivateCardEffect()
    {
        StartCoroutine(ExecuteAfterAnimation());
    }

    IEnumerator ExecuteAfterAnimation()
    {

        yield return new WaitForSeconds(VFXAnimationClip.length * 0.5f);

        Vector3Int[] directions = VectorDirections.Vector3IntCardinal;
        var playerPos = player.currentTilePosition;

        foreach (Vector3Int direction in directions)
        {
            var tileCheck = playerPos + direction;

            if (stageManager.GetGroundTileData(tileCheck) != null && stageManager.GetGroundTileData(tileCheck).entity != null)
            {
                var tileEntity = stageManager.GetGroundTileData(tileCheck).entity;
                Debug.Log(tileEntity.name);
                tileEntity.HurtEntity(attackPayload); // damaging
                Vector3Int shoveDirection = direction; // Shoving it away from the player
                tileEntity.AttemptMovement(shoveDirection.x*2, shoveDirection.y*2, 0.15f, DG.Tweening.Ease.OutQuart, ForceMoveMode.Forward);
            }
        }

        StartCoroutine(DisableEffectPrefab());

    }

    protected override IEnumerator DisableEffectPrefab()
    {
        //The minimum amount of time that an effect prefab can stay active is the player animation length, because if the card uses
        //animation events, the animation is the one which calls the card effect. If you disable your effect prefab before the player 
        //animation hits that event, the card won't work.
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length + 0.05f);
        Destroy(gameObject);
    }

    

    /*List<Vector3Int> GetTilesAdjacentToPlayer()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach (var direction in VectorDirections.Vector3IntCardinal)
        {
            Vector3Int adj = player.currentTilePosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }*/
}

