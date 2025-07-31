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

    private Vector2 moveDirection;
    private float atkTimer = 0f;
    private bool isAttacking = false;

    public float attackDuration = 1f;

    [Header("Tấn công")]
    public float minAttackInterval = 0.25f;
    public float maxAttackInterval = 2.5f;

    [Header("Độ lệch hướng")]
    public float randomAngleRange = 30f; // độ lệch ± khi di chuyển để khó đoán

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

        Vector2 toPlayer = (player.position - transform.position);
        float distance = toPlayer.magnitude;

        // Lật hình nếu cần
        if (toPlayer.x != 0)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x) * (toPlayer.x < 0 ? -1 : 1);
            transform.localScale = currentScale;
        }

        // Cập nhật khoảng thời gian giữa các lần tấn công (theo khoảng cách)
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
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

        // Xác định hướng di chuyển với yếu tố ngẫu nhiên
        if (!isAttacking)
        {
            if (distance > maxDistance)
                moveDirection = AddRandomAngle(toPlayer.normalized);
            else if (distance < minDistance)
                moveDirection = AddRandomAngle(-toPlayer.normalized);
            else
                moveDirection = Vector2.zero;
        }
        else
        {
            moveDirection = Vector2.zero;
        }
        if (!hasSpawnedManaBurst && enemyHP.GetHealthPercent()<50f)
        {
            hasSpawnedManaBurst = true;
            SpawnTinyManaBurst();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    // Thêm độ lệch ngẫu nhiên vào hướng
    Vector2 AddRandomAngle(Vector2 dir)
    {
        float angleOffset = Random.Range(-randomAngleRange, randomAngleRange);
        return Quaternion.Euler(0, 0, angleOffset) * dir;
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
