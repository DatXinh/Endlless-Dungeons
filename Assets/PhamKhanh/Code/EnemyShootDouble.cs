using UnityEngine;

public class EnemyShootDouble : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float patrolRange = 3f; // bán kính vùng tuần tra (hình vuông)
    public float attackRange = 5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float bulletSpread = 0.2f; // khoảng cách ngang giữa 2 viên

    private Rigidbody2D rb;
    private GameObject player;
    private Vector2 patrolCenter;
    private Vector2 targetPoint;
    private float shootTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // không rơi
        rb.freezeRotation = true;

        player = GameObject.FindGameObjectWithTag("Player");
        patrolCenter = transform.position;
        ChooseNewPatrolPoint();

        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            // Dừng di chuyển khi tấn công
            rb.linearVelocity = Vector2.zero;

            // Xoay mặt về phía player
            Vector2 direction = player.transform.position - transform.position;
            if (direction.x > 0.01f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);

            // Bắn
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                ShootForward();
                shootTimer = shootInterval;
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Di chuyển đến điểm tuần tra
        Vector2 dir = (targetPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;

        if (Vector2.Distance(transform.position, targetPoint) < 0.2f)
        {
            ChooseNewPatrolPoint();
        }
    }

    void ChooseNewPatrolPoint()
    {
        float offsetX = Random.Range(-patrolRange, patrolRange);
        float offsetY = Random.Range(-patrolRange, patrolRange);
        targetPoint = patrolCenter + new Vector2(offsetX, offsetY);
    }

    void ShootForward()
    {
        // Hướng từ firePoint đến player
        Vector2 shootDir = (player.transform.position - firePoint.position).normalized;

        // Tính vector vuông góc để tách các viên sang hai bên
        Vector2 perpendicular = new Vector2(-shootDir.y, shootDir.x) * bulletSpread;

        // Viên giữa
        FireBullet(firePoint.position, shootDir);

        // Viên bên trái
        FireBullet(firePoint.position + (Vector3)perpendicular, shootDir);

        // Viên bên phải
        FireBullet(firePoint.position - (Vector3)perpendicular, shootDir);
    }

    void FireBullet(Vector3 position, Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
            rbBullet.linearVelocity = direction * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(patrolCenter == Vector2.zero ? transform.position : (Vector3)patrolCenter,
            new Vector3(patrolRange * 2, patrolRange * 2, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

