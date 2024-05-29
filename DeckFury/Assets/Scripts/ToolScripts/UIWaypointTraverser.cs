using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UIWaypointTraverser : MonoBehaviour
{
    [Serializable]
    public class Waypoint
    {
        public string name;
        public Vector2 position;
        public bool useCustomEase;
        public Ease customEase;
        public bool setStateAtStart;
        public bool activeState;
    }


    public List<Waypoint> waypoints;
    public float tweenDuration = 1f;
    public Ease tweenEase = Ease.Linear;
    public bool ignoreTimeScale = true;

    int currentWaypointIndex = 0;

    RectTransform rectTransform;

    Tween currentTween;



    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        
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

    public void CycleWaypoint()
    {
        if(currentTween.IsActive())
        {
            return;
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        TraverseToWaypoint(currentWaypointIndex);
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

