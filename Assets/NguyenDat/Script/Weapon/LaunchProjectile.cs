using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [HideInInspector]
    public GameObject projectilePrefab;

    private WeaponData weaponData;

    public Transform firePoint;
    [HideInInspector]
    public Transform nearestEnemy;

    public float launchForce = 10f;

    private int projectileDame;

    private void Awake()
    {
        weaponData = GetComponent<WeaponData>();
        projectilePrefab = weaponData.weaponProjectile;
        projectileDame = weaponData.weaponDamage;
    }
    public void Launch()
    {
        // Kiểm tra đủ điều kiện trước khi bắn
        if (projectilePrefab == null || firePoint == null || nearestEnemy == null)
            return;

        // Tính hướng từ điểm bắn đến kẻ địch
        Vector2 direction = (nearestEnemy.position - firePoint.position).normalized;

        // Tạo projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Xoay đầu đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Thêm lực cho projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchForce; // Updated to use linearVelocity
        }

        //// Gửi sát thương nếu projectile có script tương ứng
        //Projectile projectileScript = projectile.GetComponent<Projectile>();
        //if (projectileScript != null)
        //{
        //    projectileScript.SetDamage(projectileDame);
        //}
    }
}
