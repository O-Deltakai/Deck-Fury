using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShotgunBlast : CardEffect
{
    CinemachineImpulseSource impulseSource;
    [SerializeField] AnimationClip VFXAnimationClip;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>(); 
    }


    public override void ActivateCardEffect()
    {
        AimpointController aimpoint = player.aimpoint;
        FaceTowardsAimpoint(aimpoint);

        if(impulseSource)
        {
            impulseSource.GenerateImpulseWithVelocity(impulseSource.m_DefaultVelocity * SettingsManager.GlobalCameraShakeMultiplier);
        }


        StartCoroutine(DisableEffectPrefab());
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.CompareTag("EnvironmentalHazard"))
        {
            StageEntity entity = other.gameObject.GetComponent<StageEntity>();
            entity.HurtEntity(attackPayload);
                        
        }

    }

    //Rotate the object towards the aimpoint so that it fires in the direction of the aimpoint
    void FaceTowardsAimpoint(AimpointController aimpoint)
    {
        switch (aimpoint.currentAimDirection) 
        {
            case AimDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;

            case AimDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break; 
            case AimDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;

            case AimDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(VFXAnimationClip.length);
        gameObject.SetActive(false);
        
    }


}
