using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlasmaSlashEffect : MonoBehaviour
{
    [SerializeField] List<BoxCollider2D> hitboxes;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Light2D slashLight;
    [SerializeField] AnimationClip slashAnimation;
    [SerializeField] AnimationEventIntermediary slashAnimEventRelay;
    public AttackPayload attackPayload;

    [Header("SFX")]
    [SerializeField] EventReference slashVFX;

    [Header("Camera Shake Settings")]
    [SerializeField] Vector3 cameraShakeVelocity = new Vector3(0.05f, 0.05f, 0f);
    [SerializeField] float cameraShakeDuration = 0.1f;
    CinemachineImpulseSourceHelper impulseSourceHelper;

    void Awake()
    {
        slashAnimEventRelay.OnAnimationEvent += TriggerHitbox;
        impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
    }


    void Start()
    {
        StartCoroutine(SelfDisable());
    }

    void TriggerHitbox()
    {
        impulseSourceHelper.ShakeCameraRandomCircle(cameraShakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, cameraShakeDuration, 1f);
        RuntimeManager.PlayOneShot(slashVFX, transform.position);
        slashLight.enabled = true;
        DOTween.To(() => slashLight.intensity, x => slashLight.intensity = x, 0f, 0.25f).SetEase(Ease.InOutSine);

        List<Collider2D> hits = new();

        foreach(BoxCollider2D hitbox in hitboxes)
        {
            hits.AddRange(Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.size, transform.eulerAngles.z, targetLayer));
        }

        if(hits.Count == 0)
        {
            return;
        }

        foreach (Collider2D hit in hits)
        {

            if(hit.gameObject.TryGetComponent<StageEntity>(out var entity))
            {
                if(hit.CompareTag(TagNames.Enemy.ToString()) || hit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                }

            }
        }
    }


    IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(slashAnimation.length);
        Destroy(gameObject);
    }


}
