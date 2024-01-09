using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;
using FMODUnity;

[RequireComponent(typeof(InputBufferHandler))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerCardManager))]
public class PlayerController : StageEntity
{

    public delegate void BasicAttackEventHandler();
    public event BasicAttackEventHandler OnBasicAttack;

    public delegate void PlayerDefeatEvent();
    public event PlayerDefeatEvent OnPlayerDefeat;

    public bool CanFireBasicShot = true;

    PlayerDataContainer playerData;
    [SerializeField] Camera mainCamera;
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [field:SerializeField] public AimpointController aimpoint{get;private set;}
    [SerializeField] GameObject basicShotProjectile;
    [SerializeField] int basicShotDamage;


    PlayerDashController dashController;
    PlayerAnimationController animationController;
    PlayerCardManager cardManager;


    public BoxCollider2D playerCollider{get; private set;}
    //implement for energy system
    GameObject energyBar;
    EnergyController energyController;
    //end for energy system

    InputBufferHandler bufferHandler;


    Vector2 mousePosition;
    Vector2 screenPointPosition;
    Vector2 playerMouseDifference;

    bool isDefeated;

    [SerializeField] bool useInputSystemMovement;
    [SerializeField] Vector2 currentInputVector;
    [SerializeField] Vector2 currentAimVector;

[Header("Player SFX")]
    [SerializeField] EventReference basicShotSFX;


    [SerializeField] float automoveCooldown = 0.2f;
    [SerializeField] bool canAutoMove = true;
    Coroutine CR_AutomoveTimer;

    protected override void Awake()
    {
        base.Awake();
        animationController = GetComponent<PlayerAnimationController>();
        cardManager = GetComponent<PlayerCardManager>();
        playerCollider = GetComponent<BoxCollider2D>();
        dashController = GetComponent<PlayerDashController>();
        bufferHandler = GetComponent<InputBufferHandler>();
        
        //implement for energy system
        energyController = FindObjectOfType<EnergyController>();
        if(energyController == null){Debug.LogWarning("EnergyController could not be found in the scene, energy system will not function.");}
        //end for energy system

        if(cardSelectionMenu == null)
        {
            cardSelectionMenu = FindObjectOfType<CardSelectionMenu>();
            if(cardSelectionMenu == null)
            {
                Debug.LogError("Card Selection Menu object could not be found in the scene, you will unable to open or use cards!"+
                "Make sure the correct UI Prefab is being used for the scene.");
            }
        }
        if(mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }


    }


    void Update()
    {
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}


        SimpleMove();

