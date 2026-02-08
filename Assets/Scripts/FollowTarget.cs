using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    [Header("Offsets")]
    public float distanceY = 10f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position;
        desiredPosition.y += distanceY;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime
        );
    }
}
