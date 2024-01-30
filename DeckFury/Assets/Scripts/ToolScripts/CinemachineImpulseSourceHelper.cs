using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CinemachineImpulseSourceHelper : MonoBehaviour
{
    CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }


    public void ShakeCameraRandomCircle(Vector2 velocity, float duration, float force)
    {
        if(impulseSource)
        {
            // Randomize direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Ensure significant magnitude for either x or y
            float magnitudeX = Random.Range(velocity.x * 0.8f, velocity.x * 1.2f);
            float magnitudeY = Random.Range(velocity.y * 0.8f, velocity.y * 1.2f);

            // Apply magnitude to the direction
            Vector2 shakeVelocity = new Vector2
            (
                randomDirection.x * magnitudeX, 
                randomDirection.y * magnitudeY 
            );

            impulseSource.m_DefaultVelocity = shakeVelocity;

            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
            impulseSource.GenerateImpulseWithForce(force);
        }
    }

}
