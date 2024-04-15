using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : CardEffect
{
    [SerializeField] GameObject lightningOrbPrefab;



    protected override IEnumerator DisableEffectPrefab()
    {
        yield break;
    }
}
