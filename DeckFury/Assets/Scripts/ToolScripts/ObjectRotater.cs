using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that continuously rotates an object around a specified axis at a specified speed and direction
/// </summary>
public class ObjectRotater : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [SerializeField] RotationAxis _rotationAxis = RotationAxis.Z;
    public RotationAxis rotationAxis
    {
        get { return _rotationAxis; }
        set
        {
            _rotationAxis = value;
            SetRotationAxis();
        }
    }
    [SerializeField] RotationDirection _rotationDirection = RotationDirection.Clockwise;
    public RotationDirection rotationDirection
    {
        get { return _rotationDirection; }
        set
        {
            _rotationDirection = value;
            SetRotationDirection();
        }
    }
    public float rotationSpeed = 10f;

    Vector3 currentRotationAxis = Vector3.zero;
    float currentRotationDirection = 1f;

    void OnValidate()
    {
        SetRotationAxis();
        SetRotationDirection();
    }

    void SetRotationAxis()
    {
        switch(_rotationAxis)
        {
            case RotationAxis.X:
                currentRotationAxis = Vector3.right;
                break;
            case RotationAxis.Y:
                currentRotationAxis = Vector3.up;
                break;
            case RotationAxis.Z:
                currentRotationAxis = Vector3.forward;
                break;
        }
    }

    void SetRotationDirection()
    {
        switch(_rotationDirection)
        {
            case RotationDirection.Clockwise:
                currentRotationDirection = 1f;
                break;
            case RotationDirection.CounterClockwise:
                currentRotationDirection = -1f;
                break;
        }
    }

    void Update()
    {
        RotateObject();
    }

    void RotateObject()
    {
        transform.Rotate(currentRotationAxis, Time.deltaTime * rotationSpeed * currentRotationDirection);
    }


}
