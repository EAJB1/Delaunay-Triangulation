using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vertex
{
    public float x;
    public float y;

    public int i;

    public Vertex(float _x = 0f, float _y = 0f, int _i = -1)
    {
        x = _x;
        y = _y;
        i = _i;
    }

    public Vertex(Vector2 _v, int _i = -1)
    {
        x = _v.x;
        y = _v.y;
        i = _i;
    }

    public bool Equals(Vertex v)
    {
        return x == v.x && y == v.y;
    }

    public int GetIndex(Vertex[] vertices)
    {
        for (int index = 0; index < vertices.Length; index++)
        {
            if (vertices[index] == this)
            {
                return index;
            }
        }

        return -1;
    }
}
