using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityAnimationController : MonoBehaviour
{
    Animator animator;
    [field:SerializeField] public AnimationClip IdleAnimation {get; protected set;}
    [field:SerializeField] public AnimationClip DefeatAnimation{get; protected set;}
    
    [field:SerializeField] public List<AnimationClip> animationList {get; protected set;}

    //Bool for checking if an animation is already playing (that is not the idle animation)
    public bool isAnimating;
    Coroutine returnToIdleCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();

    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    
    /// <summary>
    /// Plays an animation based on the given animation clip's name, then returns to idle animation after the animation clip's
    /// length. Will return if an animation clip is already playing to avoid animation cancelling.
    /// </summary>
    /// <param name="animationClip"></param>
    public void PlayAnimationClip(AnimationClip animationClip)
    {
        if(isAnimating){return;}

        animator.Play(animationClip.name);
        isAnimating = true;
        returnToIdleCoroutine = StartCoroutine(ReturnToIdle(animationClip.length));
    }

 
    /// <summary>
    /// Plays an animation clip that ignores the isAnimating bool and does not return to idle.
    /// </summary>
    /// <param name="animationClip"></param>
    public void PlayOneShotAnimation(AnimationClip animationClip)
    {
        if(returnToIdleCoroutine != null)
        {
            StopCoroutine(returnToIdleCoroutine);
        }
        
        animator.Play(animationClip.name);   
    }


    /// <summary>
    /// Plays an animation clip that ignores isAnimating bool and then returns to idle.
    /// </summary>
    /// <param name="animationClip"></param>
    public void PlayOneShotAnimationReturnIdle(AnimationClip animationClip)
    {
        if(returnToIdleCoroutine != null)
        {
            StopCoroutine(returnToIdleCoroutine);
        }
        animator.Play(animationClip.name);   
        returnToIdleCoroutine = StartCoroutine(ReturnToIdle(animationClip.length));
    }



    //Waits the given duration in seconds and then plays the predefined idle animation clip
    IEnumerator ReturnToIdle(float duration)
    {

        yield return new WaitForSeconds(duration);
        isAnimating = false;
        animator.Play(IdleAnimation.name);
        returnToIdleCoroutine = null;

    }
}
