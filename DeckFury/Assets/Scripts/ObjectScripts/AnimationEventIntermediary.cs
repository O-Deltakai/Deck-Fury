using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which allows GameObjects with animators that use Animation Events to communicate with objects that are not immediately
/// attached to the GameObject.
/// </summary>
public class AnimationEventIntermediary : MonoBehaviour
{
    public delegate void AnimationEventHandler();
    public AnimationEventHandler OnAnimationEvent;

    public delegate void IndexedAnimationEventHandler(int index);
    public IndexedAnimationEventHandler OnIndexedAnimationEvent;

    [SerializeField] GameObject ObjectToSendMessageTo;

    void NotifySubscribers()
    {
        OnAnimationEvent?.Invoke();
    }

    //Use this method when you have an animation that have multiple unique animation events - will require a gatekeeper (dispatcher) reciever method that
    //can switch between different methods depending on index.
    void IndexedNotifySubscribers(int index)
    {
        OnIndexedAnimationEvent?.Invoke(index);
    }

}
