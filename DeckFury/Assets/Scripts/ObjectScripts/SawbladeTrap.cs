using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SawbladeTrap : MonoBehaviour
{
    public enum TravelMode { Clockwise, CounterClockwise, PingPong }

    public TravelMode travelMode = TravelMode.Clockwise;

    [Tooltip("The order of nodes the sawblade will travel to.")]
    public List<Transform> pathNodes = new();

    [Tooltip("The speed at which the sawblade will travel.")]
    public float speed = 5f;

    [SerializeField] GameObject sawbladeSprite;
    [SerializeField] GameObject spriteMask;

[Header("Raycast Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float raycastDistance = 0.5f;


[Header("Sawblade Rotation Settings")]
    Tween sawbladeRotationTween;
    [SerializeField] Vector3 sawbladeRotation = new Vector3(0, 0, 359);
    [SerializeField] float sawbladeRotationSpeed = 1f;

[Header("Damage Settings")]
    public AttackPayload attackPayload;

    Tween rotationTween;
    int currentNodeIndex = 0;

    private void OnDrawGizmos()
    {
        if (pathNodes.Count == 0) return;

        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (i < pathNodes.Count - 1)
            {
                Gizmos.DrawLine(pathNodes[i].position, pathNodes[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(pathNodes[i].position, pathNodes[0].position);
            }
        }

        // Draw a ray in the direction the sawblade is traveling
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * raycastDistance);

    }

    void Start()
    {
        RotateSawblade();
    }

    // Update is called once per frame
    void Update()
    {
        MoveAlongPath();
        //DetectWall();
    }

    void DetectWall()
    {
        // Fire a raycast in the direction the sawblade is traveling
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, raycastDistance, targetLayer);

        if(hit.collider != null)
        {
            // If the raycast hits an object on the target layer, rotate the sawblade 90 degrees
            if (hit.collider.CompareTag(TagNames.Wall.ToString()))
            {
                if(!rotationTween.IsActive())
                {
                    rotationTween = transform.DORotate(new Vector3(0, 0, transform.rotation.eulerAngles.z + 90), 0.1f).SetEase(Ease.Linear);
                }
                //transform.Rotate(0, 0, 90);
            }        
        }

    }

    void RotateSawblade()
    {
        sawbladeRotationTween = sawbladeSprite.transform.DORotate(sawbladeRotation, sawbladeRotationSpeed, RotateMode.Fast).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    void MoveAlongPath()
    {
        if (pathNodes.Count == 0) return;

        Transform targetNode = pathNodes[currentNodeIndex];
        Vector3 direction = (targetNode.position - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;

        if (Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            currentNodeIndex++;
            if (currentNodeIndex >= pathNodes.Count)
            {
                currentNodeIndex = 0; // Loop back to the start of the path
                // Or you can stop the movement by removing or disabling this script
            }
        }        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<StageEntity>(out StageEntity entity))
        {
            entity.HurtEntity(attackPayload);
        }
    }


}
