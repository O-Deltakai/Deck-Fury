using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//By default, 0 rotation means the object should be facing DOWN
public enum AimDirection
{
    Up = 180,
    Down = 0,
    Left = -90,
    Right = 90,
    TopRight = 135,
    TopLeft = -135,
    BottomRight = 45,
    BottomLeft = -45
}

public class AimpointController : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] Transform worldTransform;
    CardPoolManager cardPoolManager;
    [SerializeField] PlayerCardManager playerCardManager;
    [SerializeField] PlayerController playerController;

    [field:SerializeField] public GameObject DefaultAimPoint {get; private set;}
    public bool freezeAimpoint = false;

    [field:SerializeField] public GameObject TargetingReticleAnchor {get; private set;}
    [SerializeReference] GameObject CurrentActiveReticle;



    public AimDirection currentAimDirection {get; private set;}

    Vector2 mousePosition;
    Vector2 screenPointPosition;
    Vector2 aimpointMouseDifference;

    void Start()
    {
        mainCamera = GameManager.mainCamera;


        cardPoolManager = CardPoolManager.Instance; 

        
        playerCardManager.OnRemoveCard += UpdateReticle;
        playerCardManager.OnLoadMagazine += UpdateReticle;
        playerController.OnPlayerDefeat += DisableAimpoint;

    }


    // Update is called once per frame
    void Update()
    {
        if(!GameManager.GameIsPaused)
        {
            FaceAimpointTowardsMouse();
        }


    }

    void DisableAimpoint()
    {
        gameObject.SetActive(false);
    }

    public Vector3Int GetAimVector3Int()
    {
        return currentAimDirection switch
        {
            AimDirection.Up => new Vector3Int(0, 1, 0),
            AimDirection.Down => new Vector3Int(0, -1, 0),
            AimDirection.Left => new Vector3Int(-1, 0, 0),
            AimDirection.Right => new Vector3Int(1, 0, 0),
            _ => new Vector3Int(0, 0, 0),
        };
    }
    public Vector2Int GetAimVector2Int()
    {
        return currentAimDirection switch
        {
            AimDirection.Up => new Vector2Int(0, 1),
            AimDirection.Down => new Vector2Int(0, -1),
            AimDirection.Left => new Vector2Int(-1, 0),
            AimDirection.Right => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 0),
        };
    }


    public Vector3 GetAimRotation()
    {
        return currentAimDirection switch
        {
            AimDirection.Up => new Vector3(0, 0, 180),
            AimDirection.Down => new Vector3(0, 0, 0),
            AimDirection.Left => new Vector3(0, 0, -90),
            AimDirection.Right => new Vector3(0, 0, 90),
            _ => new Vector3(0, 0, 0),
        };        
    }


    //Method for facing the aimpoint sprite towards the mouse within the constraints of the 4 cardinal directions.
    //Calculates the difference between the mouse position on screen space and the object position on screen space
    //and rotates the aimpoint if the appropriate conditions are passed.
    void FaceAimpointTowardsMouse()
    {
        
        mousePosition = Input.mousePosition;
        screenPointPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        
        Vector2 aimPointDirection = (screenPointPosition - (Vector2)worldTransform.position).normalized;
        

        if (Mathf.Abs(aimPointDirection.x) > Mathf.Abs(aimPointDirection.y)) //Mouse position is greater horizontally than vertically
        {
            if (aimPointDirection.x > 0)
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 90), 0.1f, RotateMode.Fast);
                }

                currentAimDirection = AimDirection.Right;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, -90), 0.1f, RotateMode.Fast);
                }
                currentAimDirection = AimDirection.Left;
            }
        }
        else
        {
            if (aimPointDirection.y > 0)
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 180), 0.1f, RotateMode.Fast);
                }                
                currentAimDirection = AimDirection.Up;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 0), 0.1f, RotateMode.Fast);
                }                
                currentAimDirection = AimDirection.Down;
            }
        }
        
    }

    

    void ActivateCardReticle(CardObjectReference card)
    {
        if(!card.cardSO.UseTargetingReticle || card.cardSO.TargetingReticle == null)
        {
            if(CurrentActiveReticle != null)
            {
                CurrentActiveReticle.SetActive(false);
                DefaultAimPoint.SetActive(true);
                freezeAimpoint = false;
            }

            return;
        }

        if(CurrentActiveReticle != null)
        {
            CurrentActiveReticle.SetActive(false);
        }


        DefaultAimPoint.SetActive(false);

        CurrentActiveReticle = cardPoolManager.GetReticleFromPool(card.cardSO.TargetingReticle);
        AdjustReticleOffset(card.cardSO.TargetingReticleOffSet);
        if(card.cardSO.ReticleIsStatic)
        {
            freezeAimpoint = true;
        }else
        {
            freezeAimpoint = false;
        }

        CurrentActiveReticle.SetActive(true);
        
    }

    void UpdateReticle()
    {
        if(playerCardManager.CardMagazine.Count == 0)
        {
            if(CurrentActiveReticle != null)
            {
                CurrentActiveReticle.SetActive(false);
            }
            DefaultAimPoint.SetActive(true);
            freezeAimpoint = false;
            return;            
        }

        ActivateCardReticle(playerCardManager.CardMagazine[0]);

    }

    void AdjustReticleOffset(Vector2Int offset)
    {
        TargetingReticleAnchor.transform.localPosition = new Vector3(offset.x, offset.y);
    }


}
