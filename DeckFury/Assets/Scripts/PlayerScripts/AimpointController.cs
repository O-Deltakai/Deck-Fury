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
    Vector2 lastMousePosition;
    Vector2 screenCenter;

    [SerializeField] bool useKeyboardAiming;
    [SerializeField] bool _useRelativeAiming;
    public bool UseRelativeAiming => _useRelativeAiming;


    [SerializeField, Range(0.001f, 1)] float cursorSensitivity = 1;
    [SerializeField, Min(1f)] float maxCursorDistance = 5f;
    [SerializeField] Transform virtualCursorTransform;
    public Transform VirtualCursorTransform => virtualCursorTransform;
    [SerializeField] bool _clampVirtualCursor = true;



    [SerializeField] Vector2 _currentAimVector;
    public Vector2 CurrentAimVector => _currentAimVector;
    [SerializeField] Vector3 currentMouseDelta;

    Vector3 cursorOffset;



    void AimingStyleToggle(bool flag)
    {
        _useRelativeAiming = flag;
    }

    void SetCursorSensitivity(float value)
    {
        cursorSensitivity = value;
    }


    void Start()
    {
        AimingStyleToggle(SettingsManager.UseRelativeAiming);
        SettingsManager.OnChangeAimingStyle += AimingStyleToggle;

        SetCursorSensitivity(SettingsManager.CursorSensitivity);
        SettingsManager.OnChangeSensitivity += SetCursorSensitivity;


        cursorOffset = virtualCursorTransform.position - transform.position;

        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        mainCamera = GameManager.mainCamera;
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        cardPoolManager = CardPoolManager.Instance; 

        
        playerCardManager.OnRemoveCard += UpdateReticle;
        playerCardManager.OnLoadMagazine += UpdateReticle;
        playerController.OnPlayerDefeat += DisableAimpoint;

    }


    // Update is called once per frame
    void Update()
    {
        if(!GameManager.GameIsPaused && GameManager.currentGameState != GameManager.GameState.InMenu)
        {
            if(_useRelativeAiming)
            {
                Cursor.visible = false;
                virtualCursorTransform.gameObject.SetActive(true);
                FaceAimpointTowardsMouseRelative();
            }else
            {
                virtualCursorTransform.gameObject.SetActive(false);
                FaceAimpointTowardsMouse();
            }
        }

    }

    void OnDestroy()
    {
        SettingsManager.OnChangeAimingStyle -= AimingStyleToggle;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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

    public void AimTowardDirection(int x, int y)
    {
        if(x > 0 && y == 0)//Aim right
        {
            if(!freezeAimpoint)
            {
                transform.DORotate(new Vector3(0, 0, 90), 0.1f, RotateMode.Fast).SetUpdate(true);
            }

            currentAimDirection = AimDirection.Right;            
        }

        if(x < 0 && y == 0)//Aim left
        {
            if(!freezeAimpoint)
            {
                transform.DORotate(new Vector3(0, 0, -90), 0.1f, RotateMode.Fast).SetUpdate(true);
            }

            currentAimDirection = AimDirection.Left;            
        }

        if(x == 0 && y > 0)//Aim up
        {
            if(!freezeAimpoint)
            {
                transform.DORotate(new Vector3(0, 0, 180), 0.1f, RotateMode.Fast).SetUpdate(true);
            }

            currentAimDirection = AimDirection.Up;            
        }

        if(x == 0 && y < 0)//Aim down
        {
            if(!freezeAimpoint)
            {
                transform.DORotate(new Vector3(0, 0, 0), 0.1f, RotateMode.Fast).SetUpdate(true);
            }

            currentAimDirection = AimDirection.Down;            
        }



    }


    //Method for facing the aimpoint sprite towards the mouse within the constraints of the 4 cardinal directions.
    //Calculates the difference between the mouse position on screen space and the object position on screen space
    //and rotates the aimpoint if the appropriate conditions are passed.
    void FaceAimpointTowardsMouse()
    {
        
        mousePosition = Input.mousePosition;
        screenPointPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        //virtualCursorTransform.position = screenPointPosition;
        
        Vector2 aimPointDirection = (screenPointPosition - (Vector2)worldTransform.position).normalized;

        if (Mathf.Abs(aimPointDirection.x) > Mathf.Abs(aimPointDirection.y)) //Mouse position is greater horizontally than vertically
        {
            if (aimPointDirection.x > 0)
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 90), 0.1f, RotateMode.Fast).SetUpdate(true);
                }

                currentAimDirection = AimDirection.Right;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, -90), 0.1f, RotateMode.Fast).SetUpdate(true);
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
                    transform.DORotate(new Vector3(0, 0, 180), 0.1f, RotateMode.Fast).SetUpdate(true);
                }                
                currentAimDirection = AimDirection.Up;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 0), 0.1f, RotateMode.Fast).SetUpdate(true);
                }                
                currentAimDirection = AimDirection.Down;
            }
        }
        
    }

    void UpdateVirtualCursorRelative()
    {
        Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        currentMouseDelta = mouseDelta;

        cursorOffset += mouseDelta * cursorSensitivity;

        if(_clampVirtualCursor)
        {
            // Clamp the cursor offset to the maximum radius
            if (cursorOffset.magnitude > maxCursorDistance)
            {
                cursorOffset = cursorOffset.normalized * maxCursorDistance;
            }
        }


        virtualCursorTransform.position = transform.position + cursorOffset;
    }

    void FaceAimpointTowardsMouseRelative()
    {

        UpdateVirtualCursorRelative();

        Vector2 aimPointDirection = virtualCursorTransform.position - transform.position;

        aimPointDirection.Normalize();

        _currentAimVector = aimPointDirection;

        if (Mathf.Abs(aimPointDirection.x) > Mathf.Abs(aimPointDirection.y)) //Mouse position is greater horizontally than vertically
        {
            if (aimPointDirection.x > 0)
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 90), 0.1f, RotateMode.Fast).SetUpdate(true);
                }

                currentAimDirection = AimDirection.Right;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, -90), 0.1f, RotateMode.Fast).SetUpdate(true);
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
                    transform.DORotate(new Vector3(0, 0, 180), 0.1f, RotateMode.Fast).SetUpdate(true);
                }                
                currentAimDirection = AimDirection.Up;
            }
            else
            {
                if(!freezeAimpoint)
                {
                    transform.DORotate(new Vector3(0, 0, 0), 0.1f, RotateMode.Fast).SetUpdate(true);
                }                
                currentAimDirection = AimDirection.Down;
            }
        }

        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

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
                _clampVirtualCursor = true;

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
            _clampVirtualCursor = false;
        }else
        {
            freezeAimpoint = false;
            _clampVirtualCursor = true;
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
