using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCBehaviour : MonoBehaviour
{
    protected NPC npc;
    protected EntityAnimationController entityAnimator => npc.EntityAnimator;


    protected PlayerController Player => npc.player;
    protected Vector3Int CurrentTilePosition => npc.currentTilePosition;
    protected StageManager _stageManager;


    protected bool CanAct => npc.CanAct;


    void Awake() 
    {
        npc = GetComponent<NPC>();
    }

    void Start()
    {
        _stageManager = StageManager.Instance;
    }


}
