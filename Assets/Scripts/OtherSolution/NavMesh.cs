using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NavMesh : MonoBehaviour
{
    [Tooltip("Layer mask for objects to be included in the NavMesh.")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] float vertexMergeDistance;

    [Space]

    public GameObject[] navMeshObjects;
    [SerializeField] List<Vector3> objectPoints = new();
    [SerializeField] Vertex[] vertices;
    [SerializeField] Triangle[] triangles;

    Mesh navMesh;

    public void BakeNavigationMesh()
    {
        navMeshObjects = FindActiveObjectsByLayer(layerMask);

        objectPoints = GetVerticesFromObjects(navMeshObjects);

        //objectPoints = objectPoints.Distinct().ToList();

        objectPoints = RemoveDuplicateVertices(objectPoints);

        vertices = VectorsToVertices(objectPoints.ToArray());

        DelaunayTriangulation triangulation = new DelaunayTriangulation(vertices);

        vertices = triangulation.vertices;
        triangles = triangulation.triangles;

        CreateMesh(triangulation);
    }

    public void ClearNavigationMesh()
    {
        navMesh.Clear();
    }

    private GameObject[] FindActiveObjectsByLayer(LayerMask layer)
    {
        // Get all objects in the scene.
        GameObject[] objectList = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        List<GameObject> whiteList = new();

        for (int i = 0; i < objectList.Length; i++)
        {
            if (!objectList[i].activeSelf) continue;

            // Both layers are equal.
            if ((layer & 1 << objectList[i].layer) != 0)
            {
                whiteList.Add(objectList[i]);
            }
        }

        return whiteList.ToArray();
    }

    /// <summary>
    /// Iterate through each object and add each vertex in the object to a list of vertices.
    /// </summary>
    private List<Vector3> GetVerticesFromObjects(GameObject[] gameObjects)
    {
        List<Vector3> points = new();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            MeshFilter meshFilter = gameObjects[i].GetComponent<MeshFilter>();

            if (!meshFilter)
            {
                Debug.LogWarning("GameObject does not contain a MeshFilter: " + gameObjects[i].name);
                continue;
            }

            foreach (Vector3 point in meshFilter.sharedMesh.vertices)
            {
                points.Add(gameObjects[i].transform.TransformPoint(point));
            }
        }

        return points;
    }

    private List<Vector3> RemoveDuplicateVertices(List<Vector3> points)
    {
        List<Vector3> newObjectPoints = new List<Vector3>();

        foreach (Vector3 p in points)
        {
            bool valid = true;

            foreach (Vector3 q in newObjectPoints)
            {
                if (Vector2.Distance(new Vector2(p.x, p.z), new Vector2(q.x, q.z)) <= vertexMergeDistance)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                newObjectPoints.Add(p);
            }
        }

        return newObjectPoints;
    } 

    private Vertex[] VectorsToVertices(Vector3[] points)
    {
        // Set new size
        Vertex[] vertices = new Vertex[points.Length];

        // Remap y to z
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vertex(points[i].x, points[i].z, i);
        }

        return vertices;
    }

    private Vector3[] VerticesToVectors(Vertex[] vertices)
    {
        // Set new size
        Vector3[] vectors = new Vector3[vertices.Length];

        // Remap z to y
        for (int i = 0; i < vectors.Length; i++)
        {
            vectors[i] = new Vector3(vertices[i].x, 0f, vertices[i].y);
        }

        return vectors;
    }

    private void CreateMesh(DelaunayTriangulation triangulation)
    {
        navMesh = new Mesh();
        navMesh.name = "NavMesh";

        navMesh.vertices = VerticesToVectors(triangulation.vertices);

        // Make vertices local to the transform
        for (int i = 0; i < navMesh.vertices.Length; i++)
        {
            navMesh.vertices[i] = transform.InverseTransformPoint(navMesh.vertices[i]);
        }

        int[] indices = new int[triangulation.triangles.Length * 3];

        // Set three vertex indices for each triangle
        for (int i = 0; i < triangulation.triangles.Length; i++)
        {
            indices[i * 3] = triangulation.triangles[i].vertex0.i;
            indices[i * 3 + 1] = triangulation.triangles[i].vertex1.i;
            indices[i * 3 + 2] = triangulation.triangles[i].vertex2.i;
        }

        navMesh.triangles = indices;

        GetComponent<MeshFilter>().mesh = navMesh;
    }

    private void OnDrawGizmos()
    {
        if (!navMesh || navMesh.vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;

        for (int i = 0; i < navMesh.vertices.Length; i++)
        {
            Gizmos.DrawSphere(new Vector3(navMesh.vertices[i].x, 0f, navMesh.vertices[i].z), 0.025f);
        }

        Vector3[] vertices = navMesh.vertices;
        int[] triangles = navMesh.triangles;

        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0 || i % 3 == 1)
            {
                Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i + 1]]);
                continue;
            }

            Gizmos.DrawLine(vertices[triangles[i]], vertices[triangles[i - 2]]);
        }
    }
}
