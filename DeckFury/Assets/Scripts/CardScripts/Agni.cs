using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agni : CardEffect
{


    [SerializeField] AnimationClip AgniVFS; //Set in inspector
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] Animator agniAnimator;
    [SerializeField] int validHP;
    AimpointController aimpoint;
    int extraDamage=0;

    public override void ActivateCardEffect()
    {
        validHP = (int)cardSO.QuantifiableEffects[2].GetValueDynamic();
        int maxATK = (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
        aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);
        //calculte damage, low hp higher damage
        //hp >= 50
        if(player.CurrentHP>=validHP){
            Debug.Log("too many HP");
            return;
        }
        else
        //hp<=10
        if(player.CurrentHP<=(int)cardSO.QuantifiableEffects[1].GetValueDynamic()){
            extraDamage=maxATK;
        }
        //10<hp<50
        else{
            extraDamage = (int) maxATK * (validHP - player.CurrentHP) / validHP;
        }
        attackPayload.damage = extraDamage;
        agniAnimator.Play("AgniVFS", 0);
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
            entity.HurtEntity(attackPayload);
            Vector2Int shoveDirection = aimpoint.GetAimVector2Int();
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
