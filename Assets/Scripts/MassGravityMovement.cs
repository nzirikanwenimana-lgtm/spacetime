using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MassGravityMovement : MonoBehaviour
{
    [Header("Gravity Source")]
    public Transform gravitySource;

    [Header("Gravity Settings")]
    [Min(0f)]
    public float force = 20f;

    [Min(0.1f)]
    public float influenceRadius = 10f;

    [Header("Grid Boundary")]
    public float gridSize = 20f;

    public float boundaryMargin = 0.5f;

    [Header("Movement")]
    public bool moveHorizontally = true;

    [Min(0f)]
    public float initialOrbitSpeed = 1.5f;

    [Header("Orbit Stabilization")]
    public bool stabilizeOrbit = true;

    [Min(0f)]
    public float orbitStabilization = 2f;

    [Tooltip("Maintains tangential speed when gravity force changes.")]
    public bool maintainOrbitSpeed = true;

    private Rigidbody rb;

    private float lastForce;
    private Vector3 lastRadialDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        lastForce = force;

        if (gravitySource == null)
            return;

        Vector3 offset =
            transform.position - gravitySource.position;

        offset.y = 0f;

        if (offset.sqrMagnitude < 0.0001f)
            return;

        Vector3 radialDirection = offset.normalized;

        // Start moving perpendicular to the direction of gravity.
        Vector3 tangent =
            Vector3.Cross(Vector3.up, radialDirection).normalized;

        rb.linearVelocity =
            tangent * initialOrbitSpeed;

        lastRadialDirection = radialDirection;
    }

    void FixedUpdate()
    {
        if (gravitySource == null)
            return;

        if (maintainOrbitSpeed && !Mathf.Approximately(force, lastForce))
        {
            AdjustOrbitSpeed();
            lastForce = force;
        }

        ApplyGravity();

        if (stabilizeOrbit)
            StabilizeOrbit();

        KeepInsideGrid();
    }

    void ApplyGravity()
    {
        Vector3 direction =
            gravitySource.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance < 0.01f || distance > influenceRadius)
            return;

        direction.Normalize();

        // Limit the minimum distance to avoid extreme acceleration.
        float safeDistance = Mathf.Max(distance, 0.5f);

        float strength =
            force / (safeDistance * safeDistance);

        if (moveHorizontally)
        {
            rb.AddForce(
                direction * strength,
                ForceMode.Acceleration
            );
        }
    }

    void AdjustOrbitSpeed()
    {
        if (gravitySource == null)
            return;

        Vector3 offset =
            transform.position - gravitySource.position;

        offset.y = 0f;

        float distance = offset.magnitude;

        if (distance < 0.01f)
            return;

        Vector3 radialDirection = offset.normalized;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        // Preserve the existing tangential direction.
        Vector3 tangentVelocity =
            velocity -
            Vector3.Project(velocity, radialDirection);

        float currentSpeed = tangentVelocity.magnitude;

        if (currentSpeed < 0.01f)
        {
            tangentVelocity =
                Vector3.Cross(Vector3.up, radialDirection).normalized
                * initialOrbitSpeed;
        }
        else
        {
            // Scale speed with the square root of the force ratio.
            float ratio = Mathf.Max(force, 0.01f) /
                          Mathf.Max(lastForce, 0.01f);

            tangentVelocity *= Mathf.Sqrt(ratio);
        }

        velocity.x = tangentVelocity.x;
        velocity.z = tangentVelocity.z;

        rb.linearVelocity = velocity;
    }

    void StabilizeOrbit()
    {
        Vector3 offset =
            transform.position - gravitySource.position;

        offset.y = 0f;

        float distance = offset.magnitude;

        if (distance < 0.01f)
            return;

        Vector3 radialDirection = offset.normalized;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        Vector3 radialVelocity =
            Vector3.Project(velocity, radialDirection);

        rb.AddForce(
            -radialVelocity * orbitStabilization,
            ForceMode.Acceleration
        );
    }

    void KeepInsideGrid()
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
        else if (position.x < -halfGrid)
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
        else if (position.z < -halfGrid)
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