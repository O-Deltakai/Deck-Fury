using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureChest : MonoBehaviour
{
    public enum RewardType
    {
        Card,
        Item,
        Money
    }

    [SerializeField] RewardType _rewardType;
    [SerializeField] AnimancerComponent animancer;
    [SerializeField] AnimationClip openAnimation;



    bool _isOpen = false;    


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.CompareTag(TagNames.Player.ToString()))
        {
            if(Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenChest();
            }
        }
    }

    private void OpenChest()
    {
        if(_isOpen){ return; }
        _isOpen = true;

        animancer.Play(openAnimation);

    }
}
