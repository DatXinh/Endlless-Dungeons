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

        if (roll < 0.40f)
        {
            FireBullet(firePoint.position, direction, bulletPrefab, bulletSpeed); // Đạn đơn
        }
        else if (roll < 0.65f)
        {
            FireSpread(direction); // Đạn 3 hướng
        }
        else if (roll < 0.80f)
        {
            FireRain(rainBulletCount, rainSpreadWidth); // Mưa đạn
        }
        else
        {
            FireRadial(radialBulletCount); // Vòng tròn
        }
    }

    void FireBullet(Vector2 position, Vector2 direction, GameObject prefab, float speed)
    {
        if (prefab == null) return;

        GameObject bullet = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed; // Updated to use linearVelocity
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

    void FireRain(int count, float spreadWidth)
    {
        if (demonAI == null || demonAI.TargetPlayer == null)
            return;

        Vector2 playerPos = demonAI.TargetPlayer.transform.position;
        float spawnY = playerPos.y + 6f; // Chiều cao sinh đạn (có thể cho vào [Header] nếu muốn điều chỉnh ngoài Inspector)

        for (int i = 0; i < count; i++)
        {
            float offsetX = Random.Range(-spreadWidth, spreadWidth);
            Vector2 spawnPos = new Vector2(playerPos.x + offsetX, spawnY);
            FireBullet(spawnPos, Vector2.down, bulletPrefab, bulletSpeed * 0.8f);
        }
    }

}
