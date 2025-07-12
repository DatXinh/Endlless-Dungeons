using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [Header("Weapon data")]
    public WeaponData weaponData;
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform startPoint;

    [Header("Laser Settings")]
    public float maxLaserLength = 25f;
    public float damage;
    public LayerMask hitMask;

    private bool isFiring = false;

    private void Awake()
    {
        weaponData = GetComponent<WeaponData>();
        damage = weaponData.weaponDamage;
    }

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
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    DamageInfo damageInfo = new DamageInfo(damage, 0, false);
                    EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damageInfo);
                    }

                }
            }
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }
}
