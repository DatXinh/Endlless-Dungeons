using UnityEngine;

public class DemonAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed; // Tốc độ di chuyển của Demon
    [Header("Cone Settings")]
    public float coneAngle = 60f;
    public float coneLength = 5f;
    public Vector2 coneDirection = Vector2.left;

    [Header("Detection")]
    public LayerMask detectionMask;
    public bool canAttack;

    [Header("References")]
    public Transform flipTransform; // để biết hướng quay
    public Animator animator;

    private Rigidbody2D rb;
    private GameObject targetPlayer;
   

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        canAttack = CheckPlayerInCone();

        // Nếu phát hiện player → dừng lại và tấn công
        if (canAttack)
        {
            if (animator != null)
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false);
            }
        }
        else
        {
            if (animator != null)
                animator.SetBool("IsAttacking", false);

            // Luôn xác định player gần nhất
            FindNearestPlayer();

            // Tiếp tục đuổi theo player nếu có
            if (targetPlayer != null)
            {
                Vector2 direction = (targetPlayer.transform.position - transform.position).normalized;
                Vector2 oldPosition = rb.position;
                Vector2 newPosition = oldPosition + direction * Time.deltaTime * speed; // speed = 3
                rb.MovePosition(newPosition);

                // Set IsMoving = true nếu thực sự có di chuyển
                if (animator != null)
                {
                    bool isMoving = (newPosition - oldPosition).sqrMagnitude > 0.0001f;
                    animator.SetTrigger("IsMoving");
                }

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

    // Hàm tìm player gần nhất
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

            // Kiểm tra góc
            Vector2 dirNormalized = toPlayer.normalized;
            Vector2 coneDirWorld = transform.TransformDirection(coneDirection.normalized);
            float angle = Vector2.Angle(coneDirWorld, dirNormalized);
            if (angle <= coneAngle * 0.5f)
            {
                // Raycast kiểm tra có trúng player không
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
