using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;



public class LaserTurret : StageEntity
{
    [SerializeField] AimDirection aimDirection;
    [SerializeField] VolumetricLineBehavior lineBehavior;
    [SerializeField] Transform laserStartPosition;

    [SerializeField] float maxLaserDistance;


    [SerializeField] LayerMask targetLayer; // layer mask for targeting specific layers

    [SerializeField] AttackPayload attackPayload;
    [SerializeField] float damageCooldown;


    bool canDamage = true;




    void Update()
    {
        DrawAndDetectLaser();
    }


    void DrawAndDetectLaser()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(laserStartPosition.position, VectorDirections.Vector2IntCardinal[0], maxLaserDistance, targetLayer);
        float laserLength = maxLaserDistance;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                // Check if the hit object is a wall to stop the laser
                if (hit.collider.CompareTag("Wall"))
                {
                    laserLength = hit.distance;
                    break;
                }

                if(canDamage)
                {
                    // Damage the entity
                    StageEntity hitEntity = hit.collider.GetComponent<StageEntity>();
                    if(hitEntity)
                    {
                        hitEntity.HurtEntity(attackPayload);
                        StartCoroutine(Cooldown(damageCooldown));
                    }

                }
            }
        }

        lineBehavior.EndPos = VectorDirections.Vector3Cardinal[0] * laserLength; 

    }

    IEnumerator Cooldown(float duration)
    {
        canDamage = false;
        yield return new WaitForSeconds(duration);
        canDamage = true;

    }

    void OnDisable()
    {
        StopAllCoroutines();
        canDamage = true;
    }

}
