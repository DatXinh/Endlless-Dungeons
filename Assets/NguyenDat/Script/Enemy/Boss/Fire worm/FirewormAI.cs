using UnityEngine;

public class FirewormAI : MonoBehaviour
{
    public float speed = 3f;
    public float minDistance = 10f;
    public float maxDistance = 15f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetPosition; // Điểm bot sẽ di chuyển đến
    private bool hasTargetPoint = false;

    private float atkTimer = 0f;
    private bool isAttacking = false;

    public float attackDuration = 1f;

    [Header("Tấn công")]
    public float minAttackInterval = 0.25f;
    public float maxAttackInterval = 2.5f;

    [Header("Tiny Mana")]
    public GameObject TinyMana;
    private bool hasSpawnedManaBurst = false;

    [Header("EnemyHP")]
    public EnemyHP enemyHP;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        enemyHP = GetComponent<EnemyHP>();
        atkTimer = 1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Không tìm thấy GameObject có tag 'Player'");

        spriteRenderer.flipX = false;
    }

    void Update()
    {
        if (player == null) return;

        // Nếu chưa có điểm mục tiêu hoặc đã đến gần thì chọn điểm mới
        if (!hasTargetPoint || Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            targetPosition = GetRandomPointNearPlayer();
            hasTargetPoint = true;
        }

        // Lật hướng sprite
        if (targetPosition.x != transform.position.x)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x) * (targetPosition.x < transform.position.x ? -1 : 1);
            transform.localScale = currentScale;
        }

        // Kiểm tra tấn công
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distanceToPlayer);
        float dynamicInterval = Mathf.Lerp(minAttackInterval, maxAttackInterval, t);

        atkTimer -= Time.deltaTime;

        if (!isAttacking && atkTimer <= 0f)
        {
            animator.SetTrigger("IsAtk");
            isAttacking = true;
            atkTimer = attackDuration;
        }
        else if (isAttacking && atkTimer <= 0f)
        {
            animator.SetTrigger("IsMove");
            isAttacking = false;
            atkTimer = dynamicInterval;
        }

        // Spawn mana khi máu dưới 50%
        if (!hasSpawnedManaBurst && enemyHP.GetHealthPercent() < 50f)
        {
            hasSpawnedManaBurst = true;
            SpawnTinyManaBurst();
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking && hasTargetPoint)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    // Lấy điểm ngẫu nhiên gần player
    Vector2 GetRandomPointNearPlayer()
    {
        Vector2 playerPos = player.position;
        float randomDistance = Random.Range(minDistance, maxDistance);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
        return playerPos + offset;
    }

    void SpawnTinyManaBurst()
    {
        if (TinyMana == null) return;
        int count = Random.Range(5, 11);
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
