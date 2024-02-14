using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetClosestToMouse : MonoBehaviour
{
    [SerializeField] float searchInterval = 0.1f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] CircleCollider2D searchRadius;
    [SerializeField] GameObject reticleSprite;

    [SerializeField] StageEntity target;
    public StageEntity Target { get { return target; } }

    private float searchTimer = 0f;

    Tween reticleTween;


    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        EventBus<RelayGameObjectEvent>.Raise(new RelayGameObjectEvent { gameObject = gameObject});      
        print("Raised event");  

    }


    // Update is called once per frame
    void Update()
    {
        searchTimer += Time.unscaledDeltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;

            MoveColliderToMousePosition();

            target = FindClosestTarget();
            if(target != null)
            {
                TweenReticleToPosition(target.worldTransform.position);
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

    void MoveColliderToMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        searchRadius.transform.position = mousePosition;
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
