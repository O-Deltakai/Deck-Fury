using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImageGenerator : MonoBehaviour
{
    SpriteRenderer objectSpriteRenderer;

    [SerializeField] bool playOnStart;
    [SerializeField] bool useUnscaledTime;

    [SerializeField, Min(0.05f)] float frequency = 0.5f;
    [SerializeField, Min(0)] float lifeTime = 0.5f;
    [SerializeField, Min(0)] float fadeDuration = 0.5f;
    [SerializeField] Color afterImageColor;

    [SerializeField] Ease fadeEase = Ease.Linear;


    Coroutine CR_SpawningCoroutine;


    void Awake()
    {
        objectSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if(playOnStart)
        {
            Play();
        }
    }

    void InitializeObjectPool()
    {

    }


    void SpawnImageUnpooled()
    {
        GameObject imageObject = new GameObject(gameObject.name + "_afterImage");

        imageObject.transform.position = transform.position;
        imageObject.transform.localScale = transform.localScale;

        SpriteRenderer imageRenderer = imageObject.AddComponent<SpriteRenderer>();
        SelfDestructTimer destructTimer = imageObject.AddComponent<SelfDestructTimer>();
        imageRenderer.sprite = objectSpriteRenderer.sprite;
        imageRenderer.color = afterImageColor;


        imageRenderer.DOFade(0, fadeDuration).SetUpdate(true);

        destructTimer.delay = lifeTime;
        destructTimer.useUnscaledTime = useUnscaledTime;

        destructTimer.BeginCountdown();

        

    }

    IEnumerator SpawningCoroutine()
    {
        while(true)
        {
            if(useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(frequency);
            }else
            {
                yield return new WaitForSeconds(frequency);
            }

            SpawnImageUnpooled();

        }


    }

    IEnumerator DestroyImageCoroutine(GameObject objectToDestroy)
    {
        if(useUnscaledTime)
        {
            yield return new WaitForSecondsRealtime(lifeTime);
        }else
        {
            yield return new WaitForSeconds(lifeTime);
        }

        Destroy(objectToDestroy);
    }


    public void Play()
    {
        if(CR_SpawningCoroutine != null)
        {
            StopCoroutine(CR_SpawningCoroutine);
        }
        CR_SpawningCoroutine = StartCoroutine(SpawningCoroutine());
    }

    public void Stop()
    {
        if(CR_SpawningCoroutine != null)
        {
            StopCoroutine(CR_SpawningCoroutine);
        }        
    }


    void OnDisable()
    {
        Stop();
    }

}
