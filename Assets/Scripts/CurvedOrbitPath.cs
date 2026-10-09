using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurvedOrbitPath : MonoBehaviour
{
    [Header("Orbiting Object")]
    public Transform orbitingMass;

    [Header("Path")]
    public int maxPoints = 120;
    public float pointSpacing = 0.08f;

    [Header("Surface")]
    public LayerMask surfaceLayer;
    public float rayHeight = 20f;
    public float surfaceOffset = 0.08f;

    private LineRenderer line;

    private List<Vector3> pathPoints =
        new List<Vector3>();

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.loop = false;
        line.useWorldSpace = true;
        line.positionCount = 0;
    }

    void LateUpdate()
    {
        if (orbitingMass == null)
            return;

        UpdatePath();
    }

    void UpdatePath()
    {
        Vector3 currentPosition =
            GetSurfacePosition(
                orbitingMass.position
            );

        if (pathPoints.Count == 0)
        {
            pathPoints.Add(currentPosition);
        }
        else
        {
            float distance =
                Vector3.Distance(
                    pathPoints[pathPoints.Count - 1],
                    currentPosition
                );

            if (distance >= pointSpacing)
            {
                pathPoints.Add(currentPosition);
            }
        }

        while (pathPoints.Count > maxPoints)
        {
            pathPoints.RemoveAt(0);
        }

        line.positionCount =
            pathPoints.Count;

        for (int i = 0; i < pathPoints.Count; i++)
        {
            line.SetPosition(
                i,
                pathPoints[i]
            );
        }
    }

    Vector3 GetSurfacePosition(
        Vector3 worldPosition)
    {
        Vector3 rayOrigin =
            worldPosition +
            Vector3.up * rayHeight;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            rayHeight * 2f,
            surfaceLayer,
            QueryTriggerInteraction.Ignore))
        {
            return hit.point +
                   hit.normal * surfaceOffset;
        }

        return worldPosition;
    }
}