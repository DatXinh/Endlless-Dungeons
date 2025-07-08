using UnityEngine;

public class SummonPortal : MonoBehaviour
{
    public Transform target;
    public GameObject portalPrefab;

    // Khoảng cách phía trên target (tuỳ chỉnh được trong Inspector)
    public float offsetY = 1f;

    public void SpawnPortalAtTarget()
    {
        if (portalPrefab != null && target != null)
        {
            Vector3 spawnPosition = target.position + new Vector3(0f, offsetY, 0f);
            Instantiate(portalPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
