using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSword : CardEffect
{
    [SerializeField] AnimationClip VFXAnimationClip;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] BoxCollider2D hitBox;

    public override void ActivateCardEffect()
    {
        transform.position = player.worldTransform.position;

        AimpointController aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);

        TriggerHitbox();

        StartCoroutine(DisableEffectPrefab());
    }


    void TriggerHitbox()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitBox.transform.position, hitBox.size, hitBox.transform.eulerAngles.z, targetLayer);

        foreach(Collider2D collider2D in hits) 
        {
            StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();

            if(entityHit == null)
            {
                print("collider did not have a StageEntity attached");
                continue;
            }


            if(entityHit.CompareTag(TagNames.Enemy.ToString()) || entityHit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
            {
                entityHit.HurtEntity(attackPayload);
            }             
        }
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
        yield return new WaitForSeconds(VFXAnimationClip.length);
        gameObject.SetActive(false);
        
    }
}
