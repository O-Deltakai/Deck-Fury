using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForceBlast : CardEffect
{
    [SerializeField] AnimationClip VFXAnimationClip;
    [SerializeField] BoxCollider2D effectCollider;

    AimpointController aimpoint;


    public override void ActivateCardEffect()
    {
        StartCoroutine(HardDisableTimer());
        aimpoint = player.aimpoint;
        transform.position = player.worldTransform.position;

        FaceTowardsAimpoint(aimpoint);

        int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
        LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(effectCollider.transform.position, effectCollider.size, effectCollider.transform.eulerAngles.z, stageEntitiesMask);
        if(hits.Length == 0){return;}


        var sortedEntities = hits.OrderBy(h => -Vector2.Distance(h.transform.position, transform.position)).ToList();
        

        foreach(var collider2D in sortedEntities) 
        {
            StageEntity entity = collider2D.attachedRigidbody.gameObject.GetComponent<StageEntity>();
            if(entity == null)
            {
                print("collider did not have a StageEntity attached");
                continue;
            }
            //if(!entity.CompareTag("Enemy") || !entity.CompareTag("EnvironmentalHazard")){continue;}

            if(entity.CompareTag("Enemy") || entity.CompareTag("EnvironmentalHazard"))
            {
                entity.HurtEntity(attackPayload);
                Vector2Int shoveDirection = aimpoint.GetAimVector2Int() * (int)cardSO.QuantifiableEffects[0].GetValueDynamic();

                entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, DG.Tweening.Ease.OutQuart, ForceMoveMode.Forward);         
            }


        }

        

        StartCoroutine(DisableEffectPrefab());
    }



    //Rotate the object towards the aimpoint so that it fires in the direction of the aimpoint
    void FaceTowardsAimpoint(AimpointController aimpoint)
    {
        switch (aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;

            case AimDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break; 
            case AimDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;

            case AimDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(VFXAnimationClip.length + 0.05f);
        gameObject.SetActive(false);
        
    }

    IEnumerator HardDisableTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

}
