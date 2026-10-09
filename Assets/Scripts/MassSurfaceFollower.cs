using UnityEngine;

public class MassSurfaceFollower : MonoBehaviour
{
    [Header("Surface")]
    public LayerMask surfaceLayer;

    [Header("Settings")]
    public float rayHeight = 10f;
    public float surfaceOffset = 0.05f;
    public float maximumDrop = 0.5f;

    [Header("Sphere")]
    public float sphereRadius = 0.5f;

    private float startingY;

    void Start()
    {
        // Remember the original height
        startingY = transform.position.y;

        // Automatically calculate the real sphere radius
        SphereCollider sphere = GetComponent<SphereCollider>();

        if (sphere != null)
        {
            sphereRadius =
                sphere.radius * transform.lossyScale.y;
        }
    }

    void FixedUpdate()
    {
        FollowSurface();
    }

    void FollowSurface()
    {
        Vector3 rayOrigin =
            transform.position +
            Vector3.up * rayHeight;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            rayHeight * 2f,
            surfaceLayer))
        {
            float targetY =
                hit.point.y +
                sphereRadius +
                surfaceOffset;

            float minimumY =
                startingY - maximumDrop;

            targetY = Mathf.Max(
                targetY,
                minimumY
            );

            Vector3 newPosition =
                transform.position;

            newPosition.y = targetY;

            transform.position = newPosition;
        }
        else
        {
            // No grid underneath the sphere.
            // Stop vertical falling.

            Rigidbody rb =
                GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 velocity =
                    rb.linearVelocity;

                velocity.y = 0f;

                rb.linearVelocity = velocity;
            }

            // Return to the original safe height
            Vector3 safePosition =
                transform.position;

            safePosition.y = startingY;

            transform.position =
                safePosition;
        }
    }
}