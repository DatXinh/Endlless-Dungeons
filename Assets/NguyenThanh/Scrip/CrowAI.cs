using UnityEngine;

public class CrowAI : MonoBehaviour
{
    [Header("Attributes")]
    public float moveSpeed = 2f;
    public int healthMax = 3;

    [Header("Detection Zones")]
    public Transform pursueZone;
    public Transform strikeZone; 
    public Vector2 strikeOffset = new Vector2(0.5f, 0f); // Vị trí lệch khi tấn công

    [Header("Patrol Settings")]
    public float patrolRadius = 2f;

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool attacking = false;
    private bool dead = false;
    private bool pursuing = false;
    private bool returning = false;

    private Vector2 homePosition;
    private Vector2 patrolPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = healthMax;

        homePosition = transform.position;
        GeneratePatrolPoint();

        // Gắn zone relay
        if (pursueZone != null && pursueZone.GetComponent<CrowZoneRelay>() == null)
        {
            var relay = pursueZone.gameObject.AddComponent<CrowZoneRelay>();
            relay.Initialize(this, CrowZoneType.Pursue);
        }

        if (strikeZone != null && strikeZone.GetComponent<CrowZoneRelay>() == null)
        {
            var relay = strikeZone.gameObject.AddComponent<CrowZoneRelay>();
            relay.Initialize(this, CrowZoneType.Strike);
        }
    }

    void Update()
    {
        if (dead) return;

        if (pursuing && !attacking)
        {
            MoveTo(player.position);
        }
        else if (returning)
        {
            MoveTo(homePosition);
            if (Vector2.Distance(transform.position, homePosition) < 0.1f)
            {
                returning = false;
                GeneratePatrolPoint();
            }
        }
        else if (!attacking)
        {
            Patrol();
        }

        UpdateStrikeZoneDirection();
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolPoint) < 0.2f)
            GeneratePatrolPoint();

        MoveTo(patrolPoint);
    }

    void GeneratePatrolPoint()
    {
        patrolPoint = homePosition + Random.insideUnitCircle * patrolRadius;
    }

    void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        spriteRenderer.flipX = direction.x < 0;
    }

    void UpdateStrikeZoneDirection()
    {
        if (strikeZone == null) return;

        // Nếu flipX = true (quay trái) thì offset âm, ngược lại dương
        float offsetX = spriteRenderer.flipX ? -Mathf.Abs(strikeOffset.x) : Mathf.Abs(strikeOffset.x);
        strikeZone.localPosition = new Vector3(offsetX, strikeOffset.y, 0f);

        // Xoay vùng tấn công theo hướng
        strikeZone.localRotation = spriteRenderer.flipX ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    public void SetPursuing(bool state)
    {
        pursuing = state;
        if (!state)
        {
            attacking = false;
            animator.SetBool("Attack", false);
            returning = true;
        }
        else
        {
            returning = false;
        }
    }

    public void SetAttacking(bool state)
    {
        attacking = state;
        animator.SetBool("Attack", state);
    }
}

public enum CrowZoneType { Pursue, Strike }

public class CrowZoneRelay : MonoBehaviour
{
    private CrowAI parentAI;
    private CrowZoneType zoneType;

    public void Initialize(CrowAI ai, CrowZoneType type)
    {
        parentAI = ai;
        zoneType = type;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == CrowZoneType.Pursue) parentAI.SetPursuing(true);
        else if (zoneType == CrowZoneType.Strike) parentAI.SetAttacking(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (zoneType == CrowZoneType.Pursue) parentAI.SetPursuing(false);
        else if (zoneType == CrowZoneType.Strike) parentAI.SetAttacking(false);
    }
}
