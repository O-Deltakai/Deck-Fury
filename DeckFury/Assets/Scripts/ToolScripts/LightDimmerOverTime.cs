using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightDimmerOverTime : MonoBehaviour
{
    [SerializeField] AnimationCurve lightDimCurve;
    [SerializeField] Light2D light2D;
    [SerializeField] float duration = 1f;

    public bool dimOnStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if(dimOnStart)
        {
            DimLightOverTime();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DimLightOverTime()
    {
        StartCoroutine(DimLight());
    }

    IEnumerator DimLight()
    {
        float timer = 0;
        float startIntensity = light2D.intensity;
        while(timer < duration)
        {
            light2D.intensity = lightDimCurve.Evaluate(timer / duration) * startIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
    }


}
