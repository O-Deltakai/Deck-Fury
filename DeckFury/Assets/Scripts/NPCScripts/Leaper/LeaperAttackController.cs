using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaperAttackController : MonoBehaviour
{
    [SerializeField] Leaper leaper;
    [SerializeField] AttackPayload payload;
    [SerializeField] List<BoxCollider2D> attackHitboxes;
    [SerializeField] AnimationClip VFXAnimation;


    void Awake() 
    {
        DisableHitboxes();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StageEntity player = other.gameObject.GetComponent<StageEntity>();
            player.HurtEntity(payload);
        }    
    }


    public void AttemptAttack()
    {
        StartCoroutine(TriggerAttackHitBoxes());
    }

    public IEnumerator TriggerAttackHitBoxes()
    {
        StartCoroutine(AnimateShockVFX());

        float delay = 0.1f;
        yield return new WaitForSeconds(delay);
        ActivateHitboxes();
        yield return new WaitForSeconds(VFXAnimation.length - delay * 1.5f);
        DisableHitboxes();

    }

    IEnumerator AnimateShockVFX()
    {
        foreach(BoxCollider2D hitbox in attackHitboxes)
        {
            SpriteRenderer vfxRenderer = hitbox.GetComponent<SpriteRenderer>();
            Animator vfxAnimator = hitbox.GetComponent<Animator>();

            vfxRenderer.enabled = true;
            vfxAnimator.enabled = true;
        }        

        yield return new WaitForSeconds(VFXAnimation.length);

        foreach(BoxCollider2D hitbox in attackHitboxes)
        {
            SpriteRenderer vfxRenderer = hitbox.GetComponent<SpriteRenderer>();
            Animator vfxAnimator = hitbox.GetComponent<Animator>();

            vfxRenderer.enabled = false;
            vfxAnimator.enabled = false;
        } 

    }

    void ActivateHitboxes()
    {
        foreach(BoxCollider2D hitbox in attackHitboxes)
        {
            hitbox.enabled = true;
        }
    }

    void DisableHitboxes()
    {
        foreach(BoxCollider2D hitbox in attackHitboxes)
        {
            hitbox.enabled = false;
        }
    }    


}
