using System.Collections;
using UnityEngine;

public class FallenDungeonKeeperAI : MonoBehaviour
{
    [Header("General")]
    public Transform player;
    public EnemyHealth enemyHealth;

    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Bullet Prefabs")]
    public GameObject cursedSkullPrefab;
    public GameObject brimstonePrefab;
    public GameObject miniBrimstonePrefab;
    public GameObject brimstoneArrowPrefab;

    [Header("Prediction")]
    public float projectileSpeed = 6f; // Tốc độ đạn để tính dự đoán

    [Header("Sound")]
    public AudioSource laugh;
    public AudioSource Phase1;
    public AudioSource Phase2;
    public AudioSource Phase3;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private Coroutine attackRoutine;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        StartCoroutine(IntroAttack(20f, true)); // Đòn mở đầu khi vào trận
        laugh.Play();
        Phase1.Play();
    }

    void Update()
    {
        if (player != null)
        {
            FlipTowardsPlayer();
        }
    }
    void FlipTowardsPlayer()
    {
        if (spriteRenderer == null || player == null) return;
        Vector2 dir = player.position - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x < 0 ? -1 : 1);
            transform.localScale = scale;
        }
    }
    // Tấn công mở đầu, có thể tái sử dụng khi chuyển phase
    IEnumerator IntroAttack(float duration, bool startPhaseManagerAfter = false)
    {
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        float interval = 0.3f;
        int count = Mathf.FloorToInt(duration / interval);

        yield return StartCoroutine(BrimstoneRain(count, interval));

        if (capsuleCollider != null)
            capsuleCollider.enabled = true;

        if (startPhaseManagerAfter)
        {
            StartCoroutine(PhaseManager());
        }
    }
    //Quản lý các phase của boss
    // Quản lý phase
    IEnumerator PhaseManager()
    {
        attackRoutine = StartCoroutine(PhaseOne());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 70f);

        StopCoroutine(attackRoutine);
        yield return StartCoroutine(IntroAttack(12.5f)); // Đòn intro trước Phase 2
        attackRoutine = StartCoroutine(PhaseTwo());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 35f);

        StopCoroutine(attackRoutine);
        yield return StartCoroutine(IntroAttack(15f)); // Đòn intro trước Phase 3
        attackRoutine = StartCoroutine(PhaseThree());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 0);

        StopCoroutine(attackRoutine);
    }


    IEnumerator PhaseOne()
    {
        while (true)
        {
            int random = Random.Range(0, 2); // 0 hoặc 1

            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else
                yield return StartCoroutine(BulletSpreadAttack());

            yield return new WaitForSeconds(2.5f);
        }
    }

    IEnumerator PhaseTwo()
    {
        if (Phase2 != null)
        {
            Phase1.Stop();
            Phase2.Play();
        }
        while (true)
        {
            int random = Random.Range(0, 3);

            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else if (random == 1)
                yield return StartCoroutine(BulletSpreadAttack());
            else
                yield return StartCoroutine(DashAndFireAttack());
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator PhaseThree()
    {
        if (Phase3 != null)
        {
            Phase2.Stop();
            Phase3.Play();
        }
        while (true)
        {
            int random = Random.Range(0, 4);
            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else if (random == 1)
                yield return StartCoroutine(BulletSpreadAttack());
            else if (random == 2)
                yield return StartCoroutine(DashAndFireAttack());
            else
                yield return StartCoroutine(SpiralShotAttack());
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator BrimstoneRain(int count, float interval)
    {
        if (player == null) yield break;

        float baseSpeed = 7f;
        float maxSpeed = 17f;

        for (int i = 0; i < count; i++)
        {
            float range = 25f;
            float x = Random.Range(player.position.x - range, player.position.x + range);
            Vector2 spawnPos = new Vector2(x, player.position.y + 20f);

            GameObject b = Instantiate(brimstonePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Tính tỉ lệ tăng dần: từ 0 → 1
                float t = (float)i / (count - 1);
                float speed = Mathf.Lerp(baseSpeed, maxSpeed, t);

                rb.linearVelocity = Vector2.down * speed;
            }

            yield return new WaitForSeconds(interval);
        }
    }
    IEnumerator BulletSpreadAttack()
    {
        if (player == null) yield break;

        // Dash về phía player trước khi bắn
        yield return StartCoroutine(DashToRandomNearPlayer());

        // thông số lượng sóng, góc, thời gian delay giữa các góc
        int waves = 3;
        float angleStep = 10f;
        float totalAngle = 90f;
        float sweepDelay = 0.05f;

        // Tính góc chính giữa dựa trên hướng về phía player
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        float startAngle = baseAngle - totalAngle / 2f;
        float endAngle = baseAngle + totalAngle / 2f;

        // Random chiều bắt đầu: true = trái → phải, false = phải → trái
        bool leftToRight = Random.value > 0.5f;

        for (int wave = 0; wave < waves; wave++)
        {
            if (leftToRight)
            {
                for (float angle = startAngle; angle <= endAngle; angle += angleStep)
                {
                    FireBulletAtAngle(angle);
                    yield return new WaitForSeconds(sweepDelay);
                }
            }
            else
            {
                for (float angle = endAngle; angle >= startAngle; angle -= angleStep)
                {
                    FireBulletAtAngle(angle);
                    yield return new WaitForSeconds(sweepDelay);
                }
            }

            // Đổi chiều sau mỗi wave
            leftToRight = !leftToRight;
        }
    }

    IEnumerator AccelerateBullet(Rigidbody2D rb, Vector2 direction, float initialSpeed, float finalSpeed, float duration)
    {
        float timer = 0f;
        while (timer < duration && rb != null)
        {
            float t = timer / duration;
            float currentSpeed = Mathf.Lerp(initialSpeed, finalSpeed, t);
            rb.linearVelocity = direction * currentSpeed;

            timer += Time.deltaTime;
            yield return null;
        }

        // Giữ tốc độ cuối cùng
        if (rb != null)
            rb.linearVelocity = direction * finalSpeed;
    }
    void FireBulletAtAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        GameObject bullet = Instantiate(miniBrimstonePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            StartCoroutine(AccelerateBullet(rb, dir, projectileSpeed, projectileSpeed * 2f, 1f));
        }
    }
    IEnumerator DashToRandomNearPlayer()
    {
        if (player == null || rb == null) yield break;

        Vector2 targetPos = (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(6f, 10f);
        float dashSpeed = 60f;
        float arriveDistance = 0.5f;
        float timeout = 0.5f;

        // Tính hướng và áp dụng velocity
        Vector2 direction = (targetPos - rb.position).normalized;
        rb.linearVelocity = direction * dashSpeed;

        float timer = 0f;
        while (Vector2.Distance(rb.position, targetPos) > arriveDistance && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Ngừng lại
        rb.linearVelocity = Vector2.zero;

        // Tạm dừng trước khi tấn công
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator ArrowAndClockAttack()
    {
        if (player == null) yield break;

        float duration = 5f;
        float interval = 1f;
        int repeat = Mathf.FloorToInt(duration / interval);

        for (int i = 0; i < repeat; i++)
        {
            // Bắn mũi tên Brimstone Bullet
            FireArrowFormation();

            // Bắn đồng hồ Mini Brimstone Bullet
            FireClockwiseBullets(12);

            yield return new WaitForSeconds(interval);
        }
    }
    void FireArrowFormation()
    {
        if (player == null || brimstoneArrowPrefab == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        GameObject arrowGroup = Instantiate(brimstoneArrowPrefab, rb.position, Quaternion.identity);

        // Gán vận tốc cho từng viên đạn con trong prefab
        Rigidbody2D[] bullets = arrowGroup.GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D bulletRb in bullets)
        {
            bulletRb.linearVelocity = direction * projectileSpeed * 1.5f;
        }
    }
    void FireClockwiseBullets(int count)
    {
        float angleStep = 360f / count;
        Vector2 origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            float angle = -i * angleStep; // theo chiều kim đồng hồ
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            SpawnBullet(miniBrimstonePrefab, origin, dir);
        }
    }
    void SpawnBullet(GameObject prefab, Vector2 position, Vector2 direction)
    {
        GameObject bullet = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * projectileSpeed;
        }
    }

    IEnumerator DashAndFireAttack()
    {
        if (player == null || rb == null) yield break;

        int dashCount = 3;
        float dashDuration = 0.75f;
        float fireInterval = 0.1f;
        float dashRangeMin = 10f;
        float dashRangeMax = 15f;

        for (int d = 0; d < dashCount; d++)
        {
            Vector2 start = rb.position;
            Vector2 target = (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(dashRangeMin, dashRangeMax);

            float elapsed = 0f;
            float fireTimer = 0f;

            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;
                fireTimer += Time.deltaTime;

                // Di chuyển boss theo lerp từ start → target
                Vector2 newPos = Vector2.Lerp(start, target, elapsed / dashDuration);
                rb.MovePosition(newPos);

                // Bắn đạn mỗi 0.1 giây trong lúc đang dash
                if (fireTimer >= fireInterval)
                {
                    fireTimer = 0f;
                    Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                    SpawnBullet(miniBrimstonePrefab, rb.position, dir);
                }

                yield return null;
            }

            // Dừng ngắn giữa mỗi dash nếu muốn, hoặc bỏ qua để liền mạch
            yield return new WaitForSeconds(0.1f);
        }

        // Dừng hẳn sau 3 lần dash
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
    }
    IEnumerator SpiralShotAttack()
    {
        laugh.Play();
        float duration = 5f;
        float fireInterval = 0.1f;
        int bulletsPerShot = 3;
        float currentRotation = 0f; // xoay xoắn tăng dần

        float timer = 0f;
        while (timer < duration)
        {
            timer += fireInterval;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                // Góc bắn từng viên trong 1 lần
                float angle = currentRotation + (360f / bulletsPerShot) * i;
                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

                SpawnBullet(cursedSkullPrefab, rb.position, dir);
            }

            // Tăng góc xoay cho vòng sau → tạo xoắn ốc
            currentRotation += 10f;

            yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(0.5f);
    }


}
