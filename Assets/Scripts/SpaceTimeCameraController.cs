using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceTimeCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance")]
    public float distance = 14f;
    public float minimumDistance = 5f;
    public float maximumDistance = 30f;

    [Header("Rotation")]
    public float rotationSpeed = 80f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        // Right mouse button = orbit camera
        if (mouse.rightButton.isPressed)
        {
            Vector2 mouseDelta =
                mouse.delta.ReadValue();

            transform.RotateAround(
                target.position,
                Vector3.up,
                mouseDelta.x *
                rotationSpeed *
                Time.deltaTime *
                0.01f
            );

            Vector3 right =
                transform.right;

            transform.RotateAround(
                target.position,
                right,
                -mouseDelta.y *
                rotationSpeed *
                Time.deltaTime *
                0.01f
            );
        }

        // Mouse wheel = zoom
        float scroll =
            mouse.scroll.ReadValue().y;

        distance -=
            scroll * 0.01f;

        distance =
            Mathf.Clamp(
                distance,
                minimumDistance,
                maximumDistance
            );

        Vector3 direction =
            (transform.position -
             target.position).normalized;

        transform.position =
            target.position +
            direction * distance;

        transform.LookAt(target);
    }
}