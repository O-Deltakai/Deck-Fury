using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Color = UnityEngine.Color;
using System.Collections.Generic;
using FMODUnity;

public enum ForceMoveMode
{
    None,
    Reverse,
    Forward
}



[RequireComponent(typeof(EntityAnimationController))]
[RequireComponent(typeof(EntityStatusEffectManager))]
[RequireComponent(typeof(EntityUIElementAnimator))]
public class StageEntity : MonoBehaviour
{
    [SerializeField] protected Ease MovementEase;

#region Events and Delegates


    public delegate void TweenMoveEventHandler(Vector3Int startPosition, Vector3Int destination);
    public event TweenMoveEventHandler OnTweenMove;

    public delegate void CauseOfDeathEventHandler(string deathNote, AttackPayload payload, StageEntity entity);
    public event CauseOfDeathEventHandler OnCauseOfDeath;

    //C# event for when the entity is destroyed. Triggers in the DestroyEntity method.
    public delegate void DestructionEventHandler(StageEntity entity, Vector3Int tilePosition);
    public virtual event DestructionEventHandler OnDestructionEvent;

    /// <summary>
    /// A no parameter event for when the entity is destroyed.
    /// </summary>
    public event Action OnDestroyed;


    public delegate void DamageTakenEventHandler(int damageTaken);
    public event DamageTakenEventHandler OnDamageTaken;


    public delegate void ShieldHPChangedHandler(int oldValue, int newValue);
    public event ShieldHPChangedHandler OnShieldHPChanged;

    public delegate void CurrentHPChangedHandler(int oldValue, int newValue);
    public event CurrentHPChangedHandler OnHPChanged;

    public event Action<int> OnArmorChanged;

    public event Action OnTakeCritDamage;
    public event Action OnResistDamage;


#endregion


    //The anchor point of this entity on the world - should normally correspond to a valid tile position on the GroundTileMap
    //The transform that will be used when doing calculations/methods involving moving the entity from cell to cell.
    //Should normally be the EntityWrapper game object that is parented to this game object.
    [field:SerializeField] public Transform worldTransform {get; private set;}

    //A calculated variable meant to define the INTENDED tile position of the entity, not its real-time world position
    //Use this when making movement-related methods to prevent rounding issues with using the real-time world position
    public Vector3Int currentTilePosition;

    protected StageManager _stageManager;
    public StageManager EntityStageManager{get { return _stageManager; }}
    protected EntityAnimationController entityAnimator;
    protected EntityStatusEffectManager statusEffectManager;
    protected EntityUIElementAnimator UIElementAnimator;

    //Coroutine object used to prevent too many instances of the TweenMove coroutine being started at once.
    protected Coroutine MovingCoroutine;

    //Bools used by the EntityStatusEffectManager and are set upon being stunned. Should be seperate from NPC specific conditions.
    //Only read these values on NPCs, don't set them within the actual NPC script as these bools should only be controlled by the EntityStatusEffectManager.
    public bool CanInitiateMovementActions = true;
    public bool CanAct = true;
    public bool invincible = false;

    /// <summary>
    /// If an entity is grounded, that means they can be hurt by most tile hazards.
    /// </summary>
    [field:SerializeField] public bool IsGrounded {get; private set;} = false;
    

    /// <summary>
    /// This property determines whether this stage entity has the ability to be damaged at all. If you plan on this stage entity
    /// to never being able to be damaged, set this property to true. It will disable damage methods and means you will not need to set 
    /// UI elements for HP text and Shield text.
    /// </summary>
    [field:SerializeField] public bool CannotBeTargeted {get; private set;} = false;

    [SerializeField] int currentHP = 100;
    //Publically accessible property for currentHP that when set, will set the value of the private currentHP value and raise the OnShieldHPChanged event.
    //Used for updating the shield text object when it is greater than 0.
    public int CurrentHP {
        get{return currentHP;}
        set
        {
            if(currentHP != value)
            {
                int oldValue = currentHP;
                currentHP = value;
                OnHPChanged?.Invoke(oldValue, currentHP);
            }
        }

    }


