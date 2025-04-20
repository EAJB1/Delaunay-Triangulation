using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using Unity.AI.Navigation;

public class DataCollection : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] bool isUnity;
    
    [SerializeField] NavMesh navMesh;
    [SerializeField] string navMeshDataFilePath;

    [SerializeField] NavMeshSurface navMeshSurface;
    [SerializeField] string navMeshSurfaceDataFilePath;

    [Space]

    [SerializeField] int iterations;
    
    [Space]

    [SerializeField] double[] average;
    [SerializeField] double[] min;
    [SerializeField] double[] max;
    [SerializeField] double[] range;
    [SerializeField] double[] data;
    
    [Space]

    [SerializeField] double[] average2;
    [SerializeField] double[] min2;
    [SerializeField] double[] max2;
    [SerializeField] double[] range2;
    [SerializeField] double[] data2;

    [SerializeField] GameObject[] meshes;
    int index;

    private void Awake()
    {
        average = new double[meshes.Length];
        min = new double[meshes.Length];
        max = new double[meshes.Length];
        range = new double[meshes.Length];

        average2 = new double[meshes.Length];
        min2 = new double[meshes.Length];
        max2 = new double[meshes.Length];
        range2 = new double[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            index = i;
            Collect();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < meshes.Length; i++)
            {
                index = i;
                Collect();
            }
        }
    }

    private void Collect()
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            if (i == index)
            {
                meshes[index].SetActive(true);
                continue;
            }

            meshes[i].SetActive(false);
        }

        data = new double[iterations];

        double startTime = 0;
        double endTime = 0;

        if (!isUnity)
        {
            for (int i = 0; i < iterations; i++)
            {
                startTime = Time.realtimeSinceStartupAsDouble;

                navMesh.BakeNavigationMesh();

                endTime = Time.realtimeSinceStartupAsDouble;

                data[i] = endTime - startTime;

                if (i == iterations - 1)
                {
                    break;
                }

                navMesh.ClearNavigationMesh();
            }
        }
        else
        {
            for (int i = 0; i < iterations; i++)
            {
                startTime = Time.realtimeSinceStartupAsDouble;

                navMeshSurface.BuildNavMesh();

                endTime = Time.realtimeSinceStartupAsDouble;
                
                data[i] = endTime - startTime;

                if (i == iterations - 1)
                {
                    break;
                }

                UnityEngine.AI.NavMesh.RemoveAllNavMeshData();
            }
        }

        RecordData();
        SaveToFile();
    }

    private void RecordData()
    {
        List<double> sortedData = new List<double>(data);
        sortedData.Sort();
        data = sortedData.ToArray();

        average[index] = data.Average();
        min[index] = data[0];
        max[index] = data[data.Length - 1];
        range[index] = max[index] - min[index];

        var sortedIQData = data.Skip(data.Length / 4).Take(data.Length / 2).ToArray();
        data2 = sortedIQData;

        average2[index] = data2.Average();
        min2[index] = data2[0];
        max2[index] = data2[data2.Length - 1];
        range2[index] = max2[index] - min2[index];
    }

    private void SaveToFile()
    {
        StreamWriter writer = new StreamWriter(isUnity ? navMeshSurfaceDataFilePath : navMeshDataFilePath);

        writer.WriteLine("Meshes,Mean,Min,Max,Range,IQMean,IQMin,IQMax,IQRange");

        for (int i = 0; i < meshes.Length; i++)
        {
            writer.WriteLine($"{meshes[i].name},{average[i]},{min[i]},{max[i]},{range[i]},{average2[i]},{min2[i]},{max2[i]},{range2[i]}");
        }

        writer.Flush();
        writer.Close();
    }

    public GameObject[] FindObjectsByLayer(LayerMask layer)
    {
        // Get all objects in the scene.
        GameObject[] objectList = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        List<GameObject> whiteList = new();

        for (int i = 0; i < objectList.Length; i++)
        {
            // Both layers are equal.
            if ((layer & 1 << objectList[i].layer) != 0)
            {
                whiteList.Add(objectList[i]);
            }
        }

        return whiteList.ToArray();
    }
}
