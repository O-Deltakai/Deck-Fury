using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpikeTileHazard : MonoBehaviour
{

    const string SPIKETRAP_ANIM_NAME = "SpikeTrapAnimation";
    const string SPIKETRAPREVERSE_ANIM_NAME = "SpikeTrapAnimationReverse";


    BoxCollider2D spikesCollider;
    Animator spikesAnimator;

    [SerializeField] AttackPayload attackPayload;

    bool hasAttacked = false;

    private void Awake() 
    {
        spikesCollider = GetComponent<BoxCollider2D>();
        spikesAnimator = GetComponent<Animator>();

    }

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //What happens if an entity walks into the collider of the trap?
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(hasAttacked){return;}

        StageEntity entityHit = other.gameObject.GetComponent<StageEntity>();

        if (entityHit != null)
        {
            PlayTrapAnimation(SPIKETRAP_ANIM_NAME);
            hasAttacked = true;

            entityHit.HurtEntity(attackPayload);
            if(entityHit.CurrentHP <= 0)
            {
                StartCoroutine(ReverseSpike());
            }
            

        }
    }

    IEnumerator ReverseSpike()
    {
        yield return new WaitForSeconds(spikesAnimator.GetCurrentAnimatorStateInfo(0).length);
        PlayTrapAnimation(SPIKETRAPREVERSE_ANIM_NAME);
        
        yield return new WaitForSeconds(spikesAnimator.GetCurrentAnimatorStateInfo(0).length);
        hasAttacked = false;
    }


    private void OnTriggerExit2D()
    {
        StartCoroutine(ReverseSpike());
    }

    //Use one of the const strings to play an animation
    void PlayTrapAnimation(string animationName)
    {
        spikesAnimator.Play(animationName);


    }



}
