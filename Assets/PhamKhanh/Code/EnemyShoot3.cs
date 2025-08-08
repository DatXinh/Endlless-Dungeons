using UnityEngine;

public class EnemyShoot3 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float minShootRange = 2f;
    public float maxShootRange = 6f;

    public int bulletCount = 5;         // Số viên đạn muốn bắn
    public float spreadAngle = 60f;     // Tổng góc tỏa (độ)

    private GameObject player;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        // Xoay mặt về phía player
        if (direction.x > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(1, 1, 1);
         
        if (distance >= minShootRange && distance <= maxShootRange)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot(direction.normalized);
                shootTimer = shootInterval;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (bulletCount == 1)
        {
            FireBullet(direction);
            return;
        }

        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = baseAngle - (spreadAngle / 2f);
        
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            FireBullet(dir.normalized);
        }
    }

    void FireBullet(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxShootRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minShootRange);
    }
}

