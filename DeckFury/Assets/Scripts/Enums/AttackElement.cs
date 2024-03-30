using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackElement
{
    Neutral,
    Blade,
    Fire,
    Water,
    Electric,
    Breaking,
    Pure, //Pure damage completely bypasses resistances of all kinds and ignores shields, dealing damage directly to a target's HP.
    NotApplicable, // A special element that should is only used for specific cases in code where the element is meant to be null or not applicable. 

}


