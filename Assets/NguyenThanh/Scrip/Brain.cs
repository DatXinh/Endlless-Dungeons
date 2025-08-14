using UnityEngine;

public class Brain : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public int maxHealth = 3;

    [Header("Zones")]
    public Transform chaseZone;   // Vùng truy đuổi
    public Transform attackZone;  // Vùng tấn công

    [Header("Patrol Settings")]
    public float patrolRadius = 2f; // Bán kính tuần tra

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool isChasing = false;
    private bool isReturningHome = false;

    private Vector2 patrolCenter;
    private Vector2 patrolTarget;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        patrolCenter = transform.position;
        ChooseNewPatrolPoint();

        // Gắn ZoneRelay tự động nếu chưa có
        if (chaseZone != null && chaseZone.GetComponent<ZoneRelay_Brain>() == null)
        {
            var relay = chaseZone.gameObject.AddComponent<ZoneRelay_Brain>();
            relay.Setup(this, ZoneType_Brain.Chase);
        }

        if (attackZone != null && attackZone.GetComponent<ZoneRelay_Brain>() == null)
        {
            var relay = attackZone.gameObject.AddComponent<ZoneRelay_Brain>();
            relay.Setup(this, ZoneType_Brain.Attack);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isChasing && !isAttacking)
        {
            MoveTowards(player.position);
        }
        else if (isReturningHome)
        {
            MoveTowards(patrolCenter);
            if (Vector2.Distance(transform.position, patrolCenter) < 0.1f)
            {
                isReturningHome = false;
                ChooseNewPatrolPoint();
            }
        }
        else if (!isAttacking)
        {
            Patrol();
        }

        // Test nhận sát thương
        //if (Input.GetKeyDown(KeyCode.J)) TakeDamage();
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
            ChooseNewPatrolPoint();

        MoveTowards(patrolTarget);
    }

    void ChooseNewPatrolPoint()
    {
        patrolTarget = patrolCenter + Random.insideUnitCircle * patrolRadius;
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        spriteRenderer.flipX = direction.x < 0;
    }

    //void TakeDamage()
    //{
    //    currentHealth--;
    //    if (currentHealth <= 0)
    //    {
    //        isDead = true;
    //        animator.SetTrigger("Die");
    //        animator.SetBool("Attack", false);
    //        Invoke(nameof(DestroySelf), 1f);
    //    }
    //}

    //void DestroySelf()
    //{
    //    Destroy(gameObject);
    //}

    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
        if (!chasing)
        {
            isAttacking = false;
            animator.SetBool("Attack", false);
            isReturningHome = true;
        }
        else
        {
            isReturningHome = false;
        }
    }

    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
        animator.SetBool("Attack", attacking);
    }
}

public enum ZoneType_Brain { Chase, Attack }

public class ZoneRelay_Brain : MonoBehaviour
{
    private Brain parentAI;
    private ZoneType_Brain zoneType;

    public void Setup(Brain ai, ZoneType_Brain type)
    {
        parentAI = ai;
        zoneType = type;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType_Brain.Chase) parentAI.SetChasing(true);
        else if (zoneType == ZoneType_Brain.Attack) parentAI.SetAttacking(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == ZoneType_Brain.Chase) parentAI.SetChasing(false);
        else if (zoneType == ZoneType_Brain.Attack) parentAI.SetAttacking(false);
    }
}
