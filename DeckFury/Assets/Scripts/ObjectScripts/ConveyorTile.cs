using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tile object that forcefully moves an entity towards a cardinal direction
/// </summary>
public class ConveyorTile : MonoBehaviour
{

    public enum Direction { Up, Down, Left, Right }
    public Direction direction;
    /// <summary>
    /// Defines how far the entity will be moved
    /// </summary>
    [Range(0, 10)] public int force;

    [SerializeField] GameObject spriteObject;

    // Start is called before the first frame update
    void Start()
    {
        RotateTowardsDirection(direction);
    }

    public void RotateTowardsDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        collider2D.gameObject.TryGetComponent<StageEntity>(out StageEntity entity);

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
