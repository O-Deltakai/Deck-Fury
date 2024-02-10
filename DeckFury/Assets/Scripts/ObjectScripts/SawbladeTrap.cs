using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawbladeTrap : MonoBehaviour
{
    enum TravelMode {Independent, Fixed}

    [Tooltip("The order of nodes the sawblade will travel to.")]
    public List<Transform> pathNodes = new();

    [Tooltip("The speed at which the sawblade will travel.")]
    public float speed = 5f;

    int currentNodeIndex = 0;

    private void OnDrawGizmos()
    {
        if (pathNodes.Count == 0) return;

        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (i < pathNodes.Count - 1)
            {
                Gizmos.DrawLine(pathNodes[i].position, pathNodes[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(pathNodes[i].position, pathNodes[0].position);
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (pathNodes.Count == 0) return;

        Transform targetNode = pathNodes[currentNodeIndex];
        Vector3 direction = (targetNode.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetNode.position) < 0.1f)
        {
            currentNodeIndex++;
            if (currentNodeIndex >= pathNodes.Count)
            {
                currentNodeIndex = 0; // Loop back to the start of the path
                // Or you can stop the movement by removing or disabling this script
            }
        }        
    }

}
