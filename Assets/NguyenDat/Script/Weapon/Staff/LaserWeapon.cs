using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform startPoint;

    [Header("Laser Settings")]
    public float maxLaserLength = 25f;
    public float damage = 10f;
    public LayerMask hitMask;

    private bool isFiring = false;

    void Update()
    {
        if (isFiring)
        {
            UpdateLaser();
        }
    }
    public void StartFiring()
    {
        if (isFiring) return;
        isFiring = true;
        lineRenderer.enabled = true;
        UpdateLaser();
    }
    public void StopFiring()
    {
        if (!isFiring) return;

        isFiring = false;
        lineRenderer.enabled = false;
    }

    private void UpdateLaser()
    {
        if (startPoint == null) return;

        Vector3 origin = startPoint.position;
        Vector3 direction = startPoint.right;
        Vector3 endPos = origin + direction * maxLaserLength;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, maxLaserLength, hitMask);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    endPos = hit.point;
                    break; // dừng tia nếu trúng tường
                }
                //else if (hit.collider.CompareTag("Enemy"))
                //{
                //    hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(damage);
                //}
            }
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }
}
