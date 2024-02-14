using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FreezeMine : MonoBehaviour
{
    BoxCollider2D mineCollider;

    public AttackPayload attackPayload;
    public bool objectIsPooled;

    [SerializeField] BoxCollider2D explosionRadius;
    [SerializeField] LayerMask targetLayer;

    [SerializeField] GameObject mineSprite;

    [SerializeField] AnimationEventIntermediary explosionAnimEventRelay;
    [SerializeField] AnimationClip explsoionVFXClip;
    [SerializeField] GameObject explosionVFX;
    [SerializeField] Light2D explosionLight;

    [Header("SFX")]
    [SerializeField] EventReference explosionSFX;
    [SerializeField] EventReference minePlacedSFX;
    [SerializeField] EventReference mineTriggeredSFX;


    [Header("Camera Shake")]
    [SerializeField] CinemachineImpulseSourceHelper impulseSourceHelper;
    [SerializeField] Vector2 cameraShakeVelocity;


    private void Awake() 
    {

        mineCollider = GetComponent<BoxCollider2D>();
        explosionVFX.SetActive(false);
        explosionLight.enabled = false;

        explosionAnimEventRelay.OnAnimationEvent += TriggerExplosion;
    }

    void OnEnable()
    {
        mineCollider.enabled = true;
        mineSprite.SetActive(true);
        RuntimeManager.PlayOneShot(minePlacedSFX, transform.position);
    }


    //What happens if an entity walks into the collider of the trap?
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag(TagNames.Enemy.ToString()) || other.gameObject.CompareTag(TagNames.EnvironmentalHazard.ToString()))
        {
            StageEntity entityHit = other.gameObject.GetComponent<StageEntity>();

            if (entityHit != null)
            {
                mineCollider.enabled = false;
                mineSprite.SetActive(false);

                RuntimeManager.PlayOneShot(mineTriggeredSFX, transform.position);

                TriggerVFX();
                StartCoroutine(SelfDestruct());
            }
        }

    }

    void TriggerVFX()
    {
        if(explosionVFX != null)
        {
            explosionVFX.SetActive(true);
        }


    }

    void TriggerExplosion()
    {
        explosionLight.enabled = true;
        DOTween.To(() => explosionLight.intensity, x => explosionLight.intensity = x, 0f, 0.25f).SetEase(Ease.InOutSine);   

        RuntimeManager.PlayOneShot(explosionSFX, transform.position);

        if(impulseSourceHelper)
        {
            impulseSourceHelper.ShakeCameraRandomCircle(cameraShakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, 0.25f, 1);
        }


        Collider2D[] hits = Physics2D.OverlapBoxAll(explosionRadius.transform.position, explosionRadius.size, explosionRadius.transform.eulerAngles.z, targetLayer);
        if(hits.Length == 0){return;}
        if(hits == null) { return; }

        var sortedEntities = hits.OrderBy(h => -Vector2.Distance(h.transform.position, transform.position)).ToList();
        

        foreach(var collider2D in sortedEntities) 
        {

            if(collider2D.TryGetComponent<StageEntity>(out StageEntity entity))
            {
                if(entity.CompareTag(TagNames.Enemy.ToString()) || entity.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                }
            }
        }
    
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(explsoionVFXClip.length);
        DisableObject();
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
