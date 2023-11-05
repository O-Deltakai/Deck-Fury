using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldBeacon : MonoBehaviour
{
    [SerializeField] AttackPayload attackPayload;

    [SerializeField] GameObject beaconObject;
    [SerializeField] BoxCollider2D beaconCollider;

    public PlayerController player;


    [SerializeField] GameObject shadow;
    public bool objectIsPooled;
    bool isExploding = false;

    public float restorePoint;
    public float LifeTime;
    public float restoreRate;
    public int maxPoint;

    private void Awake()
    {
        beaconObject.transform.DOLocalRotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360).SetLoops(200, LoopType.Restart).SetEase(Ease.Linear).SetUpdate(false);   
        StartCoroutine(TimedDestruction());
        StartCoroutine(ShieldRestore());
    }

    private IEnumerator TimedDestruction()
    {
        yield return new WaitForSeconds(LifeTime);
        DisableObject();
    }

    
    private IEnumerator ShieldRestore()
    {
        while(true){
            yield return new WaitForSeconds(restoreRate);
            if(player.CurrentHP>0){
                /*
                Vector3 distance = gameObject.transform.position - player.currentTilePosition;
                if(distance.x+distance.y<=2.0f){
                    player.ShieldHP+=(int)restorePoint;
                    maxPoint -= (int)restorePoint;
                }
                */

                int stageEntitiesLayer = LayerMask.NameToLayer("StageEntities");
                LayerMask stageEntitiesMask = 1 << stageEntitiesLayer;

                Collider2D[] hits = Physics2D.OverlapBoxAll(beaconCollider.transform.position, beaconCollider.size*gameObject.transform.localScale, 0, stageEntitiesMask);

                foreach(Collider2D collider2D in hits) 
                {
                    StageEntity entityHit = collider2D.gameObject.GetComponent<StageEntity>();

                    if(entityHit == null)
                    {
                        print("collider did not have a StageEntity attached");
                        continue;
                    }

                    if(entityHit.CompareTag("Player"))
                    {
                        player.ShieldHP+=(int)restorePoint;
                        maxPoint -= (int)restorePoint;
                    }             
                }
            }
            if(maxPoint==0){
                DisableObject();
            }
        }
    }

    void DisableObject()
    {
        
        if(objectIsPooled)
        {
            gameObject.SetActive(false);
        }else
        {
            Destroy(gameObject);
        }

    }

}
