using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Vampire : NPC
{
    enum VampireAnims
    {
        Vampire_Idle = 0,
        Vampire_Attack = 1,
        Vampire_Spawn = 2,
        Vampire_StrongATK = 3
    }

    // The Zombie prefab to spawn
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] VampireBasicAttack BasicAttack;
    [SerializeField] GameObject targetingReticle;
    //zombie bomb prefab
    [SerializeField] GameObject zombieBombPrefab;
    //normal attack prefab
    [SerializeField] GameObject normalATKPrefab;
    //strong attack prefab
    [SerializeField] GameObject strongATKPrefab;
    //fury attack prefab
    [SerializeField] GameObject furyATKPrefab;
    // Time to wander before spawning zombies
    [SerializeField] float zombieSpawnInterval = 0.5f;
    [SerializeField] float spawnCoolTime = 10f;  // Time to move before spawning zombies
    [SerializeField] float aTKCoolTime = 2f;  // CT for all ATK
    [SerializeField] float normalATKCoolTime = 3f;  // CT for normalATK
    [SerializeField] float strongATKCoolTime = 6f;  // CT for strongATK
    float timeSinceLastSpawn = 0f;

    //[SerializeField] float attackCooldown;
    [SerializeField] float movementCooldown;

    [SerializeField] bool EnableAI = true;
    [SerializeField] bool canAttemptMove = true;
    [SerializeField] bool canAttack = false;
    [SerializeField] bool canNormalAttack = true;
    [SerializeField] bool canStrongAttack = true;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool canSpawn = false;
    private bool isSpawning = false;
    //private bool isSpawnScheduled = false;

    private int totalZombiesSpawned = 0;
    private const int maxZombiesAllowed = 6;

    [SerializeField] float furyCharge = 2f;

    [SerializeField] float lowHPRate ;
    float lowHP;
    EntityStatus entityStatus;

    SpawnManager spawnManager;
    [SerializeField] bool triggerAttack;


    Coroutine SpawnCoroutine;
    Coroutine attackWindUpCoroutine;
    Coroutine MovementCooldownCoroutine;
    Coroutine ATKCooldownCoroutine;
    [SerializeField] float normalATKWindupDuration = 1f;
    [SerializeField] float spawnWindupDuration = 1f;
    [SerializeField] float bombATKWindupDuration = 0.5f;
    [SerializeField] float furyATKWindupDuration = 1f;
    [SerializeField] float strongATKWindupDuration = 0.5f;
    
    [SerializeField] AttackPayload normalATKPayload;
    [SerializeField] AttackPayload strongATKPayload;
    
    [SerializeField] AttackPayload zombieBATKPayload;
    [SerializeField] AttackPayload furyATKPayload;


    Vector2Int playerDistance;
    
    int aTKMultiplier=1;
    bool isAiming;
    Vector3 fireDirection;

     
    enum EntityStatus{
        Normal, Fury, Low,
    }


    protected override void Start()
    {
        base.Start();
        spawnManager = FindObjectOfType<SpawnManager>();
        statusEffectManager.OnStunned += StunnedATKCounter;
        statusEffectManager.OnStunned += CancelAttack;
        OnDamageTaken += CheckHP;
        lowHP = CurrentHP * lowHPRate;
        StartCoroutine(ATKCooldown()); 
        canSpawn = true;
        canNormalAttack = true;
        canStrongAttack = true;
        targetingReticle.SetActive(false);
        isAiming = false;
    }

    void Update()
    {
        if (!EnableAI) { return; }
        //added so cannnot move in stun
        if(!CanAct){return;}
        
        playerDistance = (Vector2Int)currentTilePosition - (Vector2Int)player.currentTilePosition; 
        //timeSinceLastSpawn += Time.deltaTime;

        if(!isAttacking){
            MovementModerator(); 
        }

    }

    
    private void FixedUpdate() 
    {
        if(isAiming && !GameManager.GameIsPaused)
        {
            SetTargetingReticleOnPlayer();
        }        
    }


    //movement priority (normal): spawn>ATK
    void MovementModerator()
    {

        if(totalZombiesSpawned < maxZombiesAllowed)
        {
            //if (timeSinceLastSpawn < spawnCoolTime)
            if(canSpawn)
            {
                if (!isSpawning)
                {
                    //isSpawnScheduled = true;
                    attackWindUpCoroutine = StartCoroutine(SpawnZombies());
                    //StartCoroutine(SpawnZombies());
                    //timeSinceLastSpawn = 0f;  // reset the timer after spawning zombies
                }
            }
            else{
                ATKModerator();
            }
        } 
        else
        {
            ATKModerator();
        }  
    }

    //fury>strong>low>move
    void ATKModerator()
    {
        if(canAttack)
        {
            if(entityStatus == EntityStatus.Fury)
            {
                StartCoroutine(FuryATK());
            }
            else
            {
                if(entityStatus == EntityStatus.Low && canStrongAttack)
                {
                    attackWindUpCoroutine = StartCoroutine(StrongATK());
                }
                else
                {
                    attackWindUpCoroutine = StartCoroutine(NormalATK());
                }
            }
        }
        else
        {
            WanderBehaviour();
        }
    }

  
    protected void CheckHP(int damage){
        if(CurrentHP<=lowHP && entityStatus == EntityStatus.Normal){
            //power up and new action
            entityStatus = EntityStatus.Low;
        }
    }

    protected void StunnedATKCounter(){
        //max 3
        aTKMultiplier++;
        if(aTKMultiplier>3){
            aTKMultiplier = 3;
        }
        entityStatus = EntityStatus.Fury;

    }

    //bomb explosion when zombie dead
    void ZombieBombSetting(string deathNote, AttackPayload payload, StageEntity entity)
    {
        if (!EnableAI) { return; }
        totalZombiesSpawned-=1;
        Debug.Log(totalZombiesSpawned);
        VampireBomb zombieB = Instantiate(zombieBombPrefab, entity.currentTilePosition, transform.rotation).GetComponent<VampireBomb>();
        stageManager.SetVFXTile(stageManager.DangerVFXTile, GetBombAdjacentTiles(entity), bombATKWindupDuration);
        zombieB.explosionDelay=bombATKWindupDuration;
        zombieB.attackPayload = zombieBATKPayload;
        zombieB.gameObject.SetActive(true);
    }

    
    void SetTargetingReticleOnPlayer()
    {
        targetingReticle.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetLoops(4, LoopType.Restart)
        .SetEase(Ease.Linear).SetUpdate(false);          
        targetingReticle.transform.DOMove(player.currentTilePosition, strongATKWindupDuration);
    }

    //summon thunder to hit player
    IEnumerator NormalATK()
    {
        if (!canNormalAttack) { yield break; }
        if (!canAttack) { yield break; }
        if (isAttacking) { yield break; }
        if (isSpawning) { yield break; }

        canAttemptMove = false;
        canNormalAttack = false;
        isAttacking = true;
        canAttack = false;

        Debug.Log("NORMALATK");

        var animation = entityAnimator.animationList[(int)VampireAnims.Vampire_Attack];
        float animationDuration = animation.length;
        entityAnimator.PlayOneShotAnimationReturnIdle(animation);
        yield return new WaitForSeconds(animationDuration-0.1f);
        SummonSnapThunder();
        yield return new WaitForSeconds(normalATKWindupDuration);

        
        attackWindUpCoroutine = null;
        isAttacking = false;
        canAttemptMove = true;


        if (ATKCooldownCoroutine != null)
        {
            StopCoroutine(ATKCooldownCoroutine);
        }
        ATKCooldownCoroutine = StartCoroutine(ATKCooldown());
        StartCoroutine(NormalATKCooldown());

    }

    void SummonSnapThunder(){
        VampireNATK normalATK = Instantiate(normalATKPrefab, player.currentTilePosition, transform.rotation).GetComponent<VampireNATK>();
        List<Vector3Int> tileList = new List<Vector3Int>();
        tileList.Add( player.currentTilePosition);
        stageManager.SetVFXTile(stageManager.DangerVFXTile, tileList, normalATKWindupDuration);
        normalATK.attackDelay=normalATKWindupDuration;
        normalATK.masterEntity = this;
        normalATK.attackPayload = normalATKPayload;
        normalATK.attackPayload.damage *= aTKMultiplier;
        normalATK.gameObject.SetActive(true);
    }

    IEnumerator SpawnZombies() // spawn zombies every 5 seconds. 3 zombies each spawning sequence.
    {
        if (!canSpawn) { yield break; }
        if (isSpawning) { yield break; }
        if (isAttacking) { yield break; }

        Debug.Log("Spawn Zombie");
        canAttemptMove = false;
        isSpawning = true;
        canSpawn = false;
        isAttacking = true;

        var animation = entityAnimator.animationList[(int)VampireAnims.Vampire_Spawn];
        float animationDuration = animation.length;
        
        // Spawn zombies on adjacent valid tiles
        for (int i = 0; i < 3; i++)
        {
            List<Vector3Int> tileList = new List<Vector3Int>();

            Vector3Int predictedSpawnTile = spawnManager.PredictNPCSpawnPosition(currentTilePosition, true);
            if (predictedSpawnTile == Vector3Int.zero)
            {
                Debug.LogWarning("Failed to predict spawn tile for zombie.");
                continue; // Just in case
            }
            tileList.Add(predictedSpawnTile);

            // set vfx
            stageManager.SetVFXTile(stageManager.DangerVFXTile, tileList, spawnWindupDuration);
            yield return new WaitForSeconds(spawnWindupDuration);

            entityAnimator.PlayOneShotAnimationReturnIdle(animation);
            yield return new WaitForSeconds(animationDuration/2);
            SummonZombie();
            yield return new WaitForSeconds(animationDuration/2);

            if(totalZombiesSpawned >= maxZombiesAllowed){
                break;
            }

            if (i < 2) // so the countdown does not happen after the last zombie spawnSed.
            {
                yield return new WaitForSeconds(zombieSpawnInterval);
            }
            
        }
        
        attackWindUpCoroutine = null;

        canAttemptMove = true;
        isSpawning = false;
        isAttacking = false;

        StartCoroutine(SpawnCooldown()); 
    }

    void SummonZombie()
    {
        
        ZombieAStar zombie = (ZombieAStar)spawnManager.SpawnNPCPrefab(zombiePrefab, currentTilePosition, true);
        zombie.CurrentHP = 50;
        zombie.OnCauseOfDeath += ZombieBombSetting;
        totalZombiesSpawned++;
    }

    //lockon and fire 4 projectile to player
    IEnumerator StrongATK()
    {
        if (!canStrongAttack) { yield break; }
        if (isAttacking) { yield break; }
        if (isSpawning) { yield break; }
        if (!canAttack) { yield break; }

        canAttemptMove = false;
        canAttack = false;
        isAttacking = true;
        canStrongAttack=false;

        Debug.Log("STRONGATK");

        targetingReticle.SetActive(true);
        targetingReticle.transform.localPosition = new Vector3(0,0,0);
        isAiming = true;

        yield return new WaitForSeconds(strongATKWindupDuration);//Wait for aiming to complete before firing a bullet

        var animation = entityAnimator.animationList[(int)VampireAnims.Vampire_StrongATK];
        float animationDuration = animation.length;
        entityAnimator.PlayOneShotAnimationReturnIdle(animation);

        isAiming = false;
        targetingReticle.SetActive(false);

        FireBlood();

        yield return new WaitForSeconds(animationDuration);


        attackWindUpCoroutine = null;
        isAttacking = false;
        canAttemptMove = true;

        if (ATKCooldownCoroutine != null)
        {
            StopCoroutine(ATKCooldownCoroutine);
        }
        ATKCooldownCoroutine = StartCoroutine(ATKCooldown());
        StartCoroutine(StrongATKCooldown());

    }

    //Instantiates a bullet with necessary variables set
    void FireBlood()
    {   
        List<Vector3Int> targetTiles = GetTilesCardinalAdjacentToTarget(player.currentTilePosition);
        foreach(var tile in targetTiles)
        {
            
            Vector3 direction = tile - transform.position;

            Bullet bullet = Instantiate(strongATKPrefab, gameObject.transform.position, transform.rotation).GetComponent<Bullet>();

            //Set bullet variables
            Vector2 bulletVelocity = new Vector2(direction.normalized.x, direction.normalized.y);
            bullet.ChangeTrailRendererColor(new Color (41/255f, 9/255f, 22/255f));
            bullet.team = EntityTeam.Enemy;
            bullet.velocity = bulletVelocity;
            bullet.trailRenderer.time = 0.2f;
            bullet.attackPayload = strongATKPayload;
            bullet.attackPayload.damage *= aTKMultiplier;

            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ensure it doesnt hit themselves

        }
    }


    //cannot be disrupted, leap to player and unleash blood blast
    IEnumerator FuryATK()
    {
        if (!canAttack) { yield break; }
        if (isAttacking) { yield break; }
        if (isSpawning) { yield break; }

        canAttemptMove = false;
        isAttacking = true;
        canAttack = false;

        
        //reset status to normal or low
        if(CurrentHP <= lowHP)
        {
            entityStatus = EntityStatus.Low;
        }
        else
        {
            entityStatus= EntityStatus.Normal;
        }

        ShieldHP+=50;
        var animation = entityAnimator.animationList[(int)VampireAnims.Vampire_Spawn];
        float animationDuration = animation.length;

        Debug.Log("FURYATK");

        //start charge
        StartCoroutine(statusEffectManager.FlashColor(new Color (41/255f, 9/255f, 22/255f), furyCharge));
        yield return new WaitForSeconds(furyCharge);
        
        //atk after charge
        int mapSize = stageManager.groundTileDictionary.Count();
        int furyCount = (int) (mapSize) /10;

        yield return StartCoroutine(LeapToPlayer()); // Wait for the jump movement to complete
        
        List<Vector3Int> adjacentTiles = GetAdjacentTiles();
        foreach(var tile in adjacentTiles){
            FuryBlastSet(tile);
        }

        entityAnimator.PlayOneShotAnimation(animation);
        yield return new WaitForSeconds(animationDuration);

        for (int i = 0; i < furyCount; i++)
        {
            int randomIndex = Random.Range(0, mapSize);
            FuryBlastSet(stageManager.groundTileList[randomIndex].localCoordinates);
            yield return new WaitForSeconds(0.1f);
        }

        entityAnimator.PlayOneShotAnimationReturnIdle(animation);
        yield return new WaitForSeconds(animationDuration+ furyATKWindupDuration);
        
        isAttacking = false;
        canAttemptMove = true;
        canSpawn = true;
        
        if (ATKCooldownCoroutine != null)
        {
            StopCoroutine(ATKCooldownCoroutine);
        }
        ATKCooldownCoroutine = StartCoroutine(ATKCooldown());
    }


    //Instantiates fury object with necessary variables set
    void FuryBlastSet(Vector3Int targetTile)
    {
        VampireBomb blast = Instantiate(furyATKPrefab, targetTile, transform.rotation).GetComponent<VampireBomb>();
        stageManager.SetVFXTile(stageManager.DangerVFXTile, GetTilesAdjacentToTarget(targetTile), furyATKWindupDuration);
        blast.explosionDelay = furyATKWindupDuration;
        blast.attackPayload = furyATKPayload;
        blast.attackPayload.damage *= aTKMultiplier;
        blast.gameObject.SetActive(true);
    }

    //copy from leaper
    IEnumerator LeapToPlayer(){
        Vector3Int? tiles = GetRandomValidTile(GetTilesCardinalAdjacentToTarget(player.currentTilePosition));
        if (tiles.HasValue)
        {
            var xpos = tiles.Value.x;
            var ypos = tiles.Value.y;
            

            stageManager.SetTileEntity(null, currentTilePosition);
            Vector3Int destination = new Vector3Int(xpos, ypos, 0);
            currentTilePosition.Set(xpos, ypos, 0);

            //Gotta make sure to set the entity at the destination before you make the jump, otherwise if some entity moves into the tile
            //before they reach the destination, things break.
            stageManager.SetTileEntity(this, destination);


            yield return worldTransform.DOMove(destination, 0.1f)
                 .SetEase(Ease.InOutExpo)
                 .OnComplete(() =>
                 {
                     stageManager.SetTileEntity(this, currentTilePosition);
                 }).WaitForCompletion();
        }
        else
        {
            yield break;
        }
    }

        void CancelAttack()
        {
            if(attackWindUpCoroutine != null)
            {
                StopCoroutine(attackWindUpCoroutine);
                canAttack = true;
                isAttacking = false;
                isSpawning = false;
            }
        }

    Vector3Int? GetRandomValidTile(List<Vector3Int> adjacentTiles)
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();
        foreach(var tile in adjacentTiles)
        {
            if (stageManager.CheckValidTile(tile))
            {
                validTiles.Add(tile);
            }
        }
        if (validTiles.Count == 0)
        {
            return null; 
        }
        else if (validTiles.Count == 1)
        {
            return validTiles[0]; 
        }
        else
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            return validTiles[randomIndex]; 
        }

    }

    List<Vector3Int> GetAdjacentTiles()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        foreach (var direction in VectorDirections.Vector3IntAll)
        {
            Vector3Int adj = currentTilePosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }

        List<Vector3Int> GetBombAdjacentTiles(StageEntity entity)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        adjacentTiles.Add(entity.currentTilePosition);
        foreach (var direction in VectorDirections.Vector3IntAll)
        {
            Vector3Int adj = entity.currentTilePosition + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }


    List<Vector3Int> GetTilesCardinalAdjacentToTarget(Vector3Int targetTile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        adjacentTiles.Add(targetTile);
        foreach(var direction in VectorDirections.Vector3IntCardinal)
        {
            Vector3Int adj = targetTile + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }


    List<Vector3Int> GetTilesAdjacentToTarget(Vector3Int targetTile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        adjacentTiles.Add(targetTile);
        foreach(var direction in VectorDirections.Vector3IntAll)
        {
            Vector3Int adj = targetTile + direction;
            adjacentTiles.Add(adj);
        }
        return adjacentTiles;
    }

    void WanderBehaviour()
    {
      
        if (!canAttemptMove || isSpawning || !CanInitiateMovementActions) { return; } // If the vampire can't attempt a move, exit the method

        canAttemptMove = false;

        MoveRandom();

        //Prevent multiple movement cooldown coroutines from running at once which produces wacky movement behaviours
        if (MovementCooldownCoroutine != null)
        {
            StopCoroutine(MovementCooldownCoroutine);
        }
        MovementCooldownCoroutine = StartCoroutine(MovementCooldown());
    }

    void MoveRandom()
    {
        // potential moves
        Vector2Int[] possibleMoves = new Vector2Int[]{
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1)
            };

        Vector2Int chosenMove = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Length)];

        StartCoroutine(TweenMove(chosenMove.x, chosenMove.y, 0.1f, MovementEase));

    }
    IEnumerator MovementCooldown()
    {
        float randomFloat = UnityEngine.Random.Range(0.5f, 1f);
        yield return new WaitForSeconds(movementCooldown + randomFloat);
        canAttemptMove = true;

    }
   
    //if hp low, longer cool down
    IEnumerator SpawnCooldown()
    {
        float actualCoolDown = spawnCoolTime;
        if(entityStatus == EntityStatus.Low){
            actualCoolDown*=1.5f;
        }
        float randomFloat = UnityEngine.Random.Range(0.25f, 1f);
        yield return new WaitForSeconds(actualCoolDown + randomFloat);
        canSpawn = true;
    }

    //if hp low, half cool down
    IEnumerator NormalATKCooldown()
    {
        float actualCoolDown = normalATKCoolTime;
        if(entityStatus == EntityStatus.Low){
            actualCoolDown/=2f;
        }
        yield return new WaitForSeconds(actualCoolDown);
        canNormalAttack = true;
    }

    IEnumerator StrongATKCooldown()
    {
        yield return new WaitForSeconds(strongATKCoolTime);
        canStrongAttack = true;
    }
    
    //low hp, half cool down
    IEnumerator ATKCooldown()
    {
        float actualCoolDown = aTKCoolTime;
        if(entityStatus == EntityStatus.Low){
            actualCoolDown/=2f;
        }
        yield return new WaitForSeconds(actualCoolDown);
        canAttack = true;
        Debug.Log("finish charge for ATK");
    }


    protected override void AdditionalDestructionEvents(AttackPayload? killingBlow = null)
    {
        base.AdditionalDestructionEvents(killingBlow);
        EnableAI = false;
    }

}

