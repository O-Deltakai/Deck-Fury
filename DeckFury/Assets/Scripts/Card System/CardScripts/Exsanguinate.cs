using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSourceHelper))]
public class Exsanguinate : CardEffect
{
    [SerializeField] GameObject VFXPrefabToUse; //Set in inspector

    //Set this in the inspector (The prefabs in here should be the same ones that are attached on the EffectPrefab itself)
    [SerializeField] List<GameObject> exsanguinateVFXPool = new List<GameObject>();
    [SerializeField] LayerMask stageEntitiesMask;

    [Header("SFX Settings")]
    [SerializeField] EventReference exsanguinateSFX;

    [Header("Camera Shake Settings")]
    [SerializeField] Vector3 cameraShakeVelocity;
    [SerializeField] float cameraShakeDuration;
    CinemachineImpulseSourceHelper cinemachineImpulseSourceHelper;

    protected override void Awake()
    {
        base.Awake();

        cinemachineImpulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
    }


    public override void ActivateCardEffect()
    {
        List<StageEntity> entities = new();
        int numberOfBleedingEnemies = 0;

        //Just creates a big box around the player and checks for any entities in that box
        Collider2D[] hits = Physics2D.OverlapBoxAll(player.worldTransform.position, new Vector2(20, 20), 0, stageEntitiesMask);
        foreach(var hit in hits)
        {
            if(hit.gameObject.CompareTag(TagNames.Enemy.ToString()) && hit.TryGetComponent(out StageEntity stageEntity))
            {
                entities.Add(stageEntity);
            }
        }

        while(entities.Count > exsanguinateVFXPool.Count)
        {
            AddNewVFXPrefabToPool();
        }

        int index = 0;  
        foreach(var entity in entities)
        {
            if(entity.StatusManager.Bleeding)
            {
                entity.StatusManager.Exsanguinate();

                if(entity.CenterPoint)
                {
                    exsanguinateVFXPool[index].transform.position = entity.CenterPoint.position;
                }else
                {
                    exsanguinateVFXPool[index].transform.position = entity.currentTilePosition;
                }

                exsanguinateVFXPool[index].SetActive(true);
                index++;

                entity.HurtEntity(attackPayload);
                numberOfBleedingEnemies++;
                RuntimeManager.PlayOneShotAttached(exsanguinateSFX, entity.gameObject);
            }
        }
        
        cinemachineImpulseSourceHelper.ShakeCameraRandomCircle(numberOfBleedingEnemies * 0.5f * SettingsManager.GlobalCameraShakeMultiplier * cameraShakeVelocity,
        cameraShakeDuration, 1f);


    }

    void AddNewVFXPrefabToPool()
    {
        GameObject newVFX = Instantiate(VFXPrefabToUse, gameObject.transform);
        newVFX.SetActive(false);
        exsanguinateVFXPool.Add(newVFX);
    }

    protected override IEnumerator DisableEffectPrefab()
    {
        yield break;
    }

}
