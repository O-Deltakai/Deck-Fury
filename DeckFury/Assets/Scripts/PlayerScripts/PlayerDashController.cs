using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDashController : MonoBehaviour
{
    PlayerController player;
    StageManager stageManager;
    PlayerAnimationController animationController;

    [SerializeField] AnimationClip dashAnimation;

    [SerializeField] AimpointController aimpoint;
    [SerializeField, Min(0)] int dashDistance = 3;
    [SerializeField, Min(0)] float dashSpeed = 0.15f;
    [SerializeField, Min(0)] float dashCooldown;

    bool usedDash = false;



    void Awake()
    {
        player = GetComponent<PlayerController>();
        animationController = GetComponent<PlayerAnimationController>();
    }

    void Start()
    {
        stageManager = player.EntityStageManager;
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if(!CanDash()) { return; }

        if(context.started)
        {
            
        }

        if(context.canceled)
        {
            DashTowardsAim();
        }

    }

    bool CanDash()
    {
        if(usedDash)
        {
            return false;
        }

        return true;
    }


    void DashTowardsAim()
    {
        int x = aimpoint.GetAimVector3Int().x * dashDistance;
        int y = aimpoint.GetAimVector3Int().y * dashDistance;

        Vector3Int destination = new Vector3Int(player.currentTilePosition.x + x, player.currentTilePosition.y + y, 0); 

        if(!stageManager.CheckValidTile(destination))
        {
            //First check if the tile ahead that is in the same direction of the leap is free and go there first.
            if(stageManager.CheckValidTile(destination + aimpoint.GetAimVector3Int()))
            {
                Vector3Int validPosition = destination + aimpoint.GetAimVector3Int();
                Vector3Int moveDistance = validPosition - player.currentTilePosition;

                StartCoroutine(DisableHitboxTimer());
                player.TweenMoveSetCoroutine(moveDistance.x, moveDistance.y, dashSpeed, Ease.OutBounce);

                animationController.PlayAnimationClip(dashAnimation);
                StartCoroutine(DashCooldown());
                return;                        
            }else
            if(stageManager.CheckValidTile(destination - aimpoint.GetAimVector3Int()))
            {//Secondly check if the tile behind that is in the same direction of the leap is free and go there second.
                Vector3Int validPosition = destination - aimpoint.GetAimVector3Int();
                Vector3Int moveDistance = validPosition - player.currentTilePosition;
 
                StartCoroutine(DisableHitboxTimer());
                player.TweenMoveSetCoroutine(moveDistance.x, moveDistance.y, dashSpeed, Ease.OutBounce);

                animationController.PlayAnimationClip(dashAnimation);
                StartCoroutine(DashCooldown());

                return; 
            }

            //If the tile the first and second check fails, iterate through all adjacent tiles to find a valid landing point.
            foreach(Vector3Int direction in VectorDirections.Vector3IntAll)
            {
                if(stageManager.CheckValidTile(destination + direction))
                {
                    Vector3Int validPosition = destination + direction;
                    Vector3Int moveDistance = validPosition - player.currentTilePosition;

                    StartCoroutine(DisableHitboxTimer());
                    player.TweenMoveSetCoroutine(moveDistance.x, moveDistance.y, dashSpeed, Ease.OutBounce);
                    
                    animationController.PlayAnimationClip(dashAnimation);
                    StartCoroutine(DashCooldown());

                    return;
                }
            }
        }else
        {
            animationController.PlayAnimationClip(dashAnimation);            
            player.TweenMoveSetCoroutine(x, y, dashSpeed, Ease.OutCubic);
            StartCoroutine(DisableHitboxTimer());
            StartCoroutine(DashCooldown());

        }



    }

    IEnumerator DisableHitboxTimer()
    {
        player.playerCollider.enabled = false;
        yield return new WaitForSeconds(dashSpeed);
        player.playerCollider.enabled = true;        
    }

    IEnumerator DashCooldown()
    {
        usedDash = true;
        yield return new WaitForSecondsRealtime(dashCooldown);
        usedDash = false;
    }

}
