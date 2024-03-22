using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : CardEffect
{
    [SerializeField] AnimationClip VFXAnimation; //Set in inspector
    [SerializeField] GameObject VFXPrefabToUse; //Set in inspector

    //Set this in the inspector (The prefabs in here should be the same ones that are attached on the EffectPrefab itself)
    [SerializeField] List<GameObject> lightningVFXPool = new List<GameObject>();

    [SerializeField] LayerMask stageEntitiesMask;


    public override void ActivateCardEffect()
    {
        List<StageEntity> entities = new();
        
        //Just creates a big box around the player and checks for any entities in that box
        Collider2D[] hits = Physics2D.OverlapBoxAll(player.worldTransform.position, new Vector2(20, 20), 0, stageEntitiesMask);
        foreach(var hit in hits)
        {
            if(hit.gameObject.CompareTag(TagNames.Enemy.ToString()) && hit.TryGetComponent(out StageEntity stageEntity))
            {
                entities.Add(stageEntity);
            }
        }
        


        //What do you do if the number of entities on the board is greater than the number of VFX prefabs?
        while(entities.Count > lightningVFXPool.Count)
        {
            AddNewVFXPrefabToPool();
        }

        int index = 0;  
        foreach(var entity in entities)
        {
            if(entity.gameObject.CompareTag(TagNames.Enemy.ToString()))
            {
                
                lightningVFXPool[index].transform.position = entity.currentTilePosition;

                EnableLightningVFX(lightningVFXPool[index]);
                index++;

                entity.HurtEntity(attackPayload);

            }

        }
          
        StartCoroutine(DisableEffectPrefab());
    }

    //Enable the VFX prefab and then start a coroutine makes it disable itself after some time
    void EnableLightningVFX(GameObject vfxPrefab)
    {
        vfxPrefab.SetActive(true);
        StartCoroutine(WaitForVFXToFinishBeforeDisable(vfxPrefab));


    }

    //Wait for the VFX animation to finish before disabling object
    IEnumerator WaitForVFXToFinishBeforeDisable(GameObject vfxPrefab)
    {
        yield return new WaitForSeconds(VFXAnimation.length);
        vfxPrefab.SetActive(false);


    }

    //Add new VFXPrefab to the pool
    void AddNewVFXPrefabToPool()
    {
        GameObject newVFX = Instantiate(VFXPrefabToUse);
        newVFX.SetActive(false);
        lightningVFXPool.Add(newVFX);

    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(cardSO.PlayerAnimation.length + VFXAnimation.length);
        gameObject.SetActive(false);
    }


    
}