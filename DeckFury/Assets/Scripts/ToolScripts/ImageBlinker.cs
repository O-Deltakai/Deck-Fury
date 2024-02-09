using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
public class ImageBlinker : MonoBehaviour
{
    enum Mode{Image, CanvasGroup, Sprite}

    [SerializeField] Mode mode;
    [SerializeField] float blinkSpeed = 0.5f;
    [SerializeField, Range(0, 1)] float startingAlpha;
    [SerializeField, Range(0, 1)] float endingAlpha;
    [SerializeField] Ease blinkEase = Ease.Linear;
    [SerializeField] bool startBlinkingOnStart = false;

    public bool IsActive = false;
    Image image;
    CanvasGroup canvasGroup;
    SpriteRenderer spriteRenderer;

    Coroutine CR_BlinkerCoroutine;

    Transform[] childrenTransforms;

    void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get all Transform components in children, including the parent's
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        
        // Exclude the parent's Transform component
        childrenTransforms = allTransforms.Where(t => t != transform).ToArray();
    }

    void Start()
    {

        if(!image && !canvasGroup && !spriteRenderer)
        {
            Debug.LogError("No Image, SpriteRenderer or CanvasGroup component found on this GameObject, ImageBlinker will not function.");
            return;
        }

        if(startBlinkingOnStart)
        {
            StartBlinking();
        }


    }

    void OnEnable()
    {
        if(IsActive)
        {
            StartBlinking();
        }
    }

    void OnDisable()
    {
        if(CR_BlinkerCoroutine != null)
        {
            StopCoroutine(CR_BlinkerCoroutine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBlinking()
    {
        if(CR_BlinkerCoroutine != null)
        {
            StopCoroutine(CR_BlinkerCoroutine);
        }
        CR_BlinkerCoroutine = StartCoroutine(BlinkerCoroutine(mode));
    }

    IEnumerator BlinkerCoroutine(Mode mode)
    {
        if (mode == Mode.Image)
        {
            if(!image)
            {
                Debug.LogError("No Image component found on this GameObject, cannot blink image");
                yield break;
            }
            while (true)
            {   
                image.DOFade(startingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                yield return new WaitForSecondsRealtime(blinkSpeed);
                image.DOFade(endingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                yield return new WaitForSecondsRealtime(blinkSpeed);
            }
        }else
        if(mode == Mode.CanvasGroup)
        {
            if(!canvasGroup)
            {
                Debug.LogError("No CanvasGroup component found on this GameObject, cannot blink canvas group");
                yield break;
            }
            while (true)
            {
                canvasGroup.DOFade(startingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                TweenChildrenTransform(1.1f);
                yield return new WaitForSecondsRealtime(blinkSpeed);
                canvasGroup.DOFade(endingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                TweenChildrenTransform(1f);
                yield return new WaitForSecondsRealtime(blinkSpeed);
            }
        }else
        {
            if(!spriteRenderer)
            {
                Debug.LogError("No SpriteRenderer component found on this GameObject, cannot blink sprite");
                yield break;
            }
            while (true)
            {
                spriteRenderer.DOFade(startingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                yield return new WaitForSecondsRealtime(blinkSpeed);
                spriteRenderer.DOFade(endingAlpha, blinkSpeed).SetEase(blinkEase).SetUpdate(true);
                yield return new WaitForSecondsRealtime(blinkSpeed);
            }
        }

    }

    void TweenChildrenTransform(float scale)
    {
        foreach (var childTransform in childrenTransforms)
        {
            childTransform.DOScale(scale, blinkSpeed).SetEase(Ease.InOutSine).SetUpdate(true);
        }
    }


}
