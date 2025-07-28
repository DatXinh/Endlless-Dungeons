using UnityEngine;

public class BoDAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;

    [Header("Cone Settings")]
    public float coneAngle = 60f;
    public float coneLength = 5f;
    public Vector2 coneDirection = Vector2.left;

    [Header("Detection")]
    public LayerMask detectionMask;
    public bool canAttack;

    [Header("References")]
    public Transform flipTransform;
    public Animator animator;
    public Rigidbody2D rb;
    public GameObject targetPlayer;
    public SummonPortal summonPortal;

    private float stateTimer = 0f;
    private bool isRanging = false;
    private static GameObject[] playersCache;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        CachePlayers();
        FindNearestPlayer();
    }

    void Update()
    {
        canAttack = CheckPlayerInCone();

        if (canAttack)
        {
            rb.linearVelocity = Vector2.zero;
            animator?.SetTrigger("IsMelee");
        }
        else
        {
            HandlePatrol();
        }
    }

    void HandlePatrol()
    {
        if (targetPlayer == null || !targetPlayer.activeInHierarchy)
        {
            CachePlayers();
            FindNearestPlayer();
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= 2f)
        {
            isRanging = !isRanging;
            stateTimer = 0f;
        }

        if (isRanging)
        {
            rb.linearVelocity = Vector2.zero;
            animator?.SetTrigger("IsRange");
        }
        else if (targetPlayer != null)
        {
            animator?.SetTrigger("IsMoving");

            Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            UpdateFlip(direction);
        }
    }

    void UpdateFlip(Vector2 moveDir)
    {
        if (flipTransform != null)
        {
            Vector3 scale = flipTransform.localScale;
            scale.x = moveDir.x < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            flipTransform.localScale = scale;

            coneDirection = (scale.x > 0) ? Vector2.left : Vector2.right;
        }
    }

    void CachePlayers()
    {
        playersCache = GameObject.FindGameObjectsWithTag("Player");
    }

    void FindNearestPlayer()
    {
        float minDist = float.MaxValue;
        GameObject nearest = null;

        foreach (var player in playersCache)
        {
            if (!player.activeInHierarchy) continue;

            float dist = (player.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearest = player;
            }
        }

        targetPlayer = nearest;
        summonPortal.target = nearest?.transform;
    }

    bool CheckPlayerInCone()
    {
        foreach (var player in playersCache)
        {
            if (!player.activeInHierarchy) continue;

            Vector2 toPlayer = player.transform.position - transform.position;
            if (toPlayer.magnitude > coneLength) continue;

            Vector2 coneDirWorld = transform.TransformDirection(coneDirection.normalized);
            float angle = Vector2.Angle(coneDirWorld, toPlayer.normalized);
            if (angle <= coneAngle * 0.5f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, coneLength, detectionMask);
                if (hit.collider != null && hit.collider.gameObject == player)
                {
                    targetPlayer = player;
                    summonPortal.target = player.transform;
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Vector3 coneDirWorld = transform.TransformDirection(coneDirection.normalized);

        float halfAngle = coneAngle * 0.5f;
        Quaternion leftRot = Quaternion.AngleAxis(-halfAngle, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(halfAngle, Vector3.forward);

        Vector3 leftDir = leftRot * coneDirWorld;
        Vector3 rightDir = rightRot * coneDirWorld;

        Gizmos.DrawLine(origin, origin + coneDirWorld * coneLength);
        Gizmos.DrawLine(origin, origin + leftDir * coneLength);
        Gizmos.DrawLine(origin, origin + rightDir * coneLength);

        int segments = 30;
        Vector3 prevPoint = origin + leftDir * coneLength;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = -halfAngle + coneAngle * t;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 point = origin + (rot * coneDirWorld) * coneLength;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }

    public GameObject TargetPlayer => targetPlayer;
}
