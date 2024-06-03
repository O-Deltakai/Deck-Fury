using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// A targeting algorithm that finds the closest tile to the mouse cursor within a certain radius
/// <summary>
public class TargetClosestTile : MonoBehaviour
{
    [SerializeField] float searchInterval = 0.1f;

    [SerializeField] GroundTileData _targetTile;
    [SerializeField] Vector3 _targetPosition;
    public GroundTileData TargetTile { get => _targetTile; }
    public Vector3 TargetPosition { get => _targetPosition; }

    [Header("Reticle Settings")]
    [SerializeField] GameObject reticleSprite;
    public Transform virtualCursor;
    [SerializeField] bool useVirtualCursor = false;

    StageManager stageManager;
    float searchTimer = 0f;
    Tween reticleTween;

    void Awake()
    {
        SettingsManager.OnChangeAimingStyle += (bool value) => useVirtualCursor = value;
    }


    void Start()
    {
        stageManager = StageManager.Instance;        
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
            FindClosestGroundTileData();
            TweenReticleToPosition(_targetPosition);
        }
    }

    void FindClosestGroundTileData()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        _targetTile = stageManager.FindClosestGroundTileData(mousePos);
        _targetPosition = _targetTile.worldPosition;
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


}
