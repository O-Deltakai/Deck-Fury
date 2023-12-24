using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ExplosiveBarrel : StageEntity
{

    [SerializeField] BoxCollider2D explosionCollider;
    [SerializeField] BoxCollider2D barrelCollider;

    [SerializeField] AttackPayload attackPayload;

    [SerializeField] GameObject shadow;
    [SerializeField] float fuseTimer = 0.5f;

    [SerializeField] EventReference explosionSFX;

    bool isExploding = false;

    protected override void Awake()
    {
        base.Awake();
        barrelCollider = GetComponent<BoxCollider2D>();
        explosionCollider.enabled = false;
        OnHPChanged += ExplodeBarrel;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SetPositionTimer());
    }

    IEnumerator SetPositionTimer()
    {
        yield return new WaitForEndOfFrame();
        _stageManager = StageManager.Instance;

        currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);
        _stageManager.SetTileEntity(this, currentTilePosition);
    }


    void ExplodeBarrel(int oldValue, int newValue)
    {
        barrelCollider.enabled = false;
        StartCoroutine(statusEffectManager.FlashColor(Color.red, fuseTimer, 0.075f));
        StartCoroutine(ExplosionTimer());
    }

    IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(fuseTimer+0.05f);

        RuntimeManager.PlayOneShot(explosionSFX, transform.position);

        isExploding = true;
        explosionCollider.enabled = true;
        shadow.SetActive(false);
        StartCoroutine(ExplosionColliderTimer());
        StartCoroutine(DestroyEntity());

    }

    IEnumerator ExplosionColliderTimer()
    {
        yield return new WaitForSeconds(0.1f);
        explosionCollider.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(!isExploding){return;}
        StageEntity entityHit = other.gameObject.GetComponent<StageEntity>();
        
        if(entityHit != null)
        {
            print("Name of entity detected: "+ entityHit.gameObject.name);
            entityHit.HurtEntity(attackPayload);
        }  

    }




}
