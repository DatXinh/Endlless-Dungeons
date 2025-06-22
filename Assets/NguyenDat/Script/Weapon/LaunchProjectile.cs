using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public Transform nearestEnemy;

    public Transform firePoint;
    public float launchForce = 10f;

    private WeaponData weaponData;
    private int projectileDame;

    private void Awake()
    {
        weaponData = GetComponentInParent<WeaponData>();
        projectilePrefab = weaponData.weaponProjectile;
        projectileDame = weaponData.weaponDamage;

        if (projectilePrefab == null)
            Debug.LogError("Projectile prefab is not assigned in WeaponData.");
    }
    public void LaunchSingle()
    {
        if (projectilePrefab == null || firePoint == null || nearestEnemy == null)
            return;

        Vector2 direction = (nearestEnemy.position - firePoint.position).normalized;
        SpawnProjectile(firePoint.position, direction);
    }

    public void LaunchDoubleSpread(float spreadAngle = 15f)
    {
        if (projectilePrefab == null || firePoint == null || nearestEnemy == null)
            return;

        Vector2 direction = (nearestEnemy.position - firePoint.position).normalized;

        Vector2 dirLeft = Quaternion.Euler(0, 0, -spreadAngle) * direction;
        Vector2 dirRight = Quaternion.Euler(0, 0, spreadAngle) * direction;

        SpawnProjectile(firePoint.position, dirLeft);
        SpawnProjectile(firePoint.position, dirRight);
    }

    public void LaunchTripleCone(float spreadAngle = 15f)
    {
        if (projectilePrefab == null || firePoint == null || nearestEnemy == null)
            return;

        Vector2 direction = (nearestEnemy.position - firePoint.position).normalized;

        SpawnProjectile(firePoint.position, direction);
        SpawnProjectile(firePoint.position, Quaternion.Euler(0, 0, -spreadAngle) * direction);
        SpawnProjectile(firePoint.position, Quaternion.Euler(0, 0, spreadAngle) * direction);
    }
    private GameObject SpawnProjectile(Vector2 position, Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.identity);

        // Xoay đầu đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Thêm lực
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchForce;
        }

        return projectile;
    }
}
