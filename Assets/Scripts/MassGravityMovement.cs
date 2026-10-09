using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MassGravityMovement : MonoBehaviour
{
    [Header("Gravity Source")]
    public Transform gravitySource;

    [Header("Gravity Settings")]
    public float force = 20f;
    public float influenceRadius = 5f;

    [Header("Grid Boundary")]
    public float gridSize = 20f;
    public float boundaryMargin = 0.5f;

    [Header("Movement")]
    public bool moveHorizontally = true;

    private Rigidbody rb;

    public float initialOrbitSpeed = 1.5f;

    [Header("Orbit")]
    public bool stabilizeOrbit = true;
    public float orbitStabilization = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.linearVelocity =
            transform.forward * initialOrbitSpeed;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        if (stabilizeOrbit)
        {
            StabilizeOrbit();
        }

        KeepInsideGrid();
        KeepInsideGrid();
    }

    void ApplyGravity()
    {
        if (gravitySource == null)
            return;

        Vector3 direction =
            gravitySource.position -
            transform.position;

        // Gravity acts horizontally
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= 0.01f)
            return;

        if (distance > influenceRadius)
            return;

        float normalized =
            1f -
            (distance / influenceRadius);

        float strength =
    force / (distance * distance);

        direction.Normalize();

        if (moveHorizontally)
        {
            rb.AddForce(
                direction * strength,
                ForceMode.Force
            );
        }
    }

    void KeepInsideGrid()
    {
        float halfGrid =
            gridSize / 2f - boundaryMargin;

        Vector3 position =
            rb.position;

        Vector3 velocity =
            rb.linearVelocity;

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
    void StabilizeOrbit()
    {
        if (gravitySource == null)
            return;

        Vector3 offset =
            transform.position -
            gravitySource.position;

        offset.y = 0f;

        float distance = offset.magnitude;

        if (distance <= 0.01f)
            return;

        Vector3 radialDirection =
            offset.normalized;

        Vector3 velocity =
            rb.linearVelocity;

        Vector3 radialVelocity =
            Vector3.Project(
                velocity,
                radialDirection
            );

        rb.AddForce(
            -radialVelocity *
            orbitStabilization,
            ForceMode.Force
        );
    }
}