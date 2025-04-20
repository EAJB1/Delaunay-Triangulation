using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public Vertex vertex0;
    public Vertex vertex1;

    public Edge(Vertex _vertex0, Vertex _vertex1)
    {
        vertex0 = _vertex0;
        vertex1 = _vertex1;
    }

    public bool Compare(Edge edge)
    {
        return vertex0.Equals(edge.vertex0) && vertex1.Equals(edge.vertex1) || vertex0.Equals(edge.vertex1) && vertex1.Equals(edge.vertex0);
    }
}
