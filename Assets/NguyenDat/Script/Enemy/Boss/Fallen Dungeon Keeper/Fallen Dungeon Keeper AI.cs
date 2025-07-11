using System.Collections;
using UnityEngine;

public class FallenDungeonKeeperAI : MonoBehaviour
{
    [Header("General")]
    public Transform player;
    public EnemyHealth enemyHealth;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float moveRadius = 10f;
    private Vector2 moveTarget;

    [Header("Bullet Prefabs")]
    public GameObject cursedSkullPrefab;
    public GameObject brimstonePrefab;
    public GameObject miniBrimstonePrefab;

    [Header("Prediction")]
    public float projectileSpeed = 6f; // Tốc độ đạn để tính dự đoán

    [Header("Orbiting")]
    public float orbitRadius;
    public float orbitSpeed;
    private float orbitAngle = 0f;

    private bool isApproachingPlayer = true;
    public float approachSpeed;
    public float approachStopDistance = 10f;
    private SpriteRenderer spriteRenderer;

    private Coroutine attackRoutine;
    private Rigidbody2D playerRb;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerRb = playerObj.GetComponent<Rigidbody2D>();
            }
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(PhaseManager());
    }

    void Update()
    {
        if (player != null)
        {
            if (isApproachingPlayer)
            {
                ApproachPlayer();
            }
            else
            {
                OrbitAroundPlayer();
            }

            FlipTowardsPlayer();
        }
    }

    void ApproachPlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > approachStopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * approachSpeed * Time.deltaTime);
        }
        else
        {
            isApproachingPlayer = false;
            orbitAngle = Random.Range(0f, 360f); // bắt đầu bay vòng
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

    void OrbitAroundPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        Vector2 center = player.position;
        float x = center.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = center.y + Mathf.Sin(orbitAngle) * orbitRadius;
        transform.position = Vector2.Lerp(transform.position, new Vector2(x, y), Time.deltaTime * moveSpeed);
    }

    IEnumerator PhaseManager()
    {
        attackRoutine = StartCoroutine(PhaseOne());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 70f);

        StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(PhaseTwo());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 35f);

        StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(PhaseThree());
        yield return new WaitUntil(() => enemyHealth.GetHealthPercent() <= 0);

        StopCoroutine(attackRoutine);
    }

    //Chia phase

    IEnumerator PhaseOne()
    {
        while (true)
        {
            ShootSpread(miniBrimstonePrefab, 5, 20f, 6f);
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(BrimstoneRain(10, 0.1f));
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator PhaseTwo()
    {
        while (true)
        {
            ShootStraight(brimstonePrefab, 3, 0.3f);
            yield return new WaitForSeconds(1.2f);

            ShootSpiral(cursedSkullPrefab, 12, 120f);
            yield return new WaitForSeconds(1.5f);

            ShootRing(miniBrimstonePrefab, 8);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator PhaseThree()
    {
        while (true)
        {
            ShootSpiral(cursedSkullPrefab, 18, 100f);
            yield return new WaitForSeconds(1f);

            StartCoroutine(BrimstoneRain(20, 0.08f));

            ShootRing(miniBrimstonePrefab, 10);
            yield return new WaitForSeconds(0.8f);

            ShootSpread(brimstonePrefab, 5, 25f, 7f);
            yield return new WaitForSeconds(1f);
        }
    }

    // Cách bắn đạn

    void Shoot(GameObject prefab, Vector2 dir, float speed = 6f)
    {
        if (prefab == null) return;
        GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir.normalized * speed;
    }

    void ShootStraight(GameObject prefab, int count, float delay)
    {
        StartCoroutine(StraightShot(prefab, count, delay));
    }

    IEnumerator StraightShot(GameObject prefab, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = PredictPlayerDirection();
            Shoot(prefab, dir, projectileSpeed);
            yield return new WaitForSeconds(delay);
        }
    }

    void ShootRing(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * 360f / count;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Shoot(prefab, dir, projectileSpeed);
        }
    }

    void ShootSpread(GameObject prefab, int count, float spreadAngle, float speed)
    {
        Vector2 baseDir = PredictPlayerDirection();
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + ((i - count / 2f) * (spreadAngle / count));
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Shoot(prefab, dir, speed);
        }
    }

    void ShootSpiral(GameObject prefab, int count, float rotationSpeed)
    {
        float offset = Time.time * rotationSpeed;
        for (int i = 0; i < count; i++)
        {
            float angle = i * 360f / count + offset;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Shoot(prefab, dir, projectileSpeed);
        }
    }

    IEnumerator BrimstoneRain(int count, float interval)
    {
        if (player == null) yield break;

        for (int i = 0; i < count; i++)
        {
            // Random trong vùng xung quanh người chơi
            float range = 25f;
            float x = Random.Range(player.position.x - range, player.position.x + range);
            Vector2 spawnPos = new Vector2(x, player.position.y + 20f); // Rơi từ trên đầu player

            GameObject b = Instantiate(brimstonePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.down * 5f;

            yield return new WaitForSeconds(interval);
        }
    }

    //Dự đoán hướng di chuyển của người chơi

    Vector2 PredictPlayerDirection()
    {
        if (player == null) return Vector2.right;

        Vector2 toPlayer = player.position - transform.position;
        Vector2 playerVelocity = playerRb != null ? playerRb.linearVelocity : Vector2.zero;

        float timeToHit = toPlayer.magnitude / projectileSpeed;
        Vector2 predictedPos = (Vector2)player.position + playerVelocity * timeToHit;

        return predictedPos - (Vector2)transform.position;
    }
}
