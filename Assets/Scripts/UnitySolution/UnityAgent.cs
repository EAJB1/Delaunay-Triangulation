using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnityAgent : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [Space]

    [SerializeField] Vector3 targetPosition;

    /// <summary>
    /// Calculate and return path for NavMesh agent.
    /// </summary>
    public NavMeshPath GetPath()
    {
        NavMeshPath path = new();

        agent.CalculatePath(targetPosition, path);

        return path;
    }
}
