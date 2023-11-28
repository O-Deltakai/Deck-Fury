using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : NPC
{
    enum VampireAnims
    {
        Vampire_Idle = 0,
        Vampire_Attack = 1,
        Vampire_Spawn = 2,
        Vampire_StrongATK = 3
    }

//The current mood of the Magician defines what attacks they will do and how quickly they move.
    enum Mood {Normal, Angry, Desperate}

    [SerializeField] Mood currentMood;

    //Percentage of starting hp before they enter Angry/Desperate mood.
    [SerializeField, Range(0, 1)] float angryThreshold;
    [SerializeField, Range(0, 1)] float desperateThreshold;

    int startingHP;





}
