using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using DG.Tweening;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class BubbleProjectile : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    [SerializeField] float projectileLifeTime;
    public AttackPayload attackPayload;

    [SerializeField] float envelopeDuration = 3f;
    float envelopeTimer = 0;


    [Header("Animations")]
    [SerializeField] AnimancerComponent animancer;
    [SerializeField] AnimationClip appearAnimation;
    [SerializeField] AnimationClip idleAnimation;
    [SerializeField] AnimationClip popAnimation;
    [SerializeField] ObjectRotater bubbleRotater;

    [Header("Tween Settings")]
    [SerializeField] GameObject bubbleSprite;
    [SerializeField] float envelopeScale = 0.75f;
    [SerializeField] Ease envelopeEase = Ease.OutBack;
    [SerializeField] float idleScale = 0.5f;

    [Header("SFX")]
    [SerializeField] EventReference bubbleAppearSFX;
    [SerializeField] EventReference bubbleEnvelopeSFX;
    [SerializeField] EventReference bubblePopSFX;

    StageEntity containedEntity;
    bool entityOriginallyHasElectricWeakness = false;

    Rigidbody2D rb2d;
    BoxCollider2D boxCollider2D;
    Coroutine CR_SelfDestructor = null;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }


    void Start()
    {
        animancer.Play(appearAnimation);
        CR_SelfDestructor = StartCoroutine(SelfDestructor());
    }

    void FixedUpdate()
    {
        rb2d.velocity = direction * speed;
    }

    void Update()
    {
        if(containedEntity != null)
        {
            if(envelopeTimer < envelopeDuration)
            {
                envelopeTimer += Time.deltaTime;
                containedEntity.CanAct = false;
                containedEntity.CanInitiateMovementActions = false;

                if(containedEntity.CenterPoint != null)
                {
                    transform.position = containedEntity.CenterPoint.position;
                }else
                {
                    transform.position = containedEntity.worldTransform.position;
                }

            }else
            {
                UntrapEntity();
                Pop();
            }
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag(TagNames.Wall.ToString()))
        {
            Pop();

        }

        if (other.gameObject.TryGetComponent(out StageEntity entity))
        {
            if(entity.CompareTag(TagNames.Enemy.ToString()))
            {
                entity.HurtEntity(attackPayload);
                animancer.Play(idleAnimation);
                EnvelopeEntity(entity);
            }
        }
    }

    void EnvelopeEntity(StageEntity entity)
    {
        bubbleRotater.enabled = false;
        bubbleSprite.transform.rotation = Quaternion.identity;


        if(CR_SelfDestructor != null)
        {
            StopCoroutine(CR_SelfDestructor);
            CR_SelfDestructor = null;
        }
        speed = 0;
        boxCollider2D.enabled = false;

        bubbleSprite.transform.DOScale(envelopeScale, 0.25f).SetEase(envelopeEase);

        if(entity.IsDead)
        {
            Pop();
            return;
        }

        containedEntity = entity;
        if(!entity.Weaknesses.Contains(AttackElement.Electric))
        {
            entity.Weaknesses.Add(AttackElement.Electric);
        }else
        {
            entityOriginallyHasElectricWeakness = true;
        }

        entity.OnFinishHurtEntity += Pop;
        containedEntity.OnBeginHurtEntity += UntrapEntity;
    }

    void UntrapEntity()
    {
        if(containedEntity != null)
        {
            containedEntity.CanAct = true;
            containedEntity.CanInitiateMovementActions = true;
            containedEntity.OnBeginHurtEntity -= UntrapEntity;
        }
    }

    void Pop()
    {
        bubbleRotater.enabled = false;
        bubbleSprite.transform.rotation = Quaternion.identity;

        speed = 0;
        boxCollider2D.enabled = false;

        if(containedEntity != null)
        {
            containedEntity.OnFinishHurtEntity -= Pop;

            if(!entityOriginallyHasElectricWeakness)
            {
                containedEntity.Weaknesses.Remove(AttackElement.Electric);
            }
        }

        containedEntity = null;

        animancer.Play(popAnimation);
        Destroy(gameObject, popAnimation.length);
    }


    IEnumerator SelfDestructor()
    {
        yield return new WaitForSeconds(projectileLifeTime);
        animancer.Play(popAnimation);
        yield return new WaitForSeconds(popAnimation.length);
        CR_SelfDestructor = null;
        Destroy(gameObject);
    }

}
