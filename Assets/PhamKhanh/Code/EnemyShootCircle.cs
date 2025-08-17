using UnityEngine;

public class EnemyShootCircle : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public int bulletCount = 12; // số viên đạn trong hình tròn

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

        // Xoay mặt về phía player
        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootCircular(); // bắn đạn vòng tròn
            shootTimer = shootInterval;
        }
    }

    void ShootCircular()
    {
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Tính hướng theo góc hiện tại
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 bulletDirection = new Vector2(bulletDirX, bulletDirY).normalized;

            // Tạo viên đạn
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = bulletDirection * bulletSpeed;

            angle += angleStep;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f); // Vòng tròn hiển thị quanh quái
    }
}

