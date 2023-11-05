using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;


public class SeekerAI : MonoBehaviour
{
    public NPC attacker;
    public StageEntity Target;
    public Ease movementType;
    public float movementTime;
    public float waitTime;
    public bool pauseAI = false;
    private bool isMoving = false;
    private Vector3Int moveLocation;

    private void Awake() 
    {
        if(!attacker)
        {
            attacker = GetComponent<NPC>();
        }    
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3Int> pathPositions = null;
        if(pauseAI){return;}
        if(!attacker.CanInitiateMovementActions){return;}

        if (!isMoving)
        {
            isMoving = true;

            pathPositions = Pathfinder.FindPath(
                StageManager.Instance.GroundTileMap,
                StageManager.Instance,
                attacker.currentTilePosition,
                Target.currentTilePosition
            );

            //Debug.Log(pathPositions);

            if (pathPositions != null)
            {
                if (pathPositions.Count >= 2)
                {
                    moveLocation = pathPositions[1];
                    StartCoroutine(MoveTowardsTarget());
                }
                else
                {
                    StartCoroutine(CoolDown());
                }
            }
            else
            {
                Vector3Int nextPositiion = new Vector3Int(99, 99, 0);
                Vector3Int playerPosition = Target.currentTilePosition;

                foreach (Vector3Int direction in VectorDirections.Vector3IntAll)
                {
                    Vector3Int temp = playerPosition + direction;
                    if (StageManager.Instance.CheckValidTile(temp))
                    {
                        nextPositiion = temp;
                        break;
                    }
                }

                if (!nextPositiion.Equals(new Vector3Int(99, 99, 0)))
                {
                    pathPositions = Pathfinder.FindPath(
                        StageManager.Instance.GroundTileMap,
                        StageManager.Instance,
                        attacker.currentTilePosition,
                        nextPositiion
                    );

                    if (pathPositions != null)
                    {
                        if (pathPositions.Count >= 2)
                        {
                            moveLocation = pathPositions[1];
                            StartCoroutine(MoveTowardsTarget());
                        }
                        else
                        {
                            StartCoroutine(CoolDown());
                        }
                    }
                    else
                    {
                        MakeRandomMove();
                    }
                }
                else
                {
                    MakeRandomMove();
                }
            }
        }
    }

    private void MakeRandomMove()
    {
        Vector3Int nextPositiion = new Vector3Int(99, 99, 0);

        Vector3Int[] cardinalDirections = VectorDirections.Vector3IntCardinal.OrderBy(x => Random.value).ToArray();
        foreach (Vector3Int direction in cardinalDirections)
        {
            Vector3Int temp = attacker.currentTilePosition + direction;
            if (StageManager.Instance.CheckValidTile(temp))
            {
                nextPositiion = temp;
                break;
            }
        }

        if (!nextPositiion.Equals(new Vector3Int(99, 99, 0)))
        {
            moveLocation = nextPositiion;
            StartCoroutine(MoveTowardsTarget());
        }
        else
        {
            StartCoroutine(CoolDown());
        }
    }

    protected IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        isMoving = false;
    }

    protected IEnumerator MoveTowardsTarget()
    {
        Vector3Int diff = moveLocation - attacker.currentTilePosition;
        yield return attacker.TweenMove(diff.x, diff.y, movementTime, movementType);
        float randomFloat = Random.Range(waitTime * -0.5f, waitTime * 0.4f);
        yield return new WaitForSeconds(waitTime + randomFloat);
        isMoving = false;
    }
}
