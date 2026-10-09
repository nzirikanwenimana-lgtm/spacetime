using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KeepInsideGrid : MonoBehaviour
{
    [Header("Grid Boundary")]
    public float gridSize = 20f;
    public float boundaryMargin = 0.5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float halfGrid =
            gridSize / 2f - boundaryMargin;

        Vector3 position = rb.position;
        Vector3 velocity = rb.linearVelocity;

        bool hitBoundary = false;

        if (position.x > halfGrid)
        {
            position.x = halfGrid;
            velocity.x = 0f;
            hitBoundary = true;
        }

        if (position.x < -halfGrid)
        {
            position.x = -halfGrid;
            velocity.x = 0f;
            hitBoundary = true;
        }

        if (position.z > halfGrid)
        {
            position.z = halfGrid;
            velocity.z = 0f;
            hitBoundary = true;
        }

        if (position.z < -halfGrid)
        {
            position.z = -halfGrid;
            velocity.z = 0f;
            hitBoundary = true;
        }

        if (hitBoundary)
        {
            rb.position = position;
            rb.linearVelocity = velocity;
        }
    }
}