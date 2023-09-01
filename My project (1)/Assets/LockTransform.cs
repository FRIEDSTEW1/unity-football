using UnityEngine;

public class LockTransform : MonoBehaviour
{
    public Transform targetTransform; // The transform to lock onto

    public Vector3 positionOffset;    // Offset to apply to the position
    public Vector3 rotationOffset;    // Offset to apply to the rotation

    private void Update()
    {
        if (targetTransform != null)
        {
            // Match position with offset
            transform.position = targetTransform.position + positionOffset;

            // Match rotation with offset
            transform.rotation = targetTransform.rotation * Quaternion.Euler(rotationOffset);
        }
        else
        {
            Debug.LogWarning("Target transform is not assigned!");
        }
    }
}
