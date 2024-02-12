using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShieldSteal : CardEffect
{


    [SerializeField] AnimationClip VFXAnimation; //Set in inspector
    [SerializeField] BoxCollider2D attackHitbox;
    AimpointController aimpoint;


    public override void ActivateCardEffect()
    {
        aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);

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

    private void OnCollisionEnter2D(Collision2D other)
    {
        StageEntity entity = other.gameObject.GetComponent<StageEntity>();
        if(entity == null){return;}
        if(other.gameObject.tag == "Enemy" || other.gameObject.CompareTag("EnvironmentalHazard"))
        {
            
            player.ShieldHP+=(int)cardSO.QuantifiableEffects[0].GetValueDynamic();
            entity.HurtEntity(attackPayload);
            Vector2Int shoveDirection = aimpoint.GetAimVector2Int();

            //entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, Ease.OutQuart, ForceMoveMode.Forward); 
            attackHitbox.enabled = false;
        }

    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);
        attackHitbox.enabled = true;
    }
}
