using UnityEngine;

public class EnemyShoot2 : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float bulletSpeed = 5f;
    public float minShootRange = 2f;
    public float maxShootRange = 6f;

    private GameObject player;
    private float shootTimer;

    void Start()
    {
        // Tìm player theo tag
        player = GameObject.FindGameObjectWithTag("Player");
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        // Xoay mặt về phía player (mặc định quay mặt sang phải)
        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);   // quay mặt phải (mặc định)
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);  // quay mặt trái

        // Bắn nếu player trong phạm vi
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxShootRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minShootRange);
    }
}

