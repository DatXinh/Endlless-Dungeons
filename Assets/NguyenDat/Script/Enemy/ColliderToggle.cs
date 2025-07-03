using UnityEngine;

public class ColliderToggle : MonoBehaviour
{
    [Tooltip("Collider cần bật/tắt")]
    public Collider2D targetCollider;

    // Có thể gọi từ script khác hoặc gán sự kiện trên Button/UI

    public void EnableCollider()
    {
        if (targetCollider != null)
            targetCollider.enabled = true;
    }

    public void DisableCollider()
    {
        if (targetCollider != null)
            targetCollider.enabled = false;
    }

    public void ToggleCollider()
    {
        if (targetCollider != null)
            targetCollider.enabled = !targetCollider.enabled;
    }
}
