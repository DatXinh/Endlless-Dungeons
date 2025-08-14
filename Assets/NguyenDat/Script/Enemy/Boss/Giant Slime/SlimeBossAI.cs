using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeBossAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public SpriteRenderer spriteRenderer;

    [Header("Summon Slimes")]
    public GameObject slimePrefab;
    public GameObject rainbowSlimePrefab;
    public int slimeCount = 5;
    public int maxSlimeCount = 21;
    public float spawnRadius = 3f;
    public Transform slimeParent;

    [Header("Slime Ball Attack")]
    public GameObject slimeBallPrefab;
    public float slimeBallRadius = 3f;
    public float slimeBallSpeed = 5f;
    public float sequentialDelay = 0.1f;

    [Header("tiny mana")]
    public GameObject TinyMana;
    private bool hasSpawnedManaBurst = false;

    private Transform playerTransform;
    private List<GameObject> spawnedSlimeBalls = new List<GameObject>();
    private readonly WaitForSeconds wait2s = new WaitForSeconds(2f);
    private WaitForSeconds waitSequential;

    private EnemyHP EnemyHP;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        waitSequential = new WaitForSeconds(sequentialDelay);
        StartCoroutine(BossActionLoop());
        EnemyHP = GetComponent<EnemyHP>();
    }

    void Update()
    {
        if (playerTransform == null) return;
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        if (spriteRenderer != null && Mathf.Abs(dir.x) > 0.01f)
        {
            spriteRenderer.flipX = dir.x < 0;
        }
        if (!hasSpawnedManaBurst && EnemyHP.GetHealthPercent() <50f)
        {
            hasSpawnedManaBurst = true;
            SpawnTinyManaBurst();
        }
    }

    private IEnumerator BossActionLoop()
    {
        while (true)
        {
            int action = Random.Range(0, 2); // 0: summon, 1: attack

            if (action == 0)
                yield return SummonSlimes();
            else
                yield return Attack();

            yield return wait2s;
        }
    }

    private IEnumerator SummonSlimes()
    {
        yield return new WaitForSeconds(3f);

        if (slimePrefab == null || slimeParent == null) yield break;

        int currentCount = slimeParent.childCount;
        int amount = Mathf.Min(slimeCount, maxSlimeCount - currentCount);
        if (amount <= 0) yield break;

        for (int i = 0; i < amount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject prefab = (Random.value <= 0.1f && rainbowSlimePrefab != null) ? rainbowSlimePrefab : slimePrefab;

            GameObject slime = Instantiate(prefab, spawnPos, Quaternion.identity, slimeParent);
        }
    }

    private IEnumerator Attack()
    {
        if (slimeBallPrefab == null || playerTransform == null) yield break;

        spawnedSlimeBalls.Clear();
        float angleStep = 360f / 12f;

        for (int i = 0; i < 12; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * slimeBallRadius;
            Vector3 spawnPos = transform.position + offset;

            GameObject ball = Instantiate(slimeBallPrefab, spawnPos, Quaternion.identity);
            spawnedSlimeBalls.Add(ball);
        }

        int attackType = Random.Range(0, 2); // 0: all at once, 1: sequential
        yield return StartCoroutine(attackType == 0 ? FireAllAtOnce() : FireSequentially());

        spawnedSlimeBalls.Clear();
    }

    private IEnumerator FireAllAtOnce()
    {
        foreach (GameObject ball in spawnedSlimeBalls)
        {
            if (ball != null)
                LaunchBall(ball);
        }
        yield return null;
    }

    private IEnumerator FireSequentially()
    {
        foreach (GameObject ball in spawnedSlimeBalls)
        {
            if (ball != null)
                LaunchBall(ball);

            yield return waitSequential;
        }
    }

    private void LaunchBall(GameObject ball)
    {
        SlimeBall slimeBall = ball.GetComponent<SlimeBall>();
        if (slimeBall != null)
        {
            slimeBall.Launch(playerTransform.position, slimeBallSpeed);
        }
    }
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
}
