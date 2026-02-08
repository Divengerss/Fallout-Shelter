using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Isometric Angles")]
    [Range(10f, 80f)] public float pitch = 30f; // Up / Down
    [Range(0f, 360f)] public float yaw = 45f;   // Left / Right

    [Header("Distance")]
    public float distance = 15f;

    [Header("Follow")]
    public float followSpeed = 8f;

    void LateUpdate()
    {
        if (!target) return;

        // Build rotation from angles
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Direction * distance
        Vector3 direction = rotation * Vector3.back;
        Vector3 desiredPosition = target.position + direction * distance;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        transform.LookAt(target);
    }
}
