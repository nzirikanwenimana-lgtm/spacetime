using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpaceTimeGridLines : MonoBehaviour
{
    [Header("Gravity Source")]
    public Transform gravitySource;

    [Header("Plane")]
    public float gridSize = 20f;

    [Header("Circular Grid")]
    public int ringCount = 28;
    public int pointsPerRing = 160;
    public float firstRadius = 0.4f;
    public float ringSpacing = 0.5f;

    [Header("Radial Grid")]
    public int radialLineCount = 48;
    public int pointsPerRadialLine = 100;

    [Header("Appearance")]
    public Color lineColor = Color.cyan;
    public float lineWidth = 0.018f;
    public float surfaceOffset = 0.045f;

    [Header("Surface")]
    public LayerMask surfaceLayer;
    public float rayHeight = 20f;

    private List<LineRenderer> lines =
        new List<LineRenderer>();

    void Start()
    {
        CreateLineObjects();
    }

    void LateUpdate()
    {
        if (gravitySource == null)
            return;

        UpdateCircularLines();
        UpdateRadialLines();
    }

    // =========================================================
    // CREATE LINE OBJECTS
    // =========================================================

    void CreateLineObjects()
    {
        ClearLines();

        // Circular lines
        for (int i = 0; i < ringCount; i++)
        {
            CreateLine(
                "Circular Line " + i
            );
        }

        // Radial lines
        for (int i = 0; i < radialLineCount; i++)
        {
            CreateLine(
                "Radial Line " + i
            );
        }
    }

    LineRenderer CreateLine(string lineName)
    {
        GameObject obj =
            new GameObject(lineName);

        obj.transform.SetParent(
            transform,
            false
        );

        LineRenderer line =
            obj.AddComponent<LineRenderer>();

        line.useWorldSpace = true;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.startColor = lineColor;
        line.endColor = lineColor;

        line.numCornerVertices = 4;
        line.numCapVertices = 4;

        line.alignment =
            LineAlignment.View;

        line.shadowCastingMode =
            ShadowCastingMode.Off;

        line.receiveShadows = false;

        Material material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        material.color =
            lineColor;

        line.material = material;

        lines.Add(line);

        return line;
    }

    // =========================================================
    // CIRCULAR LINES
    // =========================================================

    void UpdateCircularLines()
    {
        for (int r = 0; r < ringCount; r++)
        {
            LineRenderer line =
                lines[r];

            float radius =
                firstRadius +
                r * ringSpacing;

            List<Vector3> points =
                new List<Vector3>();

            for (int i = 0;
                 i < pointsPerRing;
                 i++)
            {
                float angle =
                    (float)i /
                    (pointsPerRing - 1) *
                    Mathf.PI * 2f;

                float x =
                    gravitySource.position.x +
                    Mathf.Cos(angle) *
                    radius;

                float z =
                    gravitySource.position.z +
                    Mathf.Sin(angle) *
                    radius;

                if (!IsInsidePlane(x, z))
                {
                    points.Add(Vector3.zero);
                    continue;
                }

                Vector3 surfacePoint;

                if (GetSurfacePoint(
                    x,
                    z,
                    out surfacePoint))
                {
                    points.Add(surfacePoint);
                }
                else
                {
                    points.Add(Vector3.zero);
                }
            }

            ApplyCircularPoints(
                line,
                points
            );
        }
    }

    // =========================================================
    // RADIAL LINES
    // =========================================================

    void UpdateRadialLines()
    {
        for (int r = 0;
             r < radialLineCount;
             r++)
        {
            int lineIndex =
                ringCount + r;

            LineRenderer line =
                lines[lineIndex];

            float angle =
                (float)r /
                radialLineCount *
                Mathf.PI * 2f;

            List<Vector3> points =
                new List<Vector3>();

            for (int i = 0;
                 i < pointsPerRadialLine;
                 i++)
            {
                float t =
                    (float)i /
                    (pointsPerRadialLine - 1);

                Vector3 center =
                    gravitySource.position;

                float maxRadius =
                    gridSize * 0.75f;

                float radius =
                    t * maxRadius;

                float x =
                    center.x +
                    Mathf.Cos(angle) *
                    radius;

                float z =
                    center.z +
                    Mathf.Sin(angle) *
                    radius;

                if (!IsInsidePlane(x, z))
                    break;

                Vector3 surfacePoint;

                if (GetSurfacePoint(
                    x,
                    z,
                    out surfacePoint))
                {
                    points.Add(surfacePoint);
                }
            }

            line.positionCount =
                points.Count;

            if (points.Count > 1)
            {
                line.SetPositions(
                    points.ToArray()
                );
            }
        }
    }

    // =========================================================
    // KEEP GRID INSIDE THE SQUARE PLANE
    // =========================================================

    bool IsInsidePlane(
        float x,
        float z)
    {
        Vector3 local =
            transform.InverseTransformPoint(
                new Vector3(
                    x,
                    transform.position.y,
                    z
                )
            );

        float half =
            gridSize / 2f;

        return
            local.x >= -half &&
            local.x <= half &&
            local.z >= -half &&
            local.z <= half;
    }

    // =========================================================
    // GET CURVED SURFACE POSITION
    // =========================================================

    bool GetSurfacePoint(
        float x,
        float z,
        out Vector3 point)
    {
        Vector3 rayOrigin =
            new Vector3(
                x,
                transform.position.y +
                rayHeight,
                z
            );

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            rayHeight * 2f,
            surfaceLayer))
        {
            point =
                hit.point +
                hit.normal *
                surfaceOffset;

            return true;
        }

        point =
            new Vector3(
                x,
                transform.position.y,
                z
            );

        return false;
    }

    // =========================================================
    // APPLY CIRCULAR POINTS
    // =========================================================

    void ApplyCircularPoints(
        LineRenderer line,
        List<Vector3> points)
    {
        List<Vector3> valid =
            new List<Vector3>();

        for (int i = 0;
             i < points.Count;
             i++)
        {
            if (points[i] != Vector3.zero)
            {
                valid.Add(points[i]);
            }
        }

        if (valid.Count < 2)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount =
            valid.Count;

        line.SetPositions(
            valid.ToArray()
        );
    }

    // =========================================================
    // CLEAR
    // =========================================================

    void ClearLines()
    {
        foreach (
            LineRenderer line
            in lines)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }

        lines.Clear();
    }
}