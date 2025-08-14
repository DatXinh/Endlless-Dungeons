using UnityEngine;
using System.Collections.Generic;

public class RainbowSlime : MonoBehaviour
{
    [Header("Enemy heal")]
    public int healAmount = 50; // Số máu hồi phục mỗi lần
    public float healInterval = 5f; // Khoảng thời gian giữa các lần hồi máu
    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public float moveSpeed = 5f;
    public float keepDistance = 3f; // Khoảng cách muốn giữ với mục tiêu

    public Rigidbody2D rb; // Rigidbody để di chuyển

    private GameObject currentTarget;
    private float healTimer = 0f;
    private static GameObject[] cachedEnemies;
    private static float cacheTimer = 0f;
    private static float cacheInterval = 0.5f; // Làm mới danh sách enemy mỗi 0.5s

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }
    // Lấy danh sách enemy đã cache (giảm tần suất gọi FindGameObjectsWithTag)
    private static GameObject[] GetCachedEnemies()
    {
        if (cachedEnemies == null || cacheTimer >= cacheInterval)
        {
            cachedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            cacheTimer = 0f;
        }
        return cachedEnemies;
    }

    void Update()
    {
        // Cập nhật cache timer chỉ 1 lần mỗi frame (chỉ cần ở 1 instance, nhưng an toàn cho nhiều instance)
        if (Time.frameCount % 2 == 0) // Giảm tần suất update timer
            cacheTimer += Time.deltaTime;

        currentTarget = FindNearestEnemy();
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
            // Nếu chưa đủ gần, di chuyển lại gần nhưng giữ khoảng cách an toàn
            if (dist > keepDistance)
            {
                Vector3 dir = (currentTarget.transform.position - transform.position).normalized;
                Vector3 targetPos = currentTarget.transform.position - dir * keepDistance;
                //transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime));
            }

            // Debug: vẽ đường tới mục tiêu
            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.red);
        }

        // Đếm thời gian hồi máu
        healTimer += Time.deltaTime;
        if (healTimer >= healInterval)
        {
            HealEnemiesInRange();
            healTimer = 0f;
        }
    }

    // Tìm enemy gần nhất trong bán kính detectionRadius
    public GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GetCachedEnemies();
        GameObject nearestEnemy = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == this.gameObject || enemy == null) continue; // Bỏ qua chính nó hoặc null
            float dist = Vector3.Distance(currentPos, enemy.transform.position);
            if (dist < minDist && dist <= detectionRadius)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    // Tìm tất cả EnemyHealth trong bán kính detectionRadius
    public List<EnemyHP> FindEnemiesWithHealthInRange()
    {
        List<EnemyHP> enemiesInRange = new List<EnemyHP>();
        GameObject[] enemies = GetCachedEnemies();
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == this.gameObject || enemy == null) continue;
            float dist = Vector3.Distance(currentPos, enemy.transform.position);
            if (dist <= detectionRadius)
            {
                EnemyHP health = enemy.GetComponent<EnemyHP>();
                if (health != null)
                {
                    enemiesInRange.Add(health);
                }
            }
        }
        return enemiesInRange;
    }

    // Hồi máu cho tất cả EnemyHealth trong bán kính
    private void HealEnemiesInRange()
    {
        List<EnemyHP> enemies = FindEnemiesWithHealthInRange();
        foreach (var health in enemies)
        {
            health.currentHP += healAmount;
            if (health.currentHP > health.maxHP)
                health.currentHP = health.maxHP;
            // Debug: vẽ đường hồi máu
            Debug.DrawLine(transform.position, health.transform.position, Color.green);
        }
    }
}
