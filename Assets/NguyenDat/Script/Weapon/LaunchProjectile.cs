using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [HideInInspector] public GameObject projectilePrefab;

    public Transform firePoint;           // Nơi bắn đạn
    public Transform weaponTransform;     // Dùng để lấy hướng hiện tại của vũ khí
    public float launchForce = 10f;

    private WeaponData weaponData;
    private int projectileDame;

    private void Awake()
    {
        weaponData = GetComponentInParent<WeaponData>();
        projectilePrefab = weaponData.weaponProjectile;
        projectileDame = weaponData.weaponDamage;
    }

    // 🟢 Hướng đạn lấy từ hướng xoay hiện tại của vũ khí
    public void LaunchSingle()
    {
        if (projectilePrefab == null || firePoint == null || weaponTransform == null)
            return;

        Vector2 direction = weaponTransform.right.normalized;
        SpawnProjectile(firePoint.position, direction);
    }

    public void LaunchDoubleSpread(float spreadAngle = 15f)
    {
        if (projectilePrefab == null || firePoint == null || weaponTransform == null)
            return;

        Vector2 baseDir = weaponTransform.right.normalized;

        Vector2 dirLeft = Quaternion.Euler(0, 0, -spreadAngle) * baseDir;
        Vector2 dirRight = Quaternion.Euler(0, 0, spreadAngle) * baseDir;

        SpawnProjectile(firePoint.position, dirLeft);
        SpawnProjectile(firePoint.position, dirRight);
    }

    public void LaunchTripleCone(float spreadAngle = 15f)
    {
        if (projectilePrefab == null || firePoint == null || weaponTransform == null)
            return;

        Vector2 baseDir = weaponTransform.right.normalized;

        SpawnProjectile(firePoint.position, baseDir);
        SpawnProjectile(firePoint.position, Quaternion.Euler(0, 0, -spreadAngle) * baseDir);
        SpawnProjectile(firePoint.position, Quaternion.Euler(0, 0, spreadAngle) * baseDir);
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
