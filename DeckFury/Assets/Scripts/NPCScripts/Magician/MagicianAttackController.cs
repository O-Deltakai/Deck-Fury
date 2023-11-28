using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianAttackController : MonoBehaviour
{

    Magician magician;
    PlayerController playerReference;

    // The Zombie prefab to spawn
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject targetingReticle;
    [SerializeField] VampireBasicAttack BasicAttack;
    //zombie bomb prefab
    [SerializeField] GameObject zombieBombPrefab;
    //normal attack prefab
    [SerializeField] GameObject darkOrbPrefab;
    
    [SerializeField] GameObject bloodBoltPrefab;
    [SerializeField] AttackPayload bloodBoltPayload;


    //fury attack prefab
    [SerializeField] GameObject furyAttackPrefab;

    void Awake()
    {
        magician = GetComponent<Magician>();
    }

    void Start()
    {
        playerReference = magician.player;
    }


    void SummonZombie()
    {

    }


    void FireDarkOrb()
    {

    }

    void FireBloodBolt()
    {
        List<Vector3Int> targetTiles = GetTilesCardinalAdjacentToTarget(playerReference.currentTilePosition);
        
        foreach(var tile in targetTiles)
        {
            
            Vector3 direction = tile - transform.position;

            Bullet bullet = Instantiate(bloodBoltPrefab, gameObject.transform.position, transform.rotation).GetComponent<Bullet>();

            //Set bullet variables
            Vector2 bulletDirection = new Vector2(direction.normalized.x, direction.normalized.y);
            bullet.ChangeTrailRendererColor(new Color (41/255f, 9/255f, 22/255f));
            bullet.team = EntityTeam.Enemy;
            bullet.velocity = bulletDirection;
            bullet.speed = 10;
            bullet.trailRenderer.time = 0.2f;
            bullet.attackPayload = bloodBoltPayload;

            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ensure it doesnt hit themselves

        }
    
    }

    void FuryBlast()
    {

    }

    List<Vector3Int> GetTilesCardinalAdjacentToTarget(Vector3Int targetTile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        adjacentTiles.Add(targetTile);
        foreach(var direction in VectorDirections.Vector3IntCardinal)
        {
            Vector3Int adj = targetTile + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }



}
