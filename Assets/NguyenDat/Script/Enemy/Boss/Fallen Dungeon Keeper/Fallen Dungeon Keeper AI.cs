using System.Collections;
using UnityEngine;

public class FallenDungeonKeeperAI : MonoBehaviour
{
    [Header("General")]
    public Transform player;
    public EnemyHP enemyHealth;
    public GameObject TinyMana;

    [Header("Bullet Prefabs")]
    public GameObject cursedSkullPrefab;
    public GameObject brimstonePrefab;
    public GameObject miniBrimstonePrefab;
    public GameObject brimstoneArrowPrefab;

    [Header("Attack Settings")]
    public int brimstoneRainDame = 40;
    public int miniBrimstoneDame = 20;
    public int brimstoneArrowDame = 35;
    public int cursedSkullDame = 50;

    [Header("Prediction")]
    public float projectileSpeed = 6f;

    [Header("Sound")]
    public AudioSource laugh;
    public AudioSource Phase1;
    public AudioSource Phase2;
    public AudioSource Phase3;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private Coroutine attackRoutine;
    private int lastAttack = -1;

    // --- UNITY LIFECYCLE ---
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        StartCoroutine(IntroAttack(20f, true));
        laugh.Play();
        Phase1.Play();
    }

    void Update()
    {
        if (player != null)
            FlipTowardsPlayer();
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

    void SpawnBullet(GameObject prefab, Vector2 position, Vector2 direction, int Damage)
    {
        GameObject bullet = Instantiate(prefab, position, Quaternion.identity);
        EnemyDame enemyDame = bullet.GetComponent<EnemyDame>();
        if (enemyDame != null)
        {
            enemyDame.damage = Damage;
        }
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction.normalized * projectileSpeed;
    }

    void FireBulletAtAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        GameObject bullet = Instantiate(miniBrimstonePrefab, transform.position, Quaternion.identity);
        EnemyDame enemyDame = bullet.GetComponent<EnemyDame>();
        if (enemyDame != null)
        {
            enemyDame.damage = miniBrimstoneDame;
        }
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            StartCoroutine(AccelerateBullet(rb, dir, projectileSpeed, projectileSpeed * 2f, 1f));
        }
    }

    void FireArrowFormation()
    {
        if (player == null || brimstoneArrowPrefab == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        GameObject arrowGroup = Instantiate(brimstoneArrowPrefab, rb.position, Quaternion.identity);
        EnemyDame enemyDame = arrowGroup.GetComponent<EnemyDame>();
        if (enemyDame != null)
        {
            enemyDame.damage = brimstoneArrowDame;
        }

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
            float angle = -i * angleStep;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            SpawnBullet(miniBrimstonePrefab, origin, dir, miniBrimstoneDame);
        }
    }

    // --- NEW: SPAWN TINY MANA ---
    void SpawnTinyManaBurst()
    {
        if (TinyMana == null) return;
        int count = Random.Range(5, 11); // 5 đến 10
        float minForce = 4f;
        float maxForce = 7f;
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            GameObject mana = Instantiate(TinyMana, transform.position, Quaternion.identity);
            Rigidbody2D manaRb = mana.GetComponent<Rigidbody2D>();
            if (manaRb != null)
            {
                float force = Random.Range(minForce, maxForce);
                manaRb.linearVelocity = dir * force;
            }
        }
    }

    // --- ATTACK LOGIC & PHASES ---
    IEnumerator IntroAttack(float duration, bool startPhaseManagerAfter = false)
    {
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        float interval = 0.1f;
        int count = Mathf.FloorToInt(duration / interval);

        yield return StartCoroutine(BrimstoneRain(count, interval));

        if (capsuleCollider != null)
            capsuleCollider.enabled = true;

        if (startPhaseManagerAfter)
            StartCoroutine(PhaseManager());
    }

    IEnumerator PhaseManager()
    {
        attackRoutine = StartCoroutine(PhaseOne());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 70f);
        StopCoroutine(attackRoutine);
        SpawnTinyManaBurst();
        Phase1.Stop();
        Phase2.Play();
        yield return StartCoroutine(IntroAttack(12.5f));
        attackRoutine = StartCoroutine(PhaseTwo());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 35f);
        StopCoroutine(attackRoutine);
        SpawnTinyManaBurst();
        Phase2.Stop();
        Phase3.Play();
        yield return StartCoroutine(IntroAttack(15f));
        attackRoutine = StartCoroutine(PhaseThree());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 0);
        StopCoroutine(attackRoutine);
        SpawnTinyManaBurst();
    }

    IEnumerator PhaseOne()
    {
        while (true)
        {
            int random;
            do
            {
                random = Random.Range(0, 2);
            } while (random == lastAttack);
            lastAttack = random;

            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else
                yield return StartCoroutine(BulletSpreadAttack());

            yield return new WaitForSeconds(1.75f);
        }
    }

    IEnumerator PhaseTwo()
    {
        while (true)
        {
            int random;
            do
            {
                random = Random.Range(0, 3);
            } while (random == lastAttack);
            lastAttack = random;

            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else if (random == 1)
                yield return StartCoroutine(BulletSpreadAttack());
            else
                yield return StartCoroutine(DashAndFireAttack());

            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator PhaseThree()
    {
        while (true)
        {
            int random;
            do
            {
                random = Random.Range(0, 4);
            } while (random == lastAttack);
            lastAttack = random;

            if (random == 0)
                yield return StartCoroutine(ArrowAndClockAttack());
            else if (random == 1)
                yield return StartCoroutine(BulletSpreadAttack());
            else if (random == 2)
                yield return StartCoroutine(DashAndFireAttack());
            else
                yield return StartCoroutine(SpiralShotAttack());

            yield return new WaitForSeconds(1f);
        }
    }

    // --- ATTACK COROUTINES ---
    IEnumerator BrimstoneRain(int count, float interval)
    {
        if (player == null) yield break;

        float baseSpeed = 10f;
        float maxSpeed = 20f;

        for (int i = 0; i < count; i++)
        {
            float range = 25f;
            float x = Random.Range(player.position.x - range, player.position.x + range);
            Vector2 spawnPos = new Vector2(x, player.position.y + 13f);

            GameObject b = Instantiate(brimstonePrefab, spawnPos, Quaternion.identity);
            EnemyDame enemyDame = b.GetComponent<EnemyDame>();
            if (enemyDame != null)
            {
                enemyDame.damage = brimstoneRainDame;
            }
            Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float t = (float)i / (count - 1);
                float speed = Mathf.Lerp(baseSpeed, maxSpeed, t);

                Vector2 fallDir = Vector2.down;
                if (Random.value < 0.15f)
                {
                    float angleOffset = Random.Range(-15f, 15f);
                    float rad = angleOffset * Mathf.Deg2Rad;
                    fallDir = new Vector2(Mathf.Sin(rad), -Mathf.Cos(rad)).normalized;
                }

                rb.linearVelocity = fallDir * speed;
            }

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator BulletSpreadAttack()
    {
        if (player == null) yield break;
        yield return StartCoroutine(DashToRandomNearPlayer());

        int waves = 4;
        float angleStep = 10f;
        float totalAngle = 90f;
        float sweepDelay = 0.04f;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

        float startAngle = baseAngle - totalAngle / 2f;
        float endAngle = baseAngle + totalAngle / 2f;

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

            leftToRight = !leftToRight;
        }
    }

    IEnumerator ArrowAndClockAttack()
    {
        if (player == null) yield break;

        float duration = 5f;
        float interval = 1f;
        int repeat = Mathf.FloorToInt(duration / interval);

        for (int i = 0; i < repeat; i++)
        {
            FireArrowFormation();
            FireClockwiseBullets(Random.Range(12, 18));
            yield return new WaitForSeconds(interval);
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

                Vector2 newPos = Vector2.Lerp(start, target, elapsed / dashDuration);
                rb.MovePosition(newPos);

                if (fireTimer >= fireInterval)
                {
                    fireTimer = 0f;
                    Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                    SpawnBullet(miniBrimstonePrefab, rb.position, dir, miniBrimstoneDame);

                    Vector2 perp = Vector2.Perpendicular(dir).normalized;
                    SpawnBullet(miniBrimstonePrefab, rb.position, perp, miniBrimstoneDame);
                    SpawnBullet(miniBrimstonePrefab, rb.position, -perp, miniBrimstoneDame);
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator SpiralShotAttack()
    {
        laugh.Play();
        float duration = 5f;
        float fireInterval = 0.1f;
        int bulletsPerShot = 6;
        float currentRotation = 0f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += fireInterval;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float angle = currentRotation + (360f / bulletsPerShot) * i;
                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
                SpawnBullet(cursedSkullPrefab, rb.position, dir, cursedSkullDame);
            }

            currentRotation += 5f;
            yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(0.5f);
    }

    // --- SUPPORT COROUTINES ---
    IEnumerator DashToRandomNearPlayer()
    {
        if (player == null || rb == null) yield break;

        Vector2 targetPos = (Vector2)player.position + Random.insideUnitCircle.normalized * Random.Range(6f, 10f);
        float dashSpeed = 60f;
        float arriveDistance = 0.5f;
        float timeout = 0.5f;

        Vector2 direction = (targetPos - rb.position).normalized;
        rb.linearVelocity = direction * dashSpeed;

        float timer = 0f;
        while (Vector2.Distance(rb.position, targetPos) > arriveDistance && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);
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

        if (rb != null)
            rb.linearVelocity = direction * finalSpeed;
    }
}
