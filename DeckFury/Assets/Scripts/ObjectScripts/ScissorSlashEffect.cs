using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ScissorSlashEffect : MonoBehaviour
{
    [SerializeField] BoxCollider2D rightCollider;
    [SerializeField] BoxCollider2D leftCollider;
    [SerializeField] LayerMask targetLayer;

    [SerializeField] GameObject rightSlashVFX;
    [SerializeField] AnimationEventIntermediary rightSlashAnimEventRelay;
    [SerializeField] GameObject leftSlashVFX;
    [SerializeField] AnimationEventIntermediary leftSlashAnimEventRelay;
    [SerializeField] AnimationClip slashVFXClip;

    [SerializeField] EventReference slashSFX;

    [Header("Camera Shake Settings")]
    [SerializeField] float shakeDuration;
    [SerializeField] Vector3 shakeVelocity;
    CinemachineImpulseSourceHelper impulseSourceHelper;

    public AttackPayload attackPayload;

    private void Awake()
    {
        impulseSourceHelper = GetComponent<CinemachineImpulseSourceHelper>();
        rightSlashVFX.SetActive(false);
        leftSlashVFX.SetActive(false);

        rightSlashAnimEventRelay.OnAnimationEvent += TriggerRightSlash;
        leftSlashAnimEventRelay.OnAnimationEvent += TriggerLeftSlash;
    }

    void OnEnable()
    {
        rightSlashVFX.SetActive(true);
        leftSlashVFX.SetActive(true);
        StartCoroutine(SelfDestruct(slashVFXClip.length));
    }


    void TriggerRightSlash()
    {
        impulseSourceHelper.ShakeCameraRandomCircle(shakeVelocity * SettingsManager.GlobalCameraShakeMultiplier, shakeDuration, 1f);
        RuntimeManager.PlayOneShot(slashSFX, rightSlashVFX.transform.position);
        Collider2D[] hits = Physics2D.OverlapBoxAll(rightCollider.transform.position, rightCollider.size, transform.eulerAngles.z, targetLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out StageEntity entity))
            {
                if(hit.CompareTag(TagNames.Enemy.ToString()) || hit.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                }
            }   
        }
    }

    void TriggerLeftSlash()
    {
        RuntimeManager.PlayOneShot(slashSFX, leftSlashVFX.transform.position);
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(leftCollider.transform.position, leftCollider.size, transform.eulerAngles.z, targetLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out StageEntity entity))
            {
                if(hitCollider.CompareTag(TagNames.Enemy.ToString()) || hitCollider.CompareTag(TagNames.EnvironmentalHazard.ToString()))
                {
                    entity.HurtEntity(attackPayload);
                }
            }
        }
    }

    IEnumerator SelfDestruct(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }


}
