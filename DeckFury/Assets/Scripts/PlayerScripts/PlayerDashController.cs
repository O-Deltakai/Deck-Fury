using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerDashController : MonoBehaviour
{
    PlayerController player;
    StageManager stageManager;
    PlayerAnimationController animationController;

    [SerializeField] AnimationClip dashAnimation;

    [SerializeField] AimpointController aimpoint;
    [SerializeField] GameObject _dashReticle;
    public GameObject DashReticle {get { return _dashReticle; }}

    [SerializeField] TrailRenderer dashTrail;

[Header("Dash Indicator")]
    [SerializeField] GameObject dashIndicatorObject;
    [SerializeField] Image dashIndicatorImage;
    [SerializeField] SpriteRenderer dashIndicatorFrame;
    [SerializeField] Color onCooldownFrameColor;
    [SerializeField] Color dashReadyFrameColor;
    




[Header("Dash Variables")]
    [SerializeField, Min(0)] int dashDistance = 3;
    [SerializeField, Min(0)] float dashSpeed = 0.15f;
    [SerializeField, Min(0)] float dashCooldown;

    [SerializeField] EventReference dashSFX;

    public bool inputPressedDuringCooldown = false;

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
            _dashReticle.SetActive(true);
        }

        if(context.canceled)
        {
            DashTowardsAim();
            _dashReticle.SetActive(false);
        }

    }

    public bool CanDash()
    {
        if(usedDash)
        {
            return false;
        }

        return true;
    }


    public void DashTowardsAim()
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
                StartCoroutine(DashTrailTimer());

                animationController.PlayAnimationClip(dashAnimation);
                StartCoroutine(DashCooldown());

                RuntimeManager.PlayOneShot(dashSFX, transform.position);
                return;                        
            }else
            if(stageManager.CheckValidTile(destination - aimpoint.GetAimVector3Int()))
            {//Secondly check if the tile behind that is in the same direction of the leap is free and go there second.
                Vector3Int validPosition = destination - aimpoint.GetAimVector3Int();
                Vector3Int moveDistance = validPosition - player.currentTilePosition;
 
                StartCoroutine(DisableHitboxTimer());
                player.TweenMoveSetCoroutine(moveDistance.x, moveDistance.y, dashSpeed, Ease.OutBounce);
                StartCoroutine(DashTrailTimer());


                animationController.PlayAnimationClip(dashAnimation);
                StartCoroutine(DashCooldown());
                RuntimeManager.PlayOneShot(dashSFX, transform.position);

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
                    StartCoroutine(DashTrailTimer());

                    
                    animationController.PlayAnimationClip(dashAnimation);
                    StartCoroutine(DashCooldown());
                    RuntimeManager.PlayOneShot(dashSFX, transform.position);

                    return;
                }
            }
        }else
        {
            animationController.PlayAnimationClip(dashAnimation);            
            player.TweenMoveSetCoroutine(x, y, dashSpeed, Ease.OutCubic);
            StartCoroutine(DashTrailTimer());

            StartCoroutine(DisableHitboxTimer());
            StartCoroutine(DashCooldown());
            RuntimeManager.PlayOneShot(dashSFX, transform.position);

        }



    }

    IEnumerator DisableHitboxTimer()
    {
        player.playerCollider.enabled = false;
        yield return new WaitForSeconds(dashSpeed);
        player.playerCollider.enabled = true;        
    }

    IEnumerator DashTrailTimer()
    {
        dashTrail.Clear();
        dashTrail.enabled = true;
        yield return new WaitForSeconds(dashSpeed);
        dashTrail.enabled = false;

    }


    IEnumerator DashCooldown()
    {
        usedDash = true;

        dashIndicatorImage.fillAmount = 0;
        dashIndicatorFrame.color = onCooldownFrameColor;

        dashIndicatorObject.SetActive(true);
        dashIndicatorImage.DOFillAmount(1, dashCooldown).SetEase(Ease.Linear);


        yield return new WaitForSeconds(dashCooldown - 0.2f);
        dashIndicatorFrame.DOColor(dashReadyFrameColor, 0.2f); //Change frame color to indicate dash is ready
        yield return new WaitForSeconds(0.2f);

        dashIndicatorObject.SetActive(false);

        usedDash = false;
    }

}