        if(aimpoint.UseRelativeAiming)
        {
            FacePlayerTowardsCursor();
        }else
        {
            FacePlayerTowardsMouse();
        }

 

    }

    /// <summary>
    /// Takes a given PlayerDataContainer and sets all relevant values on the player to this PlayerDataContainer
    /// </summary>
    /// <param name="givenPlayerData"></param>
    void SetPlayerValuesToPlayerData(PlayerDataContainer givenPlayerData)
    {
        CurrentHP = givenPlayerData.CurrentHP;
        ShieldHP = givenPlayerData.BaseShieldHP;

        defense = givenPlayerData.BaseDefense;
        armor = givenPlayerData.BaseArmor;

        playerData = givenPlayerData;

    }

    void SaveCurrentPlayerValuesToPlayerData()
    {
        playerData.CurrentHP = CurrentHP;

    }

    //Calculates the difference between the position of the mouse on the screen and the screen position of the player character
    //and flips the x-axis of the player model in order to face the mouse position.
    void FacePlayerTowardsMouse()
    {
        if(!CanAct){return;}
        mousePosition = Input.mousePosition;
        screenPointPosition = mainCamera.WorldToScreenPoint(worldTransform.position);
        playerMouseDifference = screenPointPosition - mousePosition;
        
        if(playerMouseDifference.x > 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            
        }else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            
        }
    }

    /// <summary>
    /// Faces the player towards the virtual cursor if relative aiming is enabled
    /// </summary>
    void FacePlayerTowardsCursor()
    {
        if(!CanAct){return;}

        if (aimpoint.CurrentAimVector.x > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }else
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

    }


    //Simple movement system - checks for keyboard input for WASD and then starts a coroutine which calls the IEnumerator
    //TweenMove on its inherited class (StageEntity) to move the entity 1 tile space in the cardinal directions.
    //Set MovingCoroutine to be the TweenMove coroutine whenever a key is pressed - TweenMove will return if MovingCoroutine is not null.
    //TweenMove will automatically set MovingCoroutine to be null once its duration is completed.
    void SimpleMove()
    {
        if(!CanInitiateMovementActions){return;}

        if(MovingCoroutine != null) { return; }
        if(cardManager.CardInUseCoroutine != null) { return; }

        if(Keyboard.current.dKey.wasPressedThisFrame)
        {
            canAutoMove = false;
            if(CR_AutomoveTimer != null)
            {
                StopCoroutine(CR_AutomoveTimer);
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }else
            {
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }
            
            //Move right
            MovingCoroutine = StartCoroutine(TweenMove(1, 0, 0.1f, MovementEase));
        }
        if(Keyboard.current.aKey.wasPressedThisFrame)
        {
            canAutoMove = false;
            if(CR_AutomoveTimer != null)
            {
                StopCoroutine(CR_AutomoveTimer);
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }else
            {
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }
            //Move left
           MovingCoroutine = StartCoroutine(TweenMove(-1, 0, 0.1f, MovementEase)); 
        }
        if(Keyboard.current.wKey.wasPressedThisFrame)
        {
            canAutoMove = false;
            if(CR_AutomoveTimer != null)
            {
                StopCoroutine(CR_AutomoveTimer);
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }else
            {
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }
            //Move up
            MovingCoroutine = StartCoroutine(TweenMove(0, 1, 0.1f, MovementEase));     
        }
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            canAutoMove = false;
            if(CR_AutomoveTimer != null)
            {
                StopCoroutine(CR_AutomoveTimer);
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }else
            {
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }
            //Move down
            MovingCoroutine = StartCoroutine(TweenMove(0, -1, 0.1f, MovementEase));
        }



        if(canAutoMove)
        {
            if(Keyboard.current.dKey.isPressed)
            {
                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }

                //Move right
                MovingCoroutine = StartCoroutine(TweenMove(1, 0, 0.1f, MovementEase));
            }
            if(Keyboard.current.aKey.isPressed)
            {
                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }

                //Move left
                MovingCoroutine = StartCoroutine(TweenMove(-1, 0, 0.1f, MovementEase)); 
            }
            if(Keyboard.current.wKey.isPressed)
            {
                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }
                //Move up
                MovingCoroutine = StartCoroutine(TweenMove(0, 1, 0.1f, MovementEase));     
            }
            if(Keyboard.current.sKey.isPressed)
            {
                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }
                //Move down
                MovingCoroutine = StartCoroutine(TweenMove(0, -1, 0.1f, MovementEase));
            }
        }




    }

    IEnumerator AutomoveTimer(float duration)
    {
        canAutoMove = false;
        yield return new WaitForSeconds(duration);
        canAutoMove = true;
        CR_AutomoveTimer = null;
    }




