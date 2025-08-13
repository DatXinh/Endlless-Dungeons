using UnityEngine;

public class FirewormShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;
    public int spreadCount = 5;
    public int circleCount = 12;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Kiểu 1: Bắn thẳng
    public void ShootStraight()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (!IsValid()) return;

        Vector2 direction = (player.position - firePoint.position).normalized;
        Shoot(direction);
    }

    // Kiểu 2: Bắn loe
    public void ShootSpread()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (!IsValid()) return;

        Vector2 baseDir = (player.position - firePoint.position).normalized;
        float angleStep = 15f;
        float startAngle = -angleStep * (spreadCount - 1) / 2;

        for (int i = 0; i < spreadCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 rotatedDir = Quaternion.Euler(0, 0, angle) * baseDir;
            Shoot(rotatedDir);
        }
    }

    // Kiểu 3: Bắn vòng tròn
    public void ShootCircle()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (!IsValid()) return;

        for (int i = 0; i < circleCount; i++)
        {
            float angle = 360f / circleCount * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Shoot(direction);
        }
    }

    // Kiểu 4: Bắn lệch ngẫu nhiên
    public void ShootRandom()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (!IsValid()) return;

        Vector2 baseDir = (player.position - firePoint.position).normalized;
        float randomAngle = Random.Range(-25f, 25f);
        Vector2 dir = Quaternion.Euler(0, 0, randomAngle) * baseDir;
        Shoot(dir);
    }

    // Ngẫu nhiên chọn 1 trong các kiểu bắn
    public void ShootRandomPattern()
    {
        int rand = Random.Range(0, 4); // 0 đến 3
        switch (rand)
        {
            case 0:
                ShootStraight();
                break;
            case 1:
                ShootSpread();
                break;
            case 2:
                ShootCircle();
                break;
            case 3:
                ShootRandom();
                break;
        }
    }

    // 🧨 Tạo đạn
    private void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * bulletSpeed;
        }
    }

    // ✅ Kiểm tra hợp lệ
    private bool IsValid()
    {
        return player != null && bulletPrefab != null && firePoint != null;
    }
}
