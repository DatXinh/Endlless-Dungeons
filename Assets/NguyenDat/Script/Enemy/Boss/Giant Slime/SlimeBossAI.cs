using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeBossAI : MonoBehaviour
{
    public float moveSpeed; // Tốc độ di chuyển của boss

    public SpriteRenderer spriteRenderer; // Gán SpriteRenderer trong Inspector

    public GameObject slimePrefab;// Prefab slime con, gán trong Inspector
    public GameObject rainbowSlimePrefab; // Prefab Rainbow Slime, gán trong Inspector
    public int slimeCount; // Số lượng slime con triệu hồi
    public float spawnRadius; // Bán kính xuất hiện slime con quanh boss
    public int maxSlimeCount = 21; // Giới hạn số lượng slime con
    public Transform slimeParent;

    public GameObject slimeBallPrefab; // Prefab quả cầu slime, gán trong Inspector
    public float slimeBallRadius; // Bán kính sinh ra quả cầu slime quanh boss
    public float slimeBallSpeed; // Tốc độ phóng quả cầu slime
    public float sequentialDelay; // Độ trễ giữa các lần phóng liên tiếp

    private Transform playerTransform;
    private List<GameObject> spawnedSlimeBalls = new List<GameObject>();

    void Start()
    {
        // Tìm GameObject có tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Bắt đầu vòng lặp hành động mỗi 2 giây
        StartCoroutine(BossActionLoop());
    }

    void Update()
    {
        if (playerTransform != null)
        {

            // Di chuyển về phía người chơi
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // Lật sprite theo hướng di chuyển
            if (spriteRenderer != null)
            {
                if (direction.x != 0)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
            }
        }
    }

    // Vòng lặp hành động: cứ 3 giây chọn 1 hành động
    private IEnumerator BossActionLoop()
    {
        while (true)
        {
            int action = Random.Range(0, 2); // 0: summon, 1: attack
            if (action == 0)
            {
                yield return SummonSlimes();
            }
            else
            {
                Attack();
                // Attack đã tự xử lý thời gian chờ trong AttackRoutine
                // Đợi thời gian phóng bóng nếu sequential
                float waitTime = sequentialDelay * 12f;
                yield return new WaitForSeconds(waitTime);
            }
            // Đảm bảo tổng thời gian mỗi vòng là ít nhất 2 giây
            yield return new WaitForSeconds(Mathf.Max(0, 2f - Time.deltaTime));
        }
    }

    // Hàm triệu hồi slime con sau 3 giây
    private IEnumerator SummonSlimes()
    {
        yield return new WaitForSeconds(3f);

        if (slimePrefab != null && slimeParent != null)
        {
            int currentSlimeCount = slimeParent.childCount;

            int spawnAmount = Mathf.Min(slimeCount, maxSlimeCount - currentSlimeCount);

            for (int i = 0; i < spawnAmount; i++)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

                // 10% cơ hội sinh rainbow slime
                GameObject prefabToSpawn = (Random.value <= 0.1f && rainbowSlimePrefab != null)
                    ? rainbowSlimePrefab
                    : slimePrefab;

                GameObject slime = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                slime.transform.SetParent(slimeParent);
            }
        }
    }


    // Phương thức tấn công sinh ra 12 quả cầu slime
    public void Attack()
    {
        if (slimeBallPrefab == null || playerTransform == null) return;

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
        StartCoroutine(AttackRoutine());
    }

    // Phóng slime ball ngay sau khi sinh ra
    private IEnumerator AttackRoutine()
    {
        int attackType = Random.Range(0, 2); // 0 hoặc 1
        if (attackType == 0)
        {
            // Phóng tất cả cùng lúc
            foreach (var ball in spawnedSlimeBalls)
            {
                if (ball != null)
                {
                    SlimeBall slimeBall = ball.GetComponent<SlimeBall>();
                    if (slimeBall != null)
                        slimeBall.Launch(playerTransform.position, slimeBallSpeed);
                }
            }
        }
        else
        {
            // Phóng lần lượt từng quả cầu
            foreach (var ball in spawnedSlimeBalls)
            {
                if (ball != null)
                {
                    SlimeBall slimeBall = ball.GetComponent<SlimeBall>();
                    if (slimeBall != null)
                        slimeBall.Launch(playerTransform.position, slimeBallSpeed);
                }
                yield return new WaitForSeconds(sequentialDelay);
            }
        }
        spawnedSlimeBalls.Clear();
    }
}
