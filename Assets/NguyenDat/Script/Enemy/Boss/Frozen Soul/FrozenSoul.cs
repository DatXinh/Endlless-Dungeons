using System.Collections;
using UnityEngine;

public class FrozenSoul : MonoBehaviour
{
    public Transform player;
    public EnemyHealth health;

    [Header("Phase Thresholds")]
    public float phase2Threshold = 83f;
    public float phase3Threshold = 66f;
    public float phase4Threshold = 50f;
    public float phase5Threshold = 33f;
    public float phase6Threshold = 16f;

    [Header("Movement")]
    public float moveSpeedPhase1 = 3f;
    public float moveSpeedPhase2 = 5f;
    public float moveSpeedPhase3 = 7f;
    public float moveSpeedPhase5 = 8f;
    public float dashSpeed = 15f;
    public float dashSpeedPhase6 = 20f;

    [Header("Prefabs")]
    public GameObject iceBulletPrefab;
    public GameObject crystalShardPrefab;
    public GameObject iceBombPrefab;

    public float bulletSpeed = 5f;

    private int currentPhase = 1;
    private Coroutine attackRoutine;

    private float currentHPPercent;

    private Transform[] shardOrbits;

    private Coroutine crystalRespawnRoutine;
    public float crystalRespawnInterval;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        attackRoutine = StartCoroutine(Phase1Routine());
    }

    void Update()
    {
        currentHPPercent = health.GetHealthPercent();

        if (currentPhase == 1 && currentHPPercent <= phase2Threshold)
        {
            currentPhase = 2;
            RestartPhase(Phase2Routine());
        }
        else if (currentPhase == 2 && currentHPPercent <= phase3Threshold)
        {
            currentPhase = 3;
            RestartPhase(Phase3Routine());
        }
        else if (currentPhase == 3 && currentHPPercent <= phase4Threshold)
        {
            currentPhase = 4;
            RestartPhase(Phase4Routine());
        }
        else if (currentPhase == 4 && currentHPPercent <= phase5Threshold)
        {
            currentPhase = 5;
            RestartPhase(Phase5Routine());
        }
        else if (currentPhase == 5 && currentHPPercent <= phase6Threshold)
        {
            currentPhase = 6;
            RestartPhase(Phase6Routine());
        }

        if (currentPhase == 1)
            MoveTowardsPlayer(moveSpeedPhase1,10f);
        else if (currentPhase == 2)
            MoveTowardsPlayer(moveSpeedPhase2,7f);
        else if (currentPhase == 3)
            MoveTowardsPlayer(moveSpeedPhase3,6f);
        else if (currentPhase == 5)
            MoveTowardsPlayer(moveSpeedPhase5,6f);

        if (currentPhase >= 2)
            RotateCrystalShards();
    }

    void RestartPhase(IEnumerator newRoutine)
    {
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(newRoutine);
    }

    void MoveTowardsPlayer(float speed , float distance)
    {
        if (player == null) return;

        // Tính vector từ player đến boss
        Vector2 toBoss = transform.position - player.position;

        // Nếu boss đang quá gần hoặc quá xa, điều chỉnh độ dài để giữ khoảng cách cố định
        float desiredDistance = distance;
        float currentDistance = toBoss.magnitude;
        if (Mathf.Abs(currentDistance - desiredDistance) > 0.1f)
        {
            // Kéo boss về/ra xa để duy trì khoảng cách
            Vector2 desiredPos = (Vector2)player.position + toBoss.normalized * desiredDistance;
            Vector2 moveDir = (desiredPos - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
            return;
        }

        // Tính hướng xoay quanh player theo ngược chiều kim đồng hồ
        Vector2 tangent = new Vector2(toBoss.y, -toBoss.x).normalized;

        transform.position += (Vector3)(tangent * speed * Time.deltaTime);
    }


    IEnumerator Phase1Routine()
    {
        while (true)
        {
            ShootCircle(iceBulletPrefab, 18);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Phase2Routine()
    {
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());
        while (true)
        {
            ShootCircle(iceBulletPrefab, 20);
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator Phase3Routine()
    {
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());
        while (true)
        {
            ShootCircle(iceBulletPrefab, 22);
            yield return new WaitForSeconds(1.2f);

            Vector3 spawnPos = player.position + Vector3.up * 8f;
            GameObject bomb = Instantiate(iceBombPrefab, spawnPos, Quaternion.identity);
            bomb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 5f;

            yield return new WaitForSeconds(1.25f);
        }
    }

    IEnumerator Phase4Routine()
    {
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());

        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            float dashTime = 1f;
            float elapsed = 0f;

            while (elapsed < dashTime)
            {
                transform.position += direction * dashSpeed * Time.deltaTime;

                if (elapsed == 0f)
                    ShootCircle(iceBulletPrefab, 24);

                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.75f);
        }
    }


    IEnumerator Phase5Routine()
    {
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());

        while (true)
        {
            ShootCircle(iceBulletPrefab,24);

            // Rơi ice bomb tăng độ loạn
            Vector3 spawnPos = player.position + Vector3.up * 7f + (Vector3)Random.insideUnitCircle * 2f;
            GameObject bomb = Instantiate(iceBombPrefab, spawnPos, Quaternion.identity);
            bomb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 6f;

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Phase6Routine()
    {
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());
        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            float dashTime = 1f;
            float elapsed = 0f;

            while (elapsed < dashTime)
            {
                transform.position += direction * dashSpeedPhase6 * Time.deltaTime;

                if (elapsed == 0f)
                    ShootCircle(iceBulletPrefab, 24);

                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.75f);
        }
    }


    void ShootCircle(GameObject prefab, int count)
    {
        float angleStep = 360f / count;
        float angle = 0f;

        for (int i = 0; i < count; i++)
        {
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 dir = new Vector2(dirX, dirY);

            GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = dir * bulletSpeed;

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            angle += angleStep;
        }
    }

    void SpawnCrystalShards()
    {
        // Xoá crystal cũ nếu còn
        if (shardOrbits != null)
        {
            foreach (Transform shard in shardOrbits)
            {
                if (shard != null)
                    Destroy(shard.gameObject);
            }
        }

        int shardCount = 12;
        shardOrbits = new Transform[shardCount];
        float radius = 4f;

        for (int i = 0; i < shardCount; i++)
        {
            float angleDeg = (360f / shardCount) * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * radius;
            Vector3 spawnPos = transform.position + offset;

            GameObject shard = Instantiate(crystalShardPrefab, spawnPos, Quaternion.identity);
            shard.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            shard.transform.SetParent(this.transform);

            shardOrbits[i] = shard.transform;
        }
    }



    void RotateCrystalShards()
    {
        if (shardOrbits == null) return;

        float rotateSpeed = 90f;

        for (int i = 0; i < shardOrbits.Length; i++)
        {
            if (shardOrbits[i] != null)
            {
                shardOrbits[i].RotateAround(transform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator CrystalRespawnLoop()
    {
        while (true)
        {
            SpawnCrystalShards();
            yield return new WaitForSeconds(crystalRespawnInterval);
        }
    }

}
