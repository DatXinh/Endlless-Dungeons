using UnityEngine;
using UnityEngine.EventSystems;

public class BoDAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
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
    public EnemyHealth EnemyHealth;
    public SummonPortal summonPortal;

    private float stateTimer = 0f;
    private bool isRanging = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        EnemyHealth = GetComponent<EnemyHealth>();
        FindNearestPlayer();
    }

    void Update()
    {
        canAttack = CheckPlayerInCone();

        // Nếu phát hiện player → dừng lại và tấn công
        if (canAttack)
        {
            animator.SetTrigger("IsMelee");
        }
        else
        {
            FindNearestPlayer();
            stateTimer += Time.deltaTime;
            if (stateTimer >= 2f)
            {
                isRanging = !isRanging;
                stateTimer = 0f;
            }

            if (isRanging)
            {
                // Dừng di chuyển và set trigger "IsRange"
                rb.linearVelocity = Vector2.zero; // Updated to use linearVelocity
                if (animator != null)
                {
                    animator.SetTrigger("IsRange");
                }
            }
            else
            {
                animator.SetTrigger("IsMoving");
                Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
                Vector2 oldPosition = rb.position;
                Vector2 newPosition = oldPosition + direction * Time.deltaTime * speed; // speed = 3
                rb.MovePosition(newPosition);
                // Lật hướng nhìn
                if (flipTransform != null)
                {
                    Vector3 scale = flipTransform.localScale;
                    scale.x = direction.x < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                    flipTransform.localScale = scale;
                }

                // Cập nhật hướng coneDirection theo hướng lật
                coneDirection = (flipTransform.localScale.x > 0) ? Vector2.left : Vector2.right;
            }
        }
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = float.MaxValue;
        GameObject nearest = null;
        foreach (GameObject player in players)
        {
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Vector2 toPlayer = (Vector2)(player.transform.position - transform.position);
            float distance = toPlayer.magnitude;
            if (distance > coneLength)
                continue;

            Vector2 dirNormalized = toPlayer.normalized;
            Vector2 coneDirWorld = transform.TransformDirection(coneDirection.normalized);
            float angle = Vector2.Angle(coneDirWorld, dirNormalized);
            if (angle <= coneAngle * 0.5f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirNormalized, coneLength, detectionMask);
                if (hit && hit.collider != null && hit.collider.gameObject == player)
                {
                    targetPlayer = player;
                    return true;
                }
            }
        }

        targetPlayer = null;
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

        // Cung hình nón
        int segments = 30;
        Vector3 prevPoint = origin + leftDir * coneLength;
        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = -halfAngle + coneAngle * t;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 point = origin + (rot * coneDirWorld) * coneLength;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
    public GameObject TargetPlayer => targetPlayer;
}
