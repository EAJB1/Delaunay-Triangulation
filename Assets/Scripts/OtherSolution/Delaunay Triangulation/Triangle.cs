using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Triangle
{
    public Vertex vertex0;
    public Vertex vertex1;
    public Vertex vertex2;

    public float circumcircle;
    public Vertex circumcenter;
    public float circumradius;

    public Triangle(Vertex _vertex0, Vertex _vertex1, Vertex _vertex2)
    {
        vertex0 = _vertex0;
        vertex1 = _vertex1;
        vertex2 = _vertex2;

        CalculateCircumcenter(vertex0, vertex1, vertex2);
    }

    public bool InCircumcircle(Vertex v)
    {
        float dx = circumcenter.x - v.x;
        float dy = circumcenter.y - v.y;

        return Mathf.Sqrt(dx * dx + dy * dy) <= circumradius;
    }

    private void CalculateCircumcenter(Vertex a, Vertex b, Vertex c)
    {
        // Line A to B
        float aDeltaX = b.x - a.x;
        float aDeltaY = b.y - a.y;
        float aC = aDeltaY * a.x + -aDeltaX * a.y;

        Vector3 lineAB = new Vector3(aDeltaY, -aDeltaX, aC);

        // Line B to C
        float bDeltaX = c.x - b.x;
        float bDeltaY = c.y - b.y;
        float bC = bDeltaY * b.x + -bDeltaX * b.y;

        Vector3 lineBC = new Vector3(bDeltaY, -bDeltaX, bC);

        // Midpoints
        Vector2 midPointAB = Vector2.Lerp(new Vector2(a.x, a.y), new Vector2(b.x, b.y), 0.5f);
        Vector2 midPointBC = Vector2.Lerp(new Vector2(b.x, b.y), new Vector2(c.x, c.y), 0.5f);

        // Perpendicular Line AB
        Vector3 newLine = new Vector3();
        newLine.x = -lineAB.y;
        newLine.y = lineAB.x;
        newLine.z = newLine.x * midPointAB.x + newLine.y * midPointAB.y;

        Vector3 perpendicularAB = newLine;

        // Perpendicular Line BC
        newLine = Vector3.zero;
        newLine.x = -lineBC.y;
        newLine.y = lineBC.x;
        newLine.z = newLine.x * midPointBC.x + newLine.y * midPointBC.y;

        Vector3 perpendicularBC = newLine;

        // Circumcircle
        float A1 = perpendicularAB.x;
        float A2 = perpendicularBC.x;
        float B1 = perpendicularAB.y;
        float B2 = perpendicularBC.y;
        float C1 = perpendicularAB.z;
        float C2 = perpendicularBC.z;

        // Cramer's rule
        float Determinant = A1 * B2 - A2 * B1;
        float DeterminantX = C1 * B2 - C2 * B1;
        float DeterminantY = A1 * C2 - A2 * C1;

        float x = DeterminantX / Determinant;
        float y = DeterminantY / Determinant;

        circumcenter = new Vertex(x, y);

        // Circumradius
        circumradius = Vector2.Distance(new Vector2(circumcenter.x, circumcenter.y), new Vector2(a.x, a.y));
    }
}
