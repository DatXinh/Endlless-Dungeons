using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float patrolRange = 5f;       // Bán kính tuần tra
    public float moveSpeed = 2f;         // Tốc độ di chuyển
    public float chaseSpeed = 3f;        // Tốc độ đuổi
    public float attackRange = 6f;       // Phạm vi tấn công
    public float stopDistance = 4f;      // Khoảng cách giữ khi tấn công

    public float shootInterval = 1.5f;   // Thời gian giữa các phát bắn
    public int bulletCount = 3;          // Số viên đạn mỗi lần bắn
    public float spreadAngle = 15f;      // Góc tỏa giữa các viên
    public GameObject bulletPrefab;      // Prefab đạn
    public float bulletSpeed = 8f;       // Tốc độ bay của đạn
    public Transform firePoint;          // Vị trí bắn đạn

    private Rigidbody2D rb;
    private Vector2 patrolTarget;
    private Transform player;
    private float shootTimer;

    private Vector2 startPos;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        SetRandomPatrolTarget();

        // Tìm Player theo Tag
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            // Nếu chưa có player thì tìm lại
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                MoveTowards(player.position, chaseSpeed);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (player.position.x > transform.position.x)
                spriteRenderer.flipX = true; // nếu sprite gốc nhìn trái, thì phải flip khi muốn nhìn phải
            else
                spriteRenderer.flipX = false;


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
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            SetRandomPatrolTarget();
        }
        MoveTowards(patrolTarget, moveSpeed);
    }

    void SetRandomPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRange;
        patrolTarget = startPos + randomOffset;
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void ShootForward()
    {
        // Tính hướng từ enemy tới player
        Vector2 forwardDir = (player.position - firePoint.position).normalized;

        // Lấy góc theo độ (rotation)
        float baseAngle = Mathf.Atan2(forwardDir.y, forwardDir.x) * Mathf.Rad2Deg;

        // Góc bắt đầu (để tỏa đều quanh hướng player)
        float startAngle = baseAngle - spreadAngle * (bulletCount - 1) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Tính góc của viên đạn hiện tại
            float currentAngle = startAngle + i * spreadAngle;

            // Chuyển thành vector hướng
            Vector2 shootDir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                                           Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            // Tạo viên đạn
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.linearVelocity = shootDir * bulletSpeed;
        }
    }


    void LateUpdate()
    {
        // Giữ nguyên rotation sprite, không bị xoay vì vật lý
        transform.rotation = Quaternion.identity;
    }
}
