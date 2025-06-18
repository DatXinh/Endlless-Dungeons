using UnityEngine;

public class DemonAI : MonoBehaviour
{
    public float moveSpeed = 3f; // Tốc độ di chuyển của Demon
    private Transform playerTransform;

    public Transform flipTransform; // Gán transform cần lật trong Inspector
    public bool isFacingLeft = true; // Biến thể hiện hướng quay hiện tại (true: trái, false: phải)

    public Animator animator; // Gán Animator trong Inspector

    // Các biến cho vùng tấn công hình nón
    public Transform attackOrigin; // Gán điểm gốc tấn công trong Inspector
    public float attackRange = 3f; // Bán kính vùng tấn công
    public float attackAngle = 60f; // Góc hình nón tấn công (độ)

    private bool isAttacking = false;

    void Start()
    {
        // Tìm GameObject có tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // Kiểm tra player có trong vùng tấn công hình nón không
            bool playerInCone = false;
            Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
            Vector3 toPlayer = playerTransform.position - origin;
            float distanceToPlayer = toPlayer.magnitude;

            if (distanceToPlayer <= attackRange)
            {
                Vector3 forward = (flipTransform != null ? flipTransform.right : transform.right) * (isFacingLeft ? -1 : 1);
                float angleToPlayer = Vector3.Angle(forward, toPlayer);
                if (angleToPlayer <= attackAngle * 0.5f)
                {
                    playerInCone = true;
                }
            }

            if (playerInCone)
            {
                isAttacking = true;
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                    animator.SetBool("IsAttacking", true);
                }
                // Không di chuyển khi tấn công
                return;
            }
            else
            {
                isAttacking = false;
                if (animator != null)
                {
                    animator.SetBool("IsMoving", true);
                    animator.SetBool("IsAttacking", false);
                }
            }

            // Di chuyển về phía người chơi nếu không tấn công
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Vector3 nextPosition = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            float moveDelta = Vector3.Distance(transform.position, nextPosition);
            transform.position = nextPosition;

            // Lật ảnh dựa vào hướng di chuyển và cập nhật hướng quay
            if (flipTransform != null && Mathf.Abs(direction.x) > 0.01f)
            {
                Vector3 scale = flipTransform.localScale;
                if (direction.x < 0)
                {
                    scale.x = Mathf.Abs(scale.x);
                    isFacingLeft = true;
                }
                else
                {
                    scale.x = -Mathf.Abs(scale.x);
                    isFacingLeft = false;
                }
                flipTransform.localScale = scale;
            }
        }
        else
        {
            // Nếu không có player, đảm bảo animation dừng lại
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsAttacking", false);
            }
        }
    }

    // Vẽ hình nón vùng tấn công bằng Gizmos
    void OnDrawGizmosSelected()
    {
        if (attackOrigin == null)
            return;

        Gizmos.color = Color.red; // Đổi sang màu đỏ tươi
        Vector3 origin = attackOrigin.position;

        // Hướng về phía đang nhìn
        Vector3 forward = (flipTransform != null ? flipTransform.right : transform.right) * (isFacingLeft ? -1 : 1);

        // Mở rộng tầm thêm 2 đơn vị khi vẽ Gizmos
        float gizmoRange = attackRange + 3f;

        int segments = 30;
        float halfAngle = attackAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.forward);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.forward);
        Vector3 leftRay = leftRayRotation * forward;
        Vector3 rightRay = rightRayRotation * forward;

        Vector3 prevPoint = origin + leftRay.normalized * gizmoRange;
        for (int i = 1; i <= segments; i++)
        {
            float lerp = (float)i / segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, lerp);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 dir = rot * forward;
            Vector3 point = origin + dir.normalized * gizmoRange;
            Gizmos.DrawLine(prevPoint, point);
            Gizmos.DrawLine(origin, point);
            prevPoint = point;
        }
    }
}
