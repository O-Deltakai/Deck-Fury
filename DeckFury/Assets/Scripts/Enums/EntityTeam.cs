using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player can damage Enemy/Neutral
/// Enemy can damage Player/Neutral
/// Neutral can damage everyone including themselves
/// </summary>
public enum EntityTeam
{
    Player,
    Enemy,
    Neutral
}
