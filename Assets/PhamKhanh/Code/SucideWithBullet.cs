using UnityEngine;

public class SuicideWithBullet : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";
    public float detectionRange = 5f;
    public float explosionRange = 1.2f;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolRange = 4f; // khoảng cách tuần tra từ vị trí ban đầu

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float damage = 50f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;  // prefab viên đạn
    public int bulletCount = 8;      // số viên đạn bắn ra
    public float bulletSpeed = 5f;   // tốc độ viên đạn

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 initialPosition;
    private bool chasingPlayer = false;
    private int patrolDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                chasingPlayer = true;
            }
            else if (distanceToPlayer > detectionRange * 1.2f) // ra xa quá thì quay lại tuần tra
            {
                chasingPlayer = false;
            }

            if (distanceToPlayer <= explosionRange)
            {
                Explode();
            }
        }
    }

    void FixedUpdate()
    {
        if (chasingPlayer && player != null)
        {
            // Xoay quái hướng về Player
            Vector2 direction = (player.position - transform.position).normalized;
            if (direction.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            rb.linearVelocity = direction * chaseSpeed;
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Đi tới giới hạn bên trái hoặc phải
        if (transform.position.x > initialPosition.x + patrolRange)
        {
            patrolDirection = -1;
            Flip();
        }
        else if (transform.position.x < initialPosition.x - patrolRange)
        {
            patrolDirection = 1;
            Flip();
        }

        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Sinh viên đạn tỏa tròn
        SpawnBullets();

        // Gây sát thương cho Player nếu có script nhận damage (có thể thêm sau)
        Destroy(gameObject);
    }

    void SpawnBullets()
    {
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Tính hướng cho viên đạn
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 bulletMoveVector = new Vector3(bulletDirX, bulletDirY, 0);

            // Tạo viên đạn tại vị trí quái
            GameObject tmpObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Gán vận tốc cho viên đạn (nếu bullet có Rigidbody2D)
            Rigidbody2D bulletRb = tmpObj.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bulletMoveVector.normalized * bulletSpeed;
            }

            angle += angleStep;
        }
    }
}
