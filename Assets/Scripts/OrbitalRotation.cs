using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrbitalRotation : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 8f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Ignore vertical movement
        velocity.y = 0f;

        if (velocity.sqrMagnitude < 0.01f)
            return;

        // Rotate the sphere in the direction it is moving
        Quaternion targetRotation =
            Quaternion.LookRotation(velocity.normalized, Vector3.up);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }
}