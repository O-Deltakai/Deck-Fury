using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;
using FMODUnity;
using Cinemachine;

[RequireComponent(typeof(InputBufferHandler))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerCardManager))]
public class PlayerController : StageEntity
{

    public event Action OnPerformAction;

    CinemachineImpulseSource impulseSource;


    public delegate void KillEnemyEventHandler(NPC enemy);
    public event KillEnemyEventHandler OnKillSpecificEnemy;

    public delegate void KillEnemyPayloadEventHandler(NPC enemy, AttackPayload attackPayload);
    public event KillEnemyPayloadEventHandler OnKillEnemyWithPayload;

    public event Action OnKillEnemy;


    public delegate void HurtEnemyEventHandler(NPC enemy, AttackPayload attackPayload);
    public event HurtEnemyEventHandler OnHurtEnemyWithPayload;

    public delegate void BasicAttackEventHandler();
    public event BasicAttackEventHandler OnBasicAttack;

    public delegate void PlayerDefeatEvent();
    public event PlayerDefeatEvent OnPlayerDefeat;

    public bool CanFireBasicShot = true;

    PlayerDataContainer playerData;
    public int MaxHP;
    public float PercentHPRemaining => (float)CurrentHP / MaxHP;
    [SerializeField] Camera mainCamera;
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [field:SerializeField] public AimpointController aimpoint{get;private set;}
    [SerializeField] GameObject basicShotProjectile;
    [SerializeField] int basicShotDamage;


    PlayerDashController _dashController;
    public PlayerDashController DashController => _dashController;
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

/// <summary>
/// _globalDamageMultiplier affects all outgoing damage from the player.
/// </summary>
    [SerializeField, Min(0)] float _globalDamageMultiplier = 1;
    public float GlobalDamageMultiplier {get{ return _globalDamageMultiplier; }
        set
        {
            if(value < 0)
            {
                _globalDamageMultiplier = 0;
            }else
            {
                _globalDamageMultiplier = value;
            }
        }
    
    }

    [SerializeField] float _cardDamageMultiplier = 1;
    public float CardDamageMultiplier {get{ return _cardDamageMultiplier; }
        set
        {
            if(value < 0)
            {
                _cardDamageMultiplier = 0;
            }else
            {
                _cardDamageMultiplier = value;
            }
        }
    
    }

[Header("Player SFX")]
    [SerializeField] EventReference basicShotSFX;


    [SerializeField] float automoveCooldown = 0.2f;
    [SerializeField] bool canAutoMove = true;
    Coroutine CR_AutomoveTimer;

[Header("HP VFX Settings")]
    //Used to make hp text shake when hp is low
    [SerializeField] TextShaker hpTextShaker;
    [SerializeField] Color lowHPTextColor = Color.red;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>(); 
        animationController = GetComponent<PlayerAnimationController>();
        cardManager = GetComponent<PlayerCardManager>();
        playerCollider = GetComponent<BoxCollider2D>();
        _dashController = GetComponent<PlayerDashController>();
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

    public void KillEnemyTrigger(NPC enemy, AttackPayload? attackPayload = null)
    {
        print("Player killed: " + enemy.name);
        OnKillEnemy?.Invoke();
        OnKillSpecificEnemy?.Invoke(enemy);

        if(attackPayload.HasValue)
        {
            OnKillEnemyWithPayload?.Invoke(enemy, attackPayload.Value);
        }

    }

    public void HurtEnemyTrigger(NPC enemy, AttackPayload? attackPayload = null)
    {

    }

    void Update()
    {
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}


        SimpleMove();
        AutoMove();
        if(aimpoint.UseRelativeAiming)
        {
            FacePlayerTowardsCursor();
        }else
        {
            FacePlayerTowardsMouse();
        }

        if(PercentHPRemaining <= 0.40f)
        {
            hpTextShaker.StartShaking();
            if(DefaultHPTextColor != lowHPTextColor)
            {
                DefaultHPTextColor = lowHPTextColor;
            }
        }else
        {
            hpTextShaker.StopShaking();
            if(DefaultHPTextColor != Color.white)
            {
                DefaultHPTextColor = Color.white;
            }
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

        Defense = givenPlayerData.BaseDefense;
        Armor = givenPlayerData.BaseArmor;

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

        void BeginAutomoveTimer()
        {
            if(CR_AutomoveTimer != null)
            {
                StopCoroutine(CR_AutomoveTimer);
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }else
            {
                CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
            }            
        }

        bool CheckDestination(int x, int y)
        {
            Vector3Int destination;
            destination = new Vector3Int(currentTilePosition.x + x, currentTilePosition.y + y, 0);

            if(!_stageManager.CheckValidTile(destination))
            {
                return false;
            }
            return true;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            if(!CheckDestination(1, 0)){return;}

            canAutoMove = false;
            BeginAutomoveTimer();

        }
        if(Keyboard.current.aKey.wasPressedThisFrame)
        {
            if(!CheckDestination(-1, 0)){return;}

            canAutoMove = false;
            BeginAutomoveTimer();

        }
        if(Keyboard.current.wKey.wasPressedThisFrame)
        {
            if(!CheckDestination(0, 1)){return;}
            canAutoMove = false;
            BeginAutomoveTimer();


        }
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            if(!CheckDestination(0, -1)){return;}
            canAutoMove = false;
            BeginAutomoveTimer();


        }

        AutoMove();


    }

    void AutoMove()
    {
        if(canAutoMove)
        {
            if(Keyboard.current.dKey.isPressed)
            {
                if(!CheckDestination(1, 0)){return;}

                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }

                //Move right
                bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(1, 0, 0.1f), 0.1f, Time.unscaledTime));

            }
            if(Keyboard.current.aKey.isPressed)
            {   
                if(!CheckDestination(-1, 0)){return;}

                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }

                //Move left
                bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(-1, 0, 0.1f), 0.1f, Time.unscaledTime));


            }
            if(Keyboard.current.wKey.isPressed)
            {
                if(!CheckDestination(0, 1)){return;}

                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }
                //Move up
                bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(0, 1, 0.1f), 0.1f, Time.unscaledTime));

            }
            if(Keyboard.current.sKey.isPressed)
            {
                if(!CheckDestination(0, -1)){return;}

                if(CR_AutomoveTimer == null)
                {
                    CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown));
                }
                //Move down
                bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(0, -1, 0.1f), 0.1f, Time.unscaledTime));


            }
        }

    }


    IEnumerator AutomoveTimer(float duration)
    {
        canAutoMove = false;
        if(_useUnscaledTimeForActions)
        {
            yield return new WaitForSecondsRealtime(duration);
        }else
        {
            yield return new WaitForSeconds(duration);
        }
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
        if(GameManager.currentGameState == GameManager.GameState.InMenu){return false;}


        return true;
    }

    float moveSpeed = 0.1f;
    float moveLockoutTime = 0.1f;

    bool CheckDestination(int x, int y)
    {
        Vector3Int destination;
        destination = new Vector3Int(currentTilePosition.x + x, currentTilePosition.y + y, 0);

        if(!_stageManager.CheckValidTile(destination))
        {
            return false;
        }
        return true;
    }

    void BeginAutomoveTimer()
    {
        if(CR_AutomoveTimer != null)
        {
            StopCoroutine(CR_AutomoveTimer);
            CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
        }else
        {
            CR_AutomoveTimer = StartCoroutine(AutomoveTimer(automoveCooldown + 0.15f));
        }            
    }

    public void MoveUp(InputAction.CallbackContext context)
    {
        if(!CheckDestination(0, 1)){return;}
        if(!CanUsePlayerInput()){return;}
        if(!CanInitiateMovementActions){return;}


        if(context.performed)
        {
            canAutoMove = false;
            BeginAutomoveTimer();

            //Move right
            bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(0, 1, moveSpeed), moveLockoutTime, Time.unscaledTime));
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if(!CheckDestination(0, -1)){return;}
        if(!CanUsePlayerInput()){return;}
        if(!CanInitiateMovementActions){return;}


        if(context.performed)
        {
            canAutoMove = false;
            BeginAutomoveTimer();

            //Move right
            bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(0, -1, moveSpeed), moveLockoutTime, Time.unscaledTime));
        }
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if(!CheckDestination(-1, 0)){return;}
        if(!CanUsePlayerInput()){return;}
        if(!CanInitiateMovementActions){return;}


        if(context.performed)
        {
            canAutoMove = false;
            BeginAutomoveTimer();

            //Move right
            bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(-1, 0, moveSpeed), moveLockoutTime, Time.unscaledTime));
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if(!CheckDestination(1, 0)){return;}
        if(!CanUsePlayerInput()){return;}
        if(!CanInitiateMovementActions){return;}


        if(context.performed)
        {
            canAutoMove = false;
            BeginAutomoveTimer();

            //Move right
            bufferHandler.BufferAction(new BufferedInput(() => ExecuteMove(1, 0, moveSpeed), moveLockoutTime, Time.unscaledTime));
        }

    }


    void ExecuteMove(int x, int y, float speed)
    {
        Vector3Int destination = new Vector3Int(currentTilePosition.x + x, currentTilePosition.y + y, 0);
        if(!_stageManager.CheckValidTile(destination)) { return; }

        MovingCoroutine = StartCoroutine(TweenMove(x, y, speed, MovementEase));
        OnPerformAction?.Invoke();
    }

    //Logic for when BasicShot input action is activated
    public void BasicShot(InputAction.CallbackContext context)
    {

        if(!CanFireBasicShot){return;}
        if(!CanAct){return;}
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}
        if(!CanUsePlayerInput()) { return; }

        if(context.performed)
        {
            if(_useUnscaledTimeForActions)
            {
                bufferHandler.BufferAction(new BufferedInput(ExecuteBasicShot, context.action, animationController.basicShotAnimation.length + 0.015f, Time.unscaledTime));
            }else
            {
                bufferHandler.BufferAction(new BufferedInput(ExecuteBasicShot, context.action, animationController.basicShotAnimation.length + 0.015f, Time.time));

            }

            OnPerformAction?.Invoke();
        }
    }

    void ExecuteBasicShot()
    {
        animationController.PlayAnimationClip(animationController.basicShotAnimation);
    }

    //Logic for when UseCard input action is activated
    public void UseCard(InputAction.CallbackContext context)
    {
        //if(MovingCoroutine != null){return;}
        if(!CanAct){return;}
        if(isDefeated){return;}
        if(GameManager.GameIsPaused){return;}
        //if(!cardManager.CanUseCards){return;}
        if(cardManager.MagazineIsEmpty()){return;}//Check if magazine is empty
        if(!CanUsePlayerInput()) { return; }

        if(context.performed)
        {
            if(_useUnscaledTimeForActions)
            {
                bufferHandler.BufferAction(new BufferedInput(ExecuteUseCard, context.action, cardManager.NextCard.cardSO.PlayerAnimation.length + 0.05f, Time.unscaledTime));
            }else
            {
                bufferHandler.BufferAction(new BufferedInput(ExecuteUseCard, context.action, cardManager.NextCard.cardSO.PlayerAnimation.length + 0.05f, Time.time));

            }

            OnPerformAction?.Invoke();
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
        if(!cardSelectionMenu.CanBeOpened) { return; }

        if(cardSelectionMenu == null)
        {
            Debug.LogError("CardSelectionMenu could not be found in the scene, could not open card select");
            return;
        }

        if(context.performed)
        {
            
            if(energyController != null)
            {
                if(!energyController.EnergyIsFull())
                {
                    return;
                }
            }

            
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
        if(isDefeated) { return; }
        if(_statusEffectManager.Stunned) { return; }
        if(!CanUsePlayerInput()) { return; }


        if(context.performed)
        {
            if(!_dashController.CanDash())
            {
                _dashController.inputPressedDuringCooldown = true;
                return;
            }

            _dashController.DashReticle.SetActive(true);
        }
        else if(context.canceled)
        {
            if(_dashController.inputPressedDuringCooldown)
            {
                _dashController.inputPressedDuringCooldown = false;
                return;
            }

            if(_dashController.CanDash())
            {

                bufferHandler.BufferAction(new BufferedInput(ExecuteDash, context.action, _dashController.DashSpeed, Time.unscaledTime));
            }else
            {
                _dashController.DashReticle.SetActive(false);
            }

        }
    }

    void ExecuteDash()
    {
        _dashController.DashTowardsAim();
        _dashController.DashReticle.SetActive(false);
        OnPerformAction?.Invoke();
    }

    public void Aim(InputAction.CallbackContext context)
    {
        currentAimVector = context.ReadValue<Vector2>();
        aimpoint.AimTowardDirection((int)currentAimVector.x, (int)currentAimVector.y);


    }

    public void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if(TimeManager.SlowMotionInProgress) { return; }
        if(isDefeated) { return; }
        if (GameManager.Instance.PauseMenu.IsOpen){ return; }

        if(context.started)
        {
            GameManager.Instance.PauseMenu.OpenPauseMenu();
        }

    }

    public void TriggerFocusMode(InputAction.CallbackContext context)
    {
        if(!FocusModeController.Instance)
        {
            Debug.LogError("FocusModeController could not be found in the scene, could not trigger focus mode.");
            return;
        }
        if(!FocusModeController.Instance.gameObject.activeInHierarchy) {return;}
        if(TimeManager.SlowMotionInProgress) { return; }
        if(isDefeated) { return; }
        if (GameManager.Instance.PauseMenu.IsOpen){ return; }
        if(GameManager.GameIsPaused){return;}
        if(_statusEffectManager.Stunned) { return; }


        if(context.started)
        {
            FocusModeController.Instance.TriggerFocusMode();
        }

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
        bullet.attackPayload.damage = (int)(basicShotDamage * GlobalDamageMultiplier);
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

    protected override void AdditionalOnHurtEvents(AttackPayload? payload = null)
    {
        base.AdditionalOnHurtEvents(payload);
    }

    protected override void AdditionalAfterHurtEvents(AttackPayload? payload = null)
    {
        base.AdditionalAfterHurtEvents(payload);
        if(ShieldHP > 0)
        {   //If player still has shields after hurt, generate a smaller impulse
            impulseSource.GenerateImpulseWithVelocity(0.1f * SettingsManager.GlobalCameraShakeMultiplier * impulseSource.m_DefaultVelocity);
        }else
        {
            impulseSource.GenerateImpulseWithVelocity(0.2f * SettingsManager.GlobalCameraShakeMultiplier * impulseSource.m_DefaultVelocity);
        }

    }


    //Custom DestroyEntity coroutine for the player 
    public override IEnumerator DestroyEntity(AttackPayload? killingBlow = null)
    {
        AdditionalDestructionEvents();
        isDefeated = true;
        invincible = true; //Make entity invincible during its destruction to prevent additional HurtEntity calls that may break things
        HPText.gameObject.SetActive(false);
        ShieldsText.gameObject.SetActive(false);

        if(_entityAnimator.DefeatAnimation != null)
        {
            _entityAnimator.PlayOneShotAnimation(_entityAnimator.DefeatAnimation); //Play defeat animation
            yield return new WaitForSeconds(_entityAnimator.DefeatAnimation.length);
        }
        
        yield return new WaitForSeconds(2f);
        OnPlayerDefeat?.Invoke();

        yield break;
    }



}
