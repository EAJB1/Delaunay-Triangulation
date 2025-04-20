using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelaunayTriangulation
{
    public Vertex[] vertices;
    public Triangle[] triangles;

    public DelaunayTriangulation(Vertex[] _vertices)
    {
        vertices = _vertices;
        triangles = Triangulate(vertices);
    }

    public Triangle[] Triangulate(Vertex[] vertices)
    {
        if (vertices.Length < 3)
        {
            Debug.LogError("Need 3 vertices for triangulation.");
            return null;
        }

        // Create super triangle
        Triangle super = CalculateSuperTriangle(vertices);
        
        // New array of triangles that accounts for the size of the vertices and the super triangle
        Triangle[] triangles = new Triangle[1]; // Mathf.RoundToInt((float)vertices.Length / 3f) + 1
        triangles[0] = super;

        // Triangulate all vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            triangles = AddVertex(vertices[i], triangles);
        }

        List<Triangle> validTriangles = new List<Triangle>();

        // Remove triangles that share vertices with super triangle
        for (int i = 0; i < triangles.Length; i++)
        {
            if (triangles[i].vertex0 == super.vertex0 || triangles[i].vertex0 == super.vertex1 || triangles[i].vertex0 == super.vertex2 ||
                triangles[i].vertex1 == super.vertex0 || triangles[i].vertex1 == super.vertex1 || triangles[i].vertex1 == super.vertex2 ||
                triangles[i].vertex2 == super.vertex0 || triangles[i].vertex2 == super.vertex1 || triangles[i].vertex2 == super.vertex2)
            {
                continue;
            }

            //if (triangles[i].vertex0.i == -1 || triangles[i].vertex1.i == -1 || triangles[i].vertex2.i == -1) continue;

            validTriangles.Add(triangles[i]);
        }

        triangles = validTriangles.ToArray();

        return triangles;
    }

    private Triangle CalculateSuperTriangle(Vertex[] vertices)
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float maxY = Mathf.NegativeInfinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            minX = Mathf.Min(minX, vertices[i].x);
            minY = Mathf.Min(minY, vertices[i].y);
            maxX = Mathf.Max(maxX, vertices[i].x);
            maxY = Mathf.Max(maxY, vertices[i].y);
        }

        float dx = (maxX - minX) * 10f;
        float dy = (maxY - minY) * 10f;

        Vertex v0 = new Vertex(minX - dx, minY - dy * 3f);
        Vertex v1 = new Vertex(minX - dx, maxY + dy);
        Vertex v2 = new Vertex(maxX + dx * 3f, maxY + dy);

        return new Triangle(v0, v1, v2);
    }

    private Triangle[] AddVertex(Vertex vertex, Triangle[] triangles)
    {
        List<Edge> newEdges = new List<Edge>();
        List<Triangle> validTriangles = new List<Triangle>();

        // Remove triangles where the vertex is inside the circumcircle
        for (int i = 0; i < triangles.Length; i++)
        {
            if (triangles[i].InCircumcircle(vertex))
            {
                newEdges.Add(new Edge(triangles[i].vertex0, triangles[i].vertex1));
                newEdges.Add(new Edge(triangles[i].vertex1, triangles[i].vertex2));
                newEdges.Add(new Edge(triangles[i].vertex2, triangles[i].vertex0));
            }
            else
            {
                validTriangles.Add(triangles[i]);
            }
        }

        // Update edges with only unique edges related to the initial vertex
        newEdges = UniqueEdges(newEdges);

        for (int i = 0; i < newEdges.Count; i++)
        {
            validTriangles.Add(new Triangle(newEdges[i].vertex0, newEdges[i].vertex1, vertex));
        }

        triangles = validTriangles.ToArray();

        return triangles;
    }

    private List<Edge> UniqueEdges(List<Edge> edges)
    {
        List<Edge> uniqueEdges = new List<Edge>();

        // Iterate through every edge
        for (int i = 0; i < edges.Count; i++)
        {
            bool isUnique = true;

            // Compare current edge against all edges
            for (int j = 0; j < edges.Count; j++)
            {
                if (i != j && edges[i].Compare(edges[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                uniqueEdges.Add(edges[i]);
            }
        }

        return uniqueEdges;
    }
}
