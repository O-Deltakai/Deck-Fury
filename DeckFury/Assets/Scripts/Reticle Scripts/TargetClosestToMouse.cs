using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// A targeting algorithm that finds the closest target to the mouse cursor within a certain radius
/// </summary>
public class TargetClosestToMouse : MonoBehaviour
{
    [SerializeField] float searchInterval = 0.1f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] CircleCollider2D searchRadius;
    [SerializeField] GameObject reticleSprite;

    public Transform virtualCursor;
    [SerializeField] bool useVirtualCursor = false;

    [SerializeField] StageEntity target;
    public StageEntity Target { get { return target; } }

    private float searchTimer = 0f;

    Tween reticleTween;

    void Awake()
    {
        SettingsManager.OnChangeAimingStyle += (bool value) => useVirtualCursor = value;
    }

    void OnEnable()
    {
        EventBus<RelayGameObjectEvent>.Raise(new RelayGameObjectEvent { gameObject = gameObject});
        virtualCursor = GameManager.Instance.player.aimpoint.VirtualCursorTransform;

    }


    // Update is called once per frame
    void Update()
    {
        searchTimer += Time.unscaledDeltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;

            MoveColliderToCursor();

            target = FindClosestTarget();
            if(target != null)
            {
                reticleSprite.SetActive(true);
                TweenReticleToPosition(target.worldTransform.position);
            }else
            {
                reticleSprite.SetActive(false);
            }
        }
    }

    void TweenReticleToPosition(Vector3 position)
    {
        //Check the distance between the reticle and the position and if it's too close, don't tween
        if(Vector3.Distance(reticleSprite.transform.position, position) < 0.01f)
        {
            reticleSprite.transform.position = position;
            return;
        }

        if (reticleTween.IsActive())
        {
            reticleTween.Kill();
        }

        reticleTween = reticleSprite.transform.DOMove(position, 0.15f).SetEase(Ease.OutExpo).SetUpdate(true);
    }

    void MoveColliderToCursor()
    {
        Vector3 cursorPosition;
        if(useVirtualCursor && virtualCursor)
        {
            cursorPosition = virtualCursor.position;
            cursorPosition.z = 0;
            searchRadius.transform.position = cursorPosition;
            return;
        }

        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0;
        searchRadius.transform.position = cursorPosition;
    }


    StageEntity FindClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(searchRadius.transform.position, searchRadius.radius, targetLayer);
        if (colliders.Length == 0)
        {
            return null;
        }

        StageEntity closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if(!collider.CompareTag(TagNames.Enemy.ToString()))
            {
                continue;
            }
            if (!collider.TryGetComponent<StageEntity>(out var entity))
            {
                continue;
            }

            float distance = Vector2.Distance(searchRadius.transform.position, entity.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = entity;
            }
        }

        return closestTarget;
    }

}
