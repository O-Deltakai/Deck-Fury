using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeWaysDagger : CardEffect
{
    
    [SerializeField] GameObject VFXPrefabToUse; //Set in inspector

    public override void ActivateCardEffect()
    {
        int quantityOfDagger = (int)cardSO.QuantifiableEffects[0].GetValueDynamic();
        
        if(!cardSO.ObjectSummonsArePooled)
        {
            for ( int i = 0; i<quantityOfDagger;i++){
                Bullet dagger = Instantiate(VFXPrefabToUse, player.currentTilePosition,
                new Quaternion(0, 0, (float)player.aimpoint.currentAimDirection, 0)).GetComponent<Bullet>();

                AssignVariable(dagger, i - (int) quantityOfDagger/2);

                dagger.gameObject.SetActive(true);
            }

        }else
        {
            for ( int i = 0; i<quantityOfDagger;i++){
                Bullet dagger = VFXPrefabToUse.GetComponent<Bullet>();

                AssignVariable(dagger, i - (int) quantityOfDagger/2);

                dagger.gameObject.SetActive(true);
            }
        }


        StartCoroutine(DisableEffectPrefab());
    }

    //design for 3s way and 5 ways
    protected void AssignVariable(Bullet dagger, int index){
            dagger.transform.position = player.currentTilePosition;
            dagger.attackPayload = attackPayload;
            dagger.speed = 10;

            switch (player.aimpoint.currentAimDirection) 
            {
                case AimDirection.Up:
                    dagger.transform.rotation = Quaternion.Euler(0, 0, 180-index*45);
                    dagger.velocity.x = 0 + index*0.5f;
                    dagger.velocity.y = 1;
                    break;

                case AimDirection.Down:
                    dagger.transform.rotation = Quaternion.Euler(0, 0, index*45);
                    dagger.velocity.x = 0 + index*0.5f;
                    dagger.velocity.y = -1;
                    break; 
                case AimDirection.Left:
                    dagger.transform.rotation = Quaternion.Euler(0, 0, -90-index*45);
                    dagger.velocity.x = -1;
                    dagger.velocity.y = 0 + index*0.5f;
                    break;

                case AimDirection.Right:
                    dagger.transform.rotation = Quaternion.Euler(0, 0, 90+index*45);
                    dagger.velocity.x = 1;
                    dagger.velocity.y = 0 + index*0.5f;
                    break;
            }
            
            dagger.gameObject.SetActive(true);
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length);
        gameObject.SetActive(false);

    }




}
