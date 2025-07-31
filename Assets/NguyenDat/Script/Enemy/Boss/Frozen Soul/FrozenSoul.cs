using System.Collections;
using UnityEngine;

public class FrozenSoul : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyHP health;

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
    public GameObject tinyMana;

    [Header("Crystal Settings")]
    public float crystalRespawnInterval = 4f;

    private int currentPhase = 1;
    private Coroutine attackRoutine;
    private Coroutine crystalRespawnRoutine;
    private Transform[] shardOrbits;

    public AudioSource bossMusic;

    private readonly WaitForSeconds wait2s = new WaitForSeconds(2f);
    private readonly WaitForSeconds wait1_5s = new WaitForSeconds(1.5f);
    private readonly WaitForSeconds wait1_25s = new WaitForSeconds(1.25f);
    private readonly WaitForSeconds wait1_2s = new WaitForSeconds(1.2f);
    private readonly WaitForSeconds wait1s = new WaitForSeconds(1f);
    private readonly WaitForSeconds wait0_75s = new WaitForSeconds(0.75f);

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            this.player = player.transform;
        attackRoutine = StartCoroutine(Phase1Routine());
        bossMusic.Play();
    }

    void Update()
    {
        float hpPercent = health.GetHealthPercent();
        switch (currentPhase)
        {
            case 1 when hpPercent <= phase2Threshold:
                ChangePhase(2, Phase2Routine());
                break;
            case 2 when hpPercent <= phase3Threshold:
                ChangePhase(3, Phase3Routine());
                break;
            case 3 when hpPercent <= phase4Threshold:
                ChangePhase(4, Phase4Routine());
                break;
            case 4 when hpPercent <= phase5Threshold:
                ChangePhase(5, Phase5Routine());
                break;
            case 5 when hpPercent <= phase6Threshold:
                ChangePhase(6, Phase6Routine());
                break;
        }

        MoveController();

        if (currentPhase >= 2) RotateCrystalShards();
    }

    void ChangePhase(int newPhase, IEnumerator newRoutine)
    {
        currentPhase = newPhase;
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(newRoutine);
        if (crystalRespawnRoutine == null)
            crystalRespawnRoutine = StartCoroutine(CrystalRespawnLoop());
        if (tinyMana != null)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject mana = Instantiate(tinyMana, transform.position, Quaternion.identity);
                MPRecover  mPRecover = mana.GetComponent<MPRecover>();
                if (mPRecover != null)
                {
                    mPRecover.manaRecoverAmount = 25;
                }
            }
        }
    }

    void MoveController()
    {
        if (player == null) return;

        float speed = currentPhase switch
        {
            1 => moveSpeedPhase1,
            2 => moveSpeedPhase2,
            3 => moveSpeedPhase3,
            5 => moveSpeedPhase5,
            _ => 0f
        };

        float desiredDistance = currentPhase switch
        {
            1 => 10f,
            2 => 7f,
            3 => 6f,
            5 => 6f,
            _ => 0f
        };

        if (speed > 0f)
            MoveTowardsPlayer(speed, desiredDistance);
    }

    void MoveTowardsPlayer(float speed, float distance)
    {
        Vector2 toBoss = transform.position - player.position;
        float currentDistance = toBoss.magnitude;

        if (Mathf.Abs(currentDistance - distance) > 0.1f)
        {
            Vector2 desiredPos = (Vector2)player.position + toBoss.normalized * distance;
            Vector2 moveDir = (desiredPos - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
        }
        else
        {
            Vector2 tangent = new Vector2(toBoss.y, -toBoss.x).normalized;
            transform.position += (Vector3)(tangent * speed * Time.deltaTime);
        }
    }

    IEnumerator Phase1Routine()
    {
        while (true)
        {
            ShootCircle(iceBulletPrefab, 8);
            yield return wait2s;
        }
    }

    IEnumerator Phase2Routine()
    {
        while (true)
        {
            ShootCircle(iceBulletPrefab, 10);
            yield return wait1_5s;
        }
    }

    IEnumerator Phase3Routine()
    {
        while (true)
        {
            ShootCircle(iceBulletPrefab, 12);
            yield return wait1_2s;

            Vector3 spawnPos = player.position + Vector3.up * 8f;
            GameObject bomb = Instantiate(iceBombPrefab, spawnPos, Quaternion.identity);
            bomb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 5f;

            yield return wait1_25s;
        }
    }

    IEnumerator Phase4Routine()
    {
        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            float elapsed = 0f;
            while (elapsed < 1f)
            {
                transform.position += direction * dashSpeed * Time.deltaTime;
                if (elapsed == 0f)
                    ShootCircle(iceBulletPrefab, 14);

                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return wait0_75s;
        }
    }

    IEnumerator Phase5Routine()
    {
        while (true)
        {
            ShootCircle(iceBulletPrefab, 15);

            Vector3 offset = Random.insideUnitCircle * 2f;
            Vector3 spawnPos = player.position + Vector3.up * 7f + offset;
            GameObject bomb = Instantiate(iceBombPrefab, spawnPos, Quaternion.identity);
            bomb.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * 6f;

            yield return wait1s;
        }
    }

    IEnumerator Phase6Routine()
    {
        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            float elapsed = 0f;
            while (elapsed < 1f)
            {
                transform.position += direction * dashSpeedPhase6 * Time.deltaTime;
                if (elapsed == 0f)
                    ShootCircle(iceBulletPrefab, 16);

                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return wait0_75s;
        }
    }

    void ShootCircle(GameObject prefab, int count)
    {
        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = dir * bulletSpeed;

            bullet.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
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

    void SpawnCrystalShards()
    {
        if (shardOrbits != null)
        {
            foreach (Transform shard in shardOrbits)
            {
                if (shard != null) Destroy(shard.gameObject);
            }
        }

        int shardCount = 12;
        float radius = 4f;
        shardOrbits = new Transform[shardCount];

        for (int i = 0; i < shardCount; i++)
        {
            float angleRad = (360f / shardCount) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * radius;

            GameObject shard = Instantiate(crystalShardPrefab, transform.position + offset, Quaternion.identity, transform);
            shard.transform.rotation = Quaternion.Euler(0f, 0f, angleRad * Mathf.Rad2Deg);
            shardOrbits[i] = shard.transform;
        }
    }

    void RotateCrystalShards()
    {
        if (shardOrbits == null) return;

        float rotateSpeed = 90f;
        foreach (Transform shard in shardOrbits)
        {
            if (shard != null)
                shard.RotateAround(transform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
}
