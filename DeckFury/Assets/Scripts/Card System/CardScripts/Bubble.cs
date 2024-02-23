using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : CardEffect
{
    [SerializeField] GameObject bubblePrefab;

    void OnValidate()
    {
        if(bubblePrefab)
        {
            if(!bubblePrefab.TryGetComponent<BubbleProjectile>(out _))
            {
                Debug.LogError("Bubble prefab must have a BubbleProjectile component");
            }
        }
    }

    public override void ActivateCardEffect()
    {
        BubbleProjectile bubble = Instantiate(bubblePrefab, player.currentTilePosition + player.aimpoint.GetAimVector3Int(),
        Quaternion.identity).GetComponent<BubbleProjectile>();
        bubble.attackPayload = attackPayload;
        bubble.direction = player.aimpoint.GetAimVector3Int();
    }


    protected override IEnumerator DisableEffectPrefab()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
