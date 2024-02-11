using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class VFXAnimationController : MonoBehaviour
{
    public AnimationClip[] animations; // Array to hold your animations
    public int selectedAnimationIndex = 0; // Default or selected animation index

    AnimancerComponent _animancer;

    void Awake()
    {
        _animancer = GetComponent<AnimancerComponent>();
    }

    void OnEnable()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        AnimancerState animancerState = _animancer.Play(animations[selectedAnimationIndex]);
    }




}

