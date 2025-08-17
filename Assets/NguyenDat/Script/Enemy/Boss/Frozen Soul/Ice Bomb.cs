using UnityEngine;

public class IceBomb : MonoBehaviour
{
    public float lifetime = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    private bool hasExploded = false;

    private void Start()
    {
        Invoke(nameof(Explode), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        ShootCircle(6); // Bắn 6 viên đạn theo vòng tròn
        Destroy(gameObject);
    }

    void ShootCircle(int count)
    {
        float angleStep = 360f / count;
        float angle = 0f;

        for (int i = 0; i < count; i++)
        {
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 dir = new Vector2(dirX, dirY);

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * bulletSpeed;

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            angle += angleStep;
        }
    }
}
