using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnityManager : MonoBehaviour
{
    [SerializeField] UnityAgent unityAgent;

    [Space]

    [SerializeField] int numberOfPathsCalculated;

    [Space]

    [SerializeField] List<NavMeshPath> paths = new();

    bool running;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (running)
            {
                return;
            }

            StartCoroutine(RunPathfinding());
        }
    }

    IEnumerator RunPathfinding()
    {
        paths.Clear();

        running = true;

        for (int i = 0; i < numberOfPathsCalculated; i++)
        {
            paths.Add(unityAgent.GetPath());

            yield return null;
        }

        for (int i = 0; i < paths.Count; i++)
        {
            Debug.Log("Path No. " + i + " | Path Status: " + paths[i].status + " | Corner Count: " + paths[i].corners.Length);
        }

        running = false;
    }
}