    [SerializeField] int shieldHP;
    //Publically accessible property for shieldHP that when set, will set the value of the private shieldHP value and raise the OnShieldHPChanged event.
    //Used for updating the shield text object when it is greater than 0.
    public int ShieldHP {
        get{return shieldHP;}
        set
        {
            if(shieldHP != value)
            {
                int oldValue = shieldHP;
                shieldHP = value;
                OnShieldHPChanged?.Invoke(oldValue, shieldHP);
            }
        }
    }
    

    [Range(0, 100)] int _armor = 0;
    public int Armor {get { return _armor; } 
        set
        {
            if(value < 0)
            {
                _armor = 0;
            }else 
            if(value > 100)
            {
                _armor = 100;
            }else
            {
                _armor = value;
            }
            OnArmorChanged?.Invoke(value);
        }
    }

    [Range(0.1f, 10f)] public double defense = 1;

    [SerializeField] protected List<AttackElement> weaknesses; // What attack elements is this entity weak to (take bonus damage from)?
    [SerializeField] protected double weaknessModifier = 1.5f;
    [SerializeField] protected List<AttackElement> resistances; // What attack elements is this entity resistant to?
    [SerializeField] protected double resistModifier = 0.5f;


[Header("Entity Stat UI Elements")]
    [SerializeField] protected TextMeshPro HPText;
    public Color DefaultHPTextColor {get; private set;}
    [SerializeField] protected TextMeshPro ShieldsText;
    public Color DefaultShieldTextColor {get; private set;}

    [SerializeField] protected SpriteRenderer _armorIcon;
    public SpriteRenderer ArmorIcon => _armorIcon;


    //Variable meant to indicate what direction the entity is facing. Normally used for aiming NPC attacks.    
    public AimDirection FacingDirection {get;private set;}
    [SerializeField] bool DoNotShowHP = false;

    [SerializeField] protected SpriteRenderer entitySpriteRenderer;

[Header("SFX")]
    [SerializeField] protected EventReference OnDamagedSFX;
    [SerializeField] protected EventReference OnDeathSFX;



