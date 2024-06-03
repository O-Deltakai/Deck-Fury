using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// A generic component that allows an object to traverse between waypoints with tweening
/// </summary>
public class UIWaypointTraverser : MonoBehaviour
{
    [Serializable]
    public class Waypoint
    {
        public string name;
        public Vector2 position;
        public bool useCustomEase = false;
        public Ease customEase = Ease.Linear;
        public bool setStateAtStart = false;
        public bool activeState = false;
    }

    public enum InitializeMode
    {
        OnStart,
        OnAwake,
        Manual
    }


    public List<Waypoint> waypoints;
    public float tweenDuration = 1f;
    public Ease tweenEase = Ease.Linear;
    public bool ignoreTimeScale = true;

    int _currentWaypointIndex = 0;
    public int CurrentWaypointIndex => _currentWaypointIndex;
    Tween currentTween;

    [Tooltip("Determines if and when the object should be set at the initial waypoint")]
    public InitializeMode initializeMode = InitializeMode.Manual;
    [Tooltip("The waypoint to set the object at when initialize mode is set to OnStart or OnAwake. Ignored if initialize mode is set to Manual")]
    [SerializeField] public int initialWaypointIndex;

    public bool IsTweening => currentTween.IsActive();

    void Awake()
    {
        if(initializeMode == InitializeMode.OnAwake)
        {
            SetAtWaypoint(initialWaypointIndex);
        }
    }

    void Start()
    {
        if(initializeMode == InitializeMode.OnStart)
        {
            SetAtWaypoint(initialWaypointIndex);
        }
    }

    public void SetAtWaypoint(Waypoint waypoint)
    {
        if(currentTween.IsActive())
        {
            currentTween.Kill();
        }
        transform.localPosition = waypoint.position;
    }

    public void SetAtWaypoint(int waypointIndex)
    {
        if(currentTween.IsActive())
        {
            currentTween.Kill();
        }

        if(waypointIndex < 0 || waypointIndex >= waypoints.Count)
        {
            Debug.LogError("Invalid waypoint index");
            return;
        }

        transform.localPosition = waypoints[waypointIndex].position;
    }

    public void TraverseToWaypoint(int waypointIndex)
    {
        if(currentTween.IsActive())
        {
            return;
        }

        if(waypointIndex < 0 || waypointIndex >= waypoints.Count)
        {
            Debug.LogError("Invalid waypoint index");
            return;
        }

        //Move the object to the waypoint with tweening
        if(waypoints[waypointIndex].setStateAtStart)
        {
            gameObject.SetActive(waypoints[waypointIndex].activeState);
            currentTween = transform.DOLocalMove(waypoints[waypointIndex].position, tweenDuration).SetUpdate(ignoreTimeScale);
        }else
        {
            currentTween = transform.DOLocalMove(waypoints[waypointIndex].position, tweenDuration)
            .OnComplete(() => gameObject.SetActive(waypoints[waypointIndex].activeState)).SetUpdate(ignoreTimeScale);
        }

        if(waypoints[waypointIndex].useCustomEase)
        {
            currentTween.SetEase(waypoints[waypointIndex].customEase);
        }else
        {
            currentTween.SetEase(tweenEase);
        }

    }

    public void TraverseToWaypoint(Waypoint waypoint)
    {
        if(currentTween.IsActive())
        {
            return;
        }

        if(waypoint == null)
        {
            Debug.LogError("Invalid waypoint");
            return;
        }

        //Move the object to the waypoint with tweening
        if(waypoint.setStateAtStart)
        {
            gameObject.SetActive(waypoint.activeState);
            currentTween = transform.DOLocalMove(waypoint.position, tweenDuration).SetUpdate(ignoreTimeScale);
        }else
        {
            currentTween = transform.DOLocalMove(waypoint.position, tweenDuration)
            .OnComplete(() => gameObject.SetActive(waypoint.activeState)).SetUpdate(ignoreTimeScale);
        }

        if(waypoint.useCustomEase)
        {
            currentTween.SetEase(waypoint.customEase);
        }else
        {
            currentTween.SetEase(tweenEase);
        }
    }

    /// <summary>
    /// Cycles through the waypoints in the waypoints list
    /// </summary>
    public void CycleWaypoint()
    {
        if(currentTween.IsActive())
        {
            return;
        }

        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
        TraverseToWaypoint(_currentWaypointIndex);
    }

    /// <summary>
    /// Reverses the cycle through the waypoints in the waypoints list
    /// </summary>
    public void ReverseCycleWaypoint()
    {
        if(currentTween.IsActive())
        {
            return;
        }

        _currentWaypointIndex = (_currentWaypointIndex - 1) % waypoints.Count;
        if(_currentWaypointIndex < 0)
        {
            _currentWaypointIndex = waypoints.Count - 1;
        }
        TraverseToWaypoint(_currentWaypointIndex);
    }


    public void TraverseToWaypoint(string waypointName)
    {
        if(currentTween.IsActive())
        {
            return;
        }

        if(string.IsNullOrEmpty(waypointName))
        {
            Debug.LogError("Invalid waypoint name");
            return;
        }

        Waypoint waypoint = waypoints.Find(x => x.name == waypointName);
        if(waypoint == null)
        {
            waypoint = FindClosestMatch(waypointName, waypoints);
            Debug.LogWarning("Waypoint with name " + waypointName + " not found," + "closest match found is " + waypoint.name);
        }

        //Move the object to the waypoint with tweening
        if(waypoint.setStateAtStart)
        {
            gameObject.SetActive(waypoint.activeState);
            currentTween = transform.DOLocalMove(waypoint.position, tweenDuration).SetUpdate(ignoreTimeScale);
        }else
        {
            currentTween = transform.DOLocalMove(waypoint.position, tweenDuration).OnComplete(() => gameObject.SetActive(waypoint.activeState))
            .SetUpdate(ignoreTimeScale);
        }

        if(waypoint.useCustomEase)
        {
            currentTween.SetEase(waypoint.customEase);
        }else
        {
            currentTween.SetEase(tweenEase);
        }


    }

    private Waypoint FindClosestMatch(string input, List<Waypoint> collection)
    {
        int distance = int.MaxValue;
        Waypoint closestMatch = null;

        foreach (Waypoint item in collection)
        {
            int tempDistance = LevenshteinDistance(input, item.name);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                closestMatch = item;
            }
        }

        return closestMatch;
    }

    private int LevenshteinDistance(string a, string b)
    {
        var matrix = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            matrix[i, 0] = i;
        for (int j = 0; j <= b.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                matrix[i, j] = Math.Min(Math.Min(
                    matrix[i - 1, j] + 1,
                    matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));

        return matrix[a.Length, b.Length];
    }



}