//Place all methods that relate to the input action asset on the Player Input component here.
#region Input Actions

    //TODO: method to check if player input can be used for better modularity
    bool CanUsePlayerInput()
    {
        if(GameManager.GameIsPaused){ return false; }
        if(isDefeated){return false;}


        return true;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if(!useInputSystemMovement) { return; }

        currentInputVector = context.ReadValue<Vector2>();
        
        
    }


    //Logic for when BasicShot input action is activated
    public void BasicShot(InputAction.CallbackContext context)
    {
        if(cardManager.CardInUseCoroutine != null) { return; }
        if(MovingCoroutine != null){return;}
        if(!CanFireBasicShot){return;}
        if(!CanAct){return;}
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}

        if(context.performed)
        {
            //animationController.PlayAnimationClip(animationController.basicShotAnimation);
            bufferHandler.BufferAction(new BufferedInput(ExecuteBasicShot, context.action, animationController.basicShotAnimation.length + 0.015f, Time.time));
        }
    }

    void ExecuteBasicShot()
    {
        animationController.PlayAnimationClip(animationController.basicShotAnimation);
    }

    //Logic for when UseCard input action is activated
    public void UseCard(InputAction.CallbackContext context)
    {
        if(MovingCoroutine != null){return;}
        if(!CanAct){return;}
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}
        //if(!cardManager.CanUseCards){return;}
        if(cardManager.MagazineIsEmpty()){return;}//Check if magazine is empty

        if(context.performed)
        {
            bufferHandler.BufferAction(new BufferedInput(ExecuteUseCard, context.action, cardManager.NextCard.cardSO.PlayerAnimation.length + 0.05f, Time.time));

            //cardManager.TriggerCard();   
        }

    }

    void ExecuteUseCard()
    {
        cardManager.TriggerCard();   
    }

    public void OpenCardSelectMenu(InputAction.CallbackContext context)
    {
        if(cardManager.CardInUseCoroutine != null) { return; }
        if(!CanAct){return;}
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}

        if(cardSelectionMenu == null)
        {
            Debug.LogError("CardSelectionMenu could not be found in the scene, could not open card select");
            return;
        }

        if(context.performed)
        {
            
            //implement for energy system
            if(energyController != null)
            {
                if(!energyController.EnergyIsFull())
                {
                    return;
                }
            }
            //end for energy system
            
            cardSelectionMenu.ActivateMenu();
        }

    }

    void ExecuteOpenCardSelect()
    {
        //implement for energy system
        if(energyController != null)
        {
            if(!energyController.EnergyIsFull())
            {
                return;
            }
        }
        //end for energy system
        
        cardSelectionMenu.ActivateMenu();        
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if(cardManager.CardInUseCoroutine != null) { return; }
        if(!CanUsePlayerInput()) { return; }
        if(MovingCoroutine != null) 
        {
            dashController.DashReticle.SetActive(false);
            return; 
        }

        if(context.performed)
        {
            if(!dashController.CanDash())
            {
                dashController.inputPressedDuringCooldown = true;
                return;
            }

            dashController.DashReticle.SetActive(true);
        }
        else if(context.canceled)
        {
            if(dashController.inputPressedDuringCooldown)
            {
                dashController.inputPressedDuringCooldown = false;
                return;
            }

            if(dashController.CanDash())
            {
                dashController.DashTowardsAim();
                dashController.DashReticle.SetActive(false);
            }

        }
    }

    void ExecuteDash()
    {

    }

    public void Aim(InputAction.CallbackContext context)
    {
        currentAimVector = context.ReadValue<Vector2>();
        aimpoint.AimTowardDirection((int)currentAimVector.x, (int)currentAimVector.y);


    }



#endregion

//Place all methods that trigger on an animation event relevant to the player animator here.
//REMEMBER: If you change the name of any of these methods, you must also change the function name on any animations that 
//try to use the method to match its name.
#region Animation Events

    public void BasicShotAnimEvent()
    {
        Bullet bullet = Instantiate(basicShotProjectile,
                                    worldTransform.position, 
                                    aimpoint.transform.rotation).GetComponent<Bullet>();
        bullet.attackPayload.damage = basicShotDamage;
        bullet.attackPayload.attacker = gameObject;
        bullet.speed = 45;

        RuntimeManager.PlayOneShot(basicShotSFX, transform.position);

        OnBasicAttack?.Invoke();

        //Check the current aim direction of the aimpoint and modifies the bullet velocity so it flies in the
        //direction of the aimpoint
        switch (aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                bullet.velocity.x = 0;
                bullet.velocity.y = 1;
                break;

            case AimDirection.Down:
                bullet.velocity.x = 0;
                bullet.velocity.y = -1;
                break; 
            case AimDirection.Left:
                bullet.velocity.x = -1;
                bullet.velocity.y = 0;
                break;

            case AimDirection.Right:
                bullet.velocity.x = 1;
                bullet.velocity.y = 0;
                break;
        }

    }

#endregion

    //Custom DestroyEntity coroutine for the player 
    public override IEnumerator DestroyEntity(AttackPayload? killingBlow = null)
    {
        AdditionalDestructionEvents();
        isDefeated = true;
        invincible = true; //Make entity invincible during its destruction to prevent additional HurtEntity calls that may break things
        HPText.gameObject.SetActive(false);
        ShieldsText.gameObject.SetActive(false);

        if(entityAnimator.DefeatAnimation != null)
        {
            entityAnimator.PlayOneShotAnimation(entityAnimator.DefeatAnimation); //Play defeat animation
            yield return new WaitForSeconds(entityAnimator.DefeatAnimation.length);
        }
        
        yield return new WaitForSeconds(2f);
        OnPlayerDefeat?.Invoke();

        yield break;
    }



}