    //Method to intialize all common variables between StageEntities
    protected virtual void IntializeAwakeVariables()
    {


        entityAnimator = GetComponent<EntityAnimationController>();
        statusEffectManager = GetComponent<EntityStatusEffectManager>();
        UIElementAnimator = GetComponent<EntityUIElementAnimator>();
        if(entitySpriteRenderer == null)
        {
            entitySpriteRenderer = GetComponent<SpriteRenderer>();
        }

        OnShieldHPChanged += UpdateShieldValue;
        OnHPChanged += UpdateHPValue;

        //stageManager = GameErrorHandler.NullCheck(StageManager.Instance, "Stage Manager");


        _stageManager = StageManager.Instance;

        if(_stageManager)
        {
            currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);
            _stageManager.SetTileEntity(this, currentTilePosition);
        }

    }

    //Method to initialize all common starting states between StageEntities
    protected virtual void InitializeStartingStates()
    {

        _stageManager = StageManager.Instance;
        if(_stageManager)
        {
            currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);
            _stageManager.SetTileEntity(this, currentTilePosition);
        }
        


        if(!CannotBeTargeted || !DoNotShowHP)
        {
            if(HPText != null)
            {
                HPText.text = currentHP.ToString();
                DefaultHPTextColor = HPText.color;  
            }
            if(ShieldsText != null)
            {
                ShieldsText.text = shieldHP.ToString();
                DefaultShieldTextColor = ShieldsText.color;
                
                if(shieldHP <= 0)
                {
                    ShieldsText.gameObject.SetActive(false);
                }
            }

        }

        if(_armorIcon)
        {
            if(Armor <= 0)
            {
                _armorIcon.gameObject.SetActive(false);
            }
        }


    }


    //Can be overriden if you need to initialize additional awake variables. However, you need to make sure to keep the
    //base.Awake() method call in order to call the InitializeAwakeVariables from this class.
    protected virtual void Awake()
    {
        IntializeAwakeVariables();

    }

    //Can be overriden if you need to initialize additional start states. However, you need to make sure to keep the
    //base.Start() method call in order to call the InitializeStartingStates from this class.
    protected virtual void Start()
    {
        InitializeStartingStates();   
    }

    protected void UpdateShieldValue(int oldValue, int newValue)
    {
        if(DoNotShowHP){return;}
        if(shieldHP <= 0)
        {
            ShieldsText.gameObject.SetActive(false);
        }else
        {
            ShieldsText.gameObject.SetActive(true);
        }

        UIElementAnimator.AnimateNumberCounter(ShieldsText, oldValue, newValue);
    
    }

    protected void UpdateHPValue(int oldValue, int newValue)
    {
        if(DoNotShowHP){return;}

        if(currentHP <= 0)
        {
            HPText.gameObject.SetActive(false);
        }else
        {
            HPText.gameObject.SetActive(true);
        }
        UIElementAnimator.AnimateNumberCounter(HPText, oldValue, newValue);
    }


    //This gets the current world transform position and casts it to a Vector3Int. Due to rounding issues, it is best not to use
    //this method when calculating movements as it may result in the wrong tile position if the entity was half-way between two tiles.
    public Vector3Int GetWorldTilePosition()
    {
        return new Vector3Int((int)worldTransform.position.x, (int)worldTransform.position.y, 0);
    }

    public void AttemptMovement(int x, int y, float duration, Ease ease = Ease.InExpo, ForceMoveMode forceMoveMode = ForceMoveMode.None)
    {
        StartCoroutine(TweenMove(x, y, duration, ease, forceMoveMode));
    }

    public void TweenMoveSetCoroutine(int x, int y, float duration, Ease ease = Ease.InExpo, ForceMoveMode forceMoveMode = ForceMoveMode.None)
    {
        MovingCoroutine = StartCoroutine(TweenMove(x, y, duration, ease, forceMoveMode));
    }

    /// <summary>
    ///<para>Uses the DOTween library DOMove method to move the entity a specified amount of distance from its original location.
    ///The destinationCell is determined by adding on the given x and y value to the currentTilePosition x and y of the object.</para>
    ///
    ///<para>duration determines how quickly entity moves from one place to another.</para>
    ///
    ///
    ///<para>Ease dictates what the curve of the movement will follow when moving the entity from one point to another.</para>
    ///
    ///<para>forceMove condition allows TweenMove to bypass regular condition checks and force the entity to move to the destination,
    ///or as close to the destination as possible by checking for valid tiles in the path, even if the destination itself is an invalid tile.</para> 
    /// 
    ///<para>The worldLocation condition indicates whether the coordinates you're inputting is the exact location on the map you want to move to or
    ///whether its the distance you want the entity to move. By defualt it is by distance and not location which is set to false.</para>
    ///
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="duration"></param>
    /// <param name="ease"></param>
    /// <param name="forceMove"></param>
    /// <returns></returns>
    public virtual IEnumerator TweenMove(int x, int y, float duration, Ease ease = Ease.Linear, ForceMoveMode forceMoveMode = ForceMoveMode.None,  bool worldLocation = false)
    {
        if(x == 0 && y == 0){yield break;}
        if(!CanInitiateMovementActions && forceMoveMode == ForceMoveMode.None){yield break;}
        //If a TweenMove coroutine is already active, break this operation, unless forceMoveMode is not none.
        if(MovingCoroutine != null && forceMoveMode == ForceMoveMode.None)
        {
            yield break;
        }

        Vector3Int destination;

        if(worldLocation)
        {
            destination = new Vector3Int(x, y, 0);
        }else
        {
            //The location this entity will attempt to move to.
            destination = new Vector3Int(currentTilePosition.x + x, currentTilePosition.y + y, 0);
        }


        if(forceMoveMode == ForceMoveMode.None || Math.Abs(x) + Math.Abs(y) == 1 || Math.Abs(x) == 1 && Math.Abs(y) == 1)
        {
            //Checks if the destination is a valid tile to move to. If not valid, break operation
            if(!_stageManager.CheckValidTile(destination))
            {yield break;}
        }else
        if(forceMoveMode == ForceMoveMode.Reverse)
        {
            
            if(!_stageManager.CheckValidTile(destination))
            {
                destination = FindValidDestinationReverseCheck(x, y, destination);
                if(destination == currentTilePosition){yield break;}

            }
        }else
        if(forceMoveMode == ForceMoveMode.Forward)
        {
            destination = FindValidDestinationForwardCheck(x, y, destination);
            if(destination == currentTilePosition){yield break;}
        }
        
        OnTweenMove?.Invoke(currentTilePosition, destination);

        //Specific order of actions: Set the tile entity of the current tile position to null, then set the current tile position
        //to be that of the destination, then set the tile entity of the destination to this. This order is important as it
        //prevents issues where the player inputs a new movement too quickly, where using the real time tile position is wrong
        //because the player has not yet moved all the way to destination, causing the Vector3Int cast of the method to round to
        //the wrong value.
        _stageManager.SetTileEntity(null, currentTilePosition);
        currentTilePosition.Set(destination.x, destination.y, 0);
        _stageManager.SetTileEntity(this, destination);


        worldTransform.DOMove(destination, duration).SetEase(ease);


        //Sets MovingCoroutine to null after some duration has passed so that another TweenMove coroutine may start.
        //Prevents too many movement inputs from happening at once, effectively limiting player mobility.
        yield return new WaitForSeconds(duration * 0.5f);
        MovingCoroutine = null;

    }

    public void TeleportToLocation(int x, int y)
    {
        Vector3Int destination = new Vector3Int(x, y, 0);
        
        if(!_stageManager.CheckValidTile(destination)) { return; }

        _stageManager.SetTileEntity(null, currentTilePosition);

        worldTransform.position = destination;
        currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);

        _stageManager.SetTileEntity(this, destination);

    }

    public void TeleportToLocation(Vector3 location)
    {
        Vector3Int destination = new Vector3Int((int)location.x, (int)location.y, (int)location.z);

        print("destination: " + destination);

        if(!_stageManager) { _stageManager = StageManager.Instance; }

        if(!_stageManager.CheckValidTile(destination)) 
        {
            print("Invalid location at: + " + destination);
            return; 
        }

        _stageManager.SetTileEntity(null, currentTilePosition);

        worldTransform.position = destination;
        currentTilePosition.Set((int)worldTransform.position.x, (int)worldTransform.position.y, 0);

        _stageManager.SetTileEntity(this, destination);
    }

    public void TeleportToLocation(Vector3Int destination)
    {
        
        if(!_stageManager.CheckValidTile(destination)) { return; }

        _stageManager.SetTileEntity(null, currentTilePosition);
        currentTilePosition.Set(destination.x, destination.y, 0);
        _stageManager.SetTileEntity(this, destination);

        worldTransform.position = destination;
    }

    //Tries to find a valid destination by counting backwards from the destination until finds the first valid position (currently untested)
    Vector3Int FindValidDestinationReverseCheck(int x, int y, Vector3Int destination)
    {
        //Is the movement purely horizontal?
        if(Math.Abs(x) > 0 && y == 0)
        {
            Vector3Int currentlyCheckedTile = destination;
            //Movement is going towards the right
            if(x > 0)
            {
                while(currentlyCheckedTile != currentTilePosition)
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.x--;
                    }else
                    {
                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                        
                    }
                }
                if(currentlyCheckedTile == currentTilePosition){return currentTilePosition;}//If could not find a valid tile in the path, return.
                
            }else //Movement is going towards the left
            {  
                while(currentlyCheckedTile != currentTilePosition)
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.x++;
                    }else
                    {
                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                        
                    }
                }
                if(currentlyCheckedTile == currentTilePosition){return currentTilePosition;}//If could not find a valid tile in the path, return.                        
            }

        }else
        if(Math.Abs(y) > 0 && x == 0)//Movement is purely vertical
        {
            Vector3Int currentlyCheckedTile = destination;
            //Movement is going upwards
            if(y > 0)
            {
                while(currentlyCheckedTile != currentTilePosition)
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.y--;
                    }else
                    {
                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                        
                    }
                }
                if(currentlyCheckedTile == currentTilePosition){return currentTilePosition;}//If could not find a valid tile in the path, return.
                
            }else //Movement is going downwards
            {  
                while(currentlyCheckedTile != currentTilePosition)
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.y++;
                    }else
                    {
                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                        
                    }
                }
                if(currentlyCheckedTile == currentTilePosition){return currentTilePosition;}//If could not find a valid tile in the path, return.                        
            }

        }else
        {
            //Currently no algorithm to calculate a valid tile for diagonal movements, will implement one in the future.
            return currentTilePosition;
        }        


        return currentTilePosition;
    }

    //Finds the destination by counting up from the currentTilePosition until hits an obstacle and invalidates the currentlyCheckedTile
    Vector3Int FindValidDestinationForwardCheck(int x, int y, Vector3Int destination)
    {
        Vector3Int currentlyCheckedTile = currentTilePosition;
        int distance = (int)Vector3Int.Distance(currentTilePosition, destination);

        //Is the movement purely horizontal?
        if(Math.Abs(x) > 0 && y == 0)
        {
            //Movement is going towards the right
            if(x > 0)
            {
                currentlyCheckedTile.x++;

                for(int i = 0; i < distance; i++) 
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.x--;
                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                    }else
                    {
                        currentlyCheckedTile.x++;
                    }    
                }
                return destination;
                
            }else //Movement is going towards the left
            {
                currentlyCheckedTile.x--;

                for(int i = 0; i < distance; i++) 
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.x++;

                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                    }else
                    {
                        currentlyCheckedTile.x--;
                    }    
                }
                return destination;                     
            }

        }else
        if(Math.Abs(y) > 0 && x == 0)//Movement is purely vertical
        {
            //Movement is going upwards
            if(y > 0)
            {
                currentlyCheckedTile.y++;

                for(int i = 0; i < distance; i++) 
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.y--;

                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                    }else
                    {
                        currentlyCheckedTile.y++;
                    }    
                }

                return destination; 
                
            }else //Movement is going downwards
            {  
                currentlyCheckedTile.y--;

                for(int i = 0; i < distance; i++) 
                {
                    if(!_stageManager.CheckValidTile(currentlyCheckedTile))
                    {
                        currentlyCheckedTile.y++;

                        return currentlyCheckedTile;//Found a valid tile in the path that we can move to
                    }else
                    {
                        currentlyCheckedTile.y--;

                    }    
                }
                return destination; 
            }

        }else
        {

            Debug.LogWarning("Currently no algorithm to calculate a valid tile for diagonal movements, will implement one in the future.");
            return currentTilePosition;
        }        


 
    }

    //Empty virtual method that may be overridden if the entity has specific operations that need to be completed
    //before the generic HurtEntity method calculates damage.
    protected virtual void AdditionalOnHurtEvents(AttackPayload? payload = null){}

    //Primary method used for dealing damage to the entity, uses the AttackPayload struct for damage calculations.
    public virtual void HurtEntity(AttackPayload payload, Color? hitFlashColor = null, EventReference? hitSFX = null)
    {
        if(CannotBeTargeted){return;}
        if(invincible){return;}

        AdditionalOnHurtEvents(payload);

        AttackPayload finalPayload = payload;

        if(statusEffectManager.MarkedForDeath && payload.canTriggerMark)
        {
            statusEffectManager.TriggerMarkEffect(payload);
        }

        Color actualHitFlashColor;
        if(hitFlashColor.HasValue)
        {
            actualHitFlashColor = hitFlashColor.Value;
        }else
        {
            actualHitFlashColor = Color.white;
        }

        if(finalPayload.statusEffectType != null)
        {
            //Iterate over all status effects in the attack payload and trigger each status effect sequentially
            foreach(StatusEffectType statusEffect in finalPayload.statusEffectType)
            {
                if(statusEffect == StatusEffectType.None)
                {
                    continue;
                }
                statusEffectManager.TriggerStatusEffect(statusEffect);
            }

        }

        if(finalPayload.actualStatusEffects != null)
        {
            foreach(StatusEffect statusEffect in finalPayload.actualStatusEffects)
            {
                if(statusEffect.statusEffectType == StatusEffectType.None)
                {
                    continue;
                }
                statusEffectManager.TriggerStatusEffect(finalPayload, statusEffect);
            }
        }




        int damageAfterModifiers;
        //Do damage calculations for if AttackElement is breaking
        if(finalPayload.attackElement == AttackElement.Breaking)
        {
            int originalShieldHP = ShieldHP;
            bool wentThroughShields = false;
            //Check shield damage - shields do not inherit armor or defense or weaknesses/resists on entity so shields will always take normal/full damage from payload.
            //However, breaking attacks deal damage to shields at 2x efficiency
            ShieldHP -= finalPayload.damage * 2;
            UIElementAnimator.AnimateShakeNumber(ShieldsText, finalPayload.damage * 2, DefaultShieldTextColor, Color.red);

            //If damage taken is greater than shieldHP, then the remaining damage is taken to the current HP.
            if(shieldHP < 0)
            {
                //If the attack was breaking damage, half the remaining damage so that damage taken to HP is still 1x efficiency
                damageAfterModifiers = (int)Math.Round(Math.Abs(shieldHP * 0.5), MidpointRounding.AwayFromZero);
                UIElementAnimator.AnimateShakeNumber(HPText, damageAfterModifiers, DefaultHPTextColor, Color.red);    
                shieldHP = 0;
                if(originalShieldHP > 0)
                {
                    wentThroughShields = true;
                }
                OnDamageTaken?.Invoke(originalShieldHP + damageAfterModifiers);
            }else
            {
                damageAfterModifiers = 0;
                OnDamageTaken?.Invoke(finalPayload.damage * 2);   
            }

            if(damageAfterModifiers != 0)
            {
                damageAfterModifiers = (int)(damageAfterModifiers/defense);
                UIElementAnimator.AnimateShakeNumber(HPText, damageAfterModifiers, DefaultHPTextColor, Color.red);
                if(wentThroughShields)
                {
                    OnDamageTaken?.Invoke(originalShieldHP + damageAfterModifiers);
                }else
                {
                    OnDamageTaken?.Invoke(damageAfterModifiers);
                }                
            }

            //Check resist/weakness to attack element to calculate final damage
            if(CheckWeakness(finalPayload.attackElement))
            {
                damageAfterModifiers = (int)(damageAfterModifiers * weaknessModifier);
                OnTakeCritDamage?.Invoke();
            }
            if(CheckResistance(finalPayload.attackElement))
            {
                damageAfterModifiers = (int)(damageAfterModifiers * resistModifier);
                OnResistDamage?.Invoke();
            }

            CurrentHP -= damageAfterModifiers;
            
        }else
        if(finalPayload.attackElement == AttackElement.Pure)//Pure damage bypasses all resistances and shields and deals damage straight to HP
        {   
            UIElementAnimator.AnimateShakeNumber(HPText, finalPayload.damage, DefaultHPTextColor, Color.red);    
            CurrentHP -= finalPayload.damage;
            OnDamageTaken?.Invoke(finalPayload.damage);

        }
        else
        {//Do damage calculation for non-Breaking damage and non-Pure damage

            int originalShieldHP = ShieldHP;
            bool wentThroughShields = false;

            ShieldHP -= finalPayload.damage;
            UIElementAnimator.AnimateShakeNumber(ShieldsText, finalPayload.damage, DefaultShieldTextColor, Color.red);
            if(shieldHP < 0)
            {
                damageAfterModifiers = Math.Abs(shieldHP);
                shieldHP = 0;
                if(originalShieldHP > 0)
                {
                    wentThroughShields = true;
                }
                
            }else
            {
                damageAfterModifiers = 0;
                OnDamageTaken?.Invoke(finalPayload.damage);   
            }

            if(damageAfterModifiers != 0)
            {
                damageAfterModifiers = (int)(damageAfterModifiers * ((100 - Armor) * 0.01) * defense);
                UIElementAnimator.AnimateShakeNumber(HPText, damageAfterModifiers, DefaultHPTextColor, Color.red);
                if(wentThroughShields)
                {
                    OnDamageTaken?.Invoke(originalShieldHP + damageAfterModifiers);
                }else
                {
                    OnDamageTaken?.Invoke(damageAfterModifiers);
                }
            }
            
            //Check resist/weakness to attack element to calculate final damage
            if(CheckWeakness(finalPayload.attackElement))
            {
                damageAfterModifiers = (int)(damageAfterModifiers * weaknessModifier);
                OnTakeCritDamage?.Invoke();
            }
            if(CheckResistance(finalPayload.attackElement))
            {
                damageAfterModifiers = (int)(damageAfterModifiers * resistModifier);
                OnResistDamage?.Invoke();
            }            

            CurrentHP -= damageAfterModifiers;
            
        }

        StartCoroutine(statusEffectManager.FlashColor(actualHitFlashColor, 0.025f, 0.025f));//Flash white to indicate being hit
        if(!OnDamagedSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(OnDamagedSFX, transform.position);
        }

        if(currentHP <= 0) //Begin destruction once HP goes to at or below 0
        {
            StartCoroutine(DestroyEntity(payload));
            OnCauseOfDeath?.Invoke(payload.causeOfDeathNote, payload, this);
        }
    }

    bool CheckWeakness(AttackElement attackElement)
    {
        foreach(AttackElement element in weaknesses)
        {
            if(attackElement == element)
            {
                return true;
            }
        }

        return false;
    }

    bool CheckResistance(AttackElement attackElement)
    {
        foreach(AttackElement element in resistances)
        {
            if(attackElement == element)
            {
                return true;
            }
        }

        return false;        
    }

    //TODO: Method which calculates the final damage output of an attack payload given this entity's stats. Also handles the
    //triggering of MarkedForDeath interactions. 
    private int CalculateDamageAndOtherEffects(AttackPayload payload)
    {
        int damageAfterModifiers = 0;
        AttackPayload tempPayload = payload;

        return damageAfterModifiers;
    }



    //Empty virtual method that may be overridden if the entity has specific operations that need to be completed
    //before the generic DestroyEntity coroutine is completed.
    protected virtual void AdditionalDestructionEvents(AttackPayload? killingBlow = null){}
     

    //IEnumerator used for destroying the entity, will contain various statements that need to be performed before the 
    //object is fully destroyed.
    public virtual IEnumerator DestroyEntity(AttackPayload? killingBlow = null)
    {
        AdditionalDestructionEvents(killingBlow);
        CanInitiateMovementActions = false;
        CanAct = false;
        invincible = true; //Make entity invincible during its destruction to prevent additional HurtEntity calls that may break things
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>(); //Disable the entity's box collider if it has one
        if(boxCollider2D != null){boxCollider2D.enabled = false;}
        HPText.enabled = false;

        if(!OnDeathSFX.IsNull)
        {
            RuntimeManager.PlayOneShot(OnDeathSFX, transform.position);
        }


        if(entityAnimator.DefeatAnimation != null)
        {
            entityAnimator.PlayOneShotAnimation(entityAnimator.DefeatAnimation); //Play defeat animation
            yield return new WaitForSeconds(entityAnimator.DefeatAnimation.length);
        }

        _stageManager.SetTileEntity(null, currentTilePosition);

        GameErrorHandler.ExecuteSafely(() =>
        {
            OnDestructionEvent?.Invoke(this, currentTilePosition);
            return true;
        });

        OnDestroyed?.Invoke();

        Destroy(transform.parent.gameObject);
        Destroy(gameObject);

        yield break;
    }


}
