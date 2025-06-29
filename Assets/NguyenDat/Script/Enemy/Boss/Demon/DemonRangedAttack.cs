using UnityEngine;

public class DemonRangedAttack : MonoBehaviour
{
    [Header("References")]
    public DemonAI demonAI;
    public Transform firePoint;

    [Header("Bullet Prefabs")]
    public GameObject bulletPrefab;
    public GameObject chargedBulletPrefab;

    [Header("Bullet Settings")]
    public float bulletSpeed = 5f;
    public float attackCooldown = 2f;
    public float maxAttackRange = 20f;

    [Header("Bullet Rain Settings")]
    public int rainBulletCount = 5;
    public float rainSpreadWidth = 4f;

    [Header("Radial Settings")]
    public int radialBulletCount = 8;

    private float cooldownTimer;

    void Update()
    {
        if (demonAI == null || demonAI.TargetPlayer == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, demonAI.TargetPlayer.transform.position);
        if (distanceToPlayer > maxAttackRange)
            return;

        if (cooldownTimer <= 0f)
        {
            PerformRangedAttack();
            cooldownTimer = attackCooldown;
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void PerformRangedAttack()
    {
        Vector2 direction = (demonAI.TargetPlayer.transform.position - firePoint.position).normalized;

        float roll = Random.value;

        if (roll < 0.30f)
        {
            FireBullet(firePoint.position, direction, bulletPrefab, bulletSpeed); // Đạn đơn
        }
        else if (roll < 0.55f)
        {
            FireSpread(direction); // Đạn 3 hướng
        }
        else if (roll < 0.75f)
        {
            FireRain(rainBulletCount, rainSpreadWidth); // Mưa đạn
        }
        else if (roll < 0.90f)
        {
            FireRadial(radialBulletCount); // Vòng tròn
        }
        else
        {
            FireChargedShot(direction); // Đạn đặc biệt mới
        }
    }

    void FireBullet(Vector2 position, Vector2 direction, GameObject prefab, float speed)
    {
        if (prefab == null) return;

        GameObject bullet = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void FireSpread(Vector2 forward)
    {
        float spreadAngle = 15f;
        for (int i = -1; i <= 1; i++)
        {
            float angleOffset = i * spreadAngle;
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * forward;
            FireBullet(firePoint.position, dir.normalized, bulletPrefab, bulletSpeed);
        }
    }

    void FireRadial(int bulletCount)
    {
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            FireBullet(firePoint.position, dir.normalized, bulletPrefab, bulletSpeed);
        }
    }

    // Cải tiến: Mỗi viên đạn mưa cách nhau 1-2 đơn vị, đều nhau
    void FireRain(int count, float spreadWidth)
    {
        if (demonAI == null || demonAI.TargetPlayer == null)
            return;

        Vector2 playerPos = demonAI.TargetPlayer.transform.position;
        float spawnY = playerPos.y + 6f;

        // Tính toán khoảng cách đều nhau trong khoảng spreadWidth
        float minSpacing = 1f;
        float maxSpacing = 2f;
        float totalWidth = Mathf.Clamp((count - 1) * minSpacing, 0, spreadWidth * 2);
        float actualSpacing = Mathf.Min(maxSpacing, Mathf.Max(minSpacing, totalWidth / (count - 1)));
        float startX = playerPos.x - (actualSpacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++)
        {
            float spawnX = startX + i * actualSpacing;
            Vector2 spawnPos = new Vector2(spawnX, spawnY);
            FireBullet(spawnPos, Vector2.down, bulletPrefab, bulletSpeed * 0.8f);
        }
    }

    // Thêm phương thức tấn công mới: Đạn đặc biệt (charged shot)
    void FireChargedShot(Vector2 direction)
    {
        if (chargedBulletPrefab == null) return;
        FireBullet(firePoint.position, direction, chargedBulletPrefab, bulletSpeed * 1.5f);
    }
}
