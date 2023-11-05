using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBasicAttack : MonoBehaviour
{
    Zombie zombie;

    public AbilityData abilityData;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            StageEntity entity = other.GetComponent<StageEntity>();
            entity.HurtEntity(abilityData.attackPayload);
        }

    }

    public void FaceTowardsAimpoint(AimDirection aimDirection)
    {
        switch (aimDirection) 
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

    private void OnEnable()
    {
        StartCoroutine(SelfDisable());    
    }

    IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(0.15f);
        gameObject.SetActive(false);
    }


}
