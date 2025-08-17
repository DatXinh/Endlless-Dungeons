using UnityEngine;

public class EnemyPatrol : MonoBehaviour
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
    public int bulletCount = 12;

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
                ShootCircular();
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

    void ShootCircular()
    {
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            if (rbBullet != null)
                rbBullet.linearVelocity = bulletDirection * bulletSpeed;

            angle += angleStep;
        }
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
