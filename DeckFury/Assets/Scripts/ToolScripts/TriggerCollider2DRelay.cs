using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Intermediary object to allow for trigger 2D colliders on different objects to communicate to other objects via events
/// without needing one to be the parent and have a rigidbody.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TriggerCollider2DRelay : MonoBehaviour
{
    public event Action<Collider2D> OnTriggerEnter2DRelay;
    public event Action<Collider2D> OnTriggerExit2DRelay;
    public event Action<Collider2D> OnTriggerStay2DRelay;

    void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter2DRelay?.Invoke(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit2DRelay?.Invoke(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerStay2DRelay?.Invoke(other);
    }

}
