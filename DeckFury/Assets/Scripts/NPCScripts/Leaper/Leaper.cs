using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Leaper : NPC
{

    [SerializeField] float jumpCooldown = 5f;

    [SerializeField] bool EnableAI = true;
    [SerializeField] bool canJump;
    [SerializeField] bool isJumping;

    [SerializeField] LeaperAttackController attackController;
    [SerializeField] float attackWindupDelay = 0.5f;



    


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canJump = false;
        isJumping = true;
        StartCoroutine(JumpCooldown(2)); 
    
    }

    // Update is called once per frame
    void Update()
    {
        if (!EnableAI) { return; }
        if(!CanAct){return;}

        if (!isJumping)
        {
            StartCoroutine(JumpToPlayer());
            
        }
        


    }

    IEnumerator JumpToPlayer()
    {
        if (!canJump) { yield break; }
        if (isJumping) { yield break; }

        isJumping = true;
        canJump = false;

        
        yield return StartCoroutine(JumpMovementCoroutine()); // Wait for the jump movement to complete

        //DamageTiles(); // Call the damageTiles method once the movement is complete
        yield return StartCoroutine(TelegraphAttack());

        yield return StartCoroutine(JumpCooldown());

    }

    IEnumerator JumpMovementCoroutine() 
    {

        Vector3Int? tiles = GetRandomValidTile(GetTilesAdjacentToPlayer());
        if (tiles.HasValue)
        {
            var xpos = tiles.Value.x;
            var ypos = tiles.Value.y;
            

            _stageManager.SetTileEntity(null, currentTilePosition);
            Vector3Int destination = new Vector3Int(xpos, ypos, 0);
            currentTilePosition.Set(xpos, ypos, 0);

            //Gotta make sure to set the entity at the destination before you make the jump, otherwise if some entity moves into the tile
            //before they reach the destination, things break.
            _stageManager.SetTileEntity(this, destination);


            yield return worldTransform.DOMove(destination, 0.5f)
                 .SetEase(Ease.InOutExpo)
                 .OnComplete(() =>
                 {
                     _stageManager.SetTileEntity(this, currentTilePosition);
                 }).WaitForCompletion();
        }
        else
        {
            yield break;
        }
        
    }
   
    IEnumerator TelegraphAttack()
    {
        _stageManager.SetVFXTile(_stageManager.DangerVFXTile, GetAdjacentTiles(), attackWindupDelay);
        yield return new WaitForSeconds(attackWindupDelay);
        _entityAnimator.PlayAnimationClip(_entityAnimator.animationList[0]);

        yield return StartCoroutine(attackController.TriggerAttackHitBoxes());


    }



    IEnumerator JumpCooldown(float duration = 0)
    {
        float actualCooldown = jumpCooldown;
        if(duration > 0)
        {
            actualCooldown = duration;
        }

        float randomFloat = Random.Range(-0.5f, 0.75f);
        yield return new WaitForSeconds(actualCooldown + randomFloat);
        canJump = true;
        isJumping = false;
    }


    //Considering the way the jump works, this is not a good solution and it makes it really hard to create a system that properly telegraphs the Leaper's attack
    //It would be better to use an attack hitbox box collider that goes around the leaper to detect the player which would allow for much clearer telegraph
    //once you implement an aiming square reticle that shows where the leaper is going to attack.
    void DamageTiles()
    {
        AttackPayload payload = new AttackPayload(10);

        var directionsIncludingCurrent = new List<Vector3Int>(VectorDirections.Vector3IntAll);
        directionsIncludingCurrent.Add(Vector3Int.zero);


        foreach (var direction in directionsIncludingCurrent)
        {
            var tileCheck = currentTilePosition + direction;
            var tileData = _stageManager.GetGroundTileData(tileCheck);
          

            if (tileData != null && tileData.entity != null)
            {
                if (tileData.entity.gameObject.CompareTag("Player"))
                {
                    Debug.Log("FOUND " + tileData.entity);
                    tileData.entity.HurtEntity(payload);
                }
                
            }
            
            
        }
        
    }

    Vector3Int? GetRandomValidTile(List<Vector3Int> adjacentTiles)
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();
        foreach(var tile in adjacentTiles)
        {
            if (_stageManager.CheckValidTile(tile))
            {
                validTiles.Add(tile);
            }
        }
        if (validTiles.Count == 0)
        {
            return null; 
        }
        else if (validTiles.Count == 1)
        {
            return validTiles[0]; 
        }
        else
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            return validTiles[randomIndex]; 
        }

    }


    List<Vector3Int> GetTilesAdjacentToPlayer()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach(var direction in VectorDirections.Vector3IntCardinal)
        {
            Vector3Int adj = player.currentTilePosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }

    List<Vector3Int> GetAdjacentTiles()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach(var direction in VectorDirections.Vector3IntCardinal)
        {
            Vector3Int adj = currentTilePosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }






    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow = null)
    {
        base.AdditionalDestructionEvents(killingBlow);
        EnableAI = false;
        isJumping = false;
       
    }
}
