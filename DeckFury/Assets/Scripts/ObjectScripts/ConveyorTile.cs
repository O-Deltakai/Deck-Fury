using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tile object that forcefully moves an entity towards a cardinal direction
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class ConveyorTile : MonoBehaviour
{

    public enum Direction { Up, Down, Left, Right }
    public Direction direction;

    [Tooltip("How far the entity will be moved in the direction of the conveyor tile.")]
    [Range(0, 10)] public int force;

    [Tooltip("The target layer of which objects with StageEntity will be affected by the conveyor tile.")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject spriteObject;

    BoxCollider2D conveyorCollider;
    bool occupied = false;

    // Start is called before the first frame update
    void Start()
    {
        conveyorCollider = GetComponent<BoxCollider2D>();
        RotateTowardsDirection(direction);
    }

    void OnValidate()
    {
        RotateTowardsDirection(direction);
    }

    public void RotateTowardsDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Direction.Down:
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.Left:
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Right:
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        occupied = true;
        ShoveEntity();
        if(occupied)
        {
            StartCoroutine(ShoveTimer(0.25f));
        }        
    }

    void ShoveEntity()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(conveyorCollider.transform.position, conveyorCollider.size, conveyorCollider.transform.eulerAngles.z, targetLayer);
        if(hits.Length == 0){return;}
        if(hits == null) { return; }
        foreach (var collider in hits)
        {
            collider.gameObject.TryGetComponent<StageEntity>(out StageEntity entity);

            if (entity == null)
            {
                return;
            }else
            {

                Vector2Int shoveDirection = Vector2Int.zero;
                switch (direction)
                {
                    case Direction.Up:
                        shoveDirection = new Vector2Int(0, force);
                        break;
                    case Direction.Down:
                        shoveDirection = new Vector2Int(0, -force);
                        break;
                    case Direction.Left:
                        shoveDirection = new Vector2Int(-force, 0);
                        break;
                    case Direction.Right:
                        shoveDirection = new Vector2Int(force, 0);
                        break;
                }
                entity.AttemptMovement(shoveDirection.x, shoveDirection.y, 0.15f, DG.Tweening.Ease.OutQuart, ForceMoveMode.Forward);         
                
            }            
        }


    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        occupied = false;
    }

    IEnumerator ShoveTimer(float delay)
    {
        while(occupied)
        {
            yield return new WaitForSeconds(delay);
            ShoveEntity();
        }
        
    }


}
