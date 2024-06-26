using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructTimer : MonoBehaviour
{
    public bool beginCountdownOnStart = false;
    [Min(0)] public float delay = 1f;
    public bool useUnscaledTime = false;

    Coroutine CR_DestructionCoroutine = null;

    void Start()
    {
        if(beginCountdownOnStart)
        {
            BeginCountdown();
        }
    }


    public void BeginCountdown()
    {
        if(!gameObject.activeInHierarchy) { return; }
        if(CR_DestructionCoroutine != null) { return; }
        CR_DestructionCoroutine = StartCoroutine(DestructionTimer());
    }

    IEnumerator DestructionTimer()
    {
        if(useUnscaledTime)
        {
            yield return new WaitForSecondsRealtime(delay);
        }else
        {
            yield return new WaitForSeconds(delay);
        }

        Destroy(gameObject);
    }

}
