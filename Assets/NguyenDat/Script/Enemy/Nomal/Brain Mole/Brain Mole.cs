using UnityEngine;

public class BrainMole : MonoBehaviour
{
    [Header("Cone Settings")]
    public float coneAngle = 60f; // Góc hình nón (độ)
    public float coneLength = 5f; // Độ dài hình nón
    public Vector2 coneDirection; // Hướng hình nón (theo local space)

    [Header("Detection")]
    public LayerMask detectionMask; // Lớp để phát hiện (nên để Player)
    public bool canAttack; // true nếu có thể tấn công, false nếu không

    private monsterAI monsterAIComponent;
    private Animator animator;
    private CircleCollider2D childCircleCollider;

    void Awake()
    {
        // Lấy tham chiếu đến script MonsterAI trên cùng GameObject
        monsterAIComponent = GetComponent<monsterAI>();
        if (monsterAIComponent == null)
        {
            Debug.LogWarning("monsterAI component not found on " + gameObject.name);
        }

        // Lấy tham chiếu đến Animator trên cùng GameObject
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name);
        }

        // Tìm gameobject con và lấy CircleCollider2D đầu tiên nếu có
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                childCircleCollider = child.GetComponent<CircleCollider2D>();
                if (childCircleCollider != null)
                    break;
            }
            if (childCircleCollider == null)
            {
                Debug.LogWarning("No CircleCollider2D found on any child of " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("No child GameObject found under " + gameObject.name);
        }
    }

    void Update()
    {
        // Cập nhật coneDirection.x theo scale.x
        coneDirection.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(coneDirection.x);

        canAttack = CheckPlayerInCone();

        // Nếu có thể tấn công thì không cho phép di chuyển, ngược lại cho phép di chuyển
        if (monsterAIComponent != null)
        {
            monsterAIComponent.isMovable = !canAttack;
        }

        // Nếu có thể tấn công thì setTrigger "Atk"
        if (animator != null)
        {
            if (canAttack)
            {
                SetAttackTrigger();
            }
        }
    }

    // Hàm set trigger "Atk" cho Animator nếu canAttack = true
    public void SetAttackTrigger()
    {
        animator.SetBool("Atk",true);
    }

    // Hàm reset trigger "Atk" cho Animator nếu canAttack = false
    public void ResetAttackTrigger()
    {
        animator.SetBool("Atk",false);
        canAttack = false;
    }

    // Bật CircleCollider2D của gameobject con (nếu có)
    public void EnableChildCircleCollider()
    {
        if (childCircleCollider != null)
        {
            childCircleCollider.enabled = true;
        }
    }

    // Tắt CircleCollider2D của gameobject con (nếu có)
    public void DisableChildCircleCollider()
    {
        if (childCircleCollider != null)
        {
            childCircleCollider.enabled = false;
        }
    }

    // Hàm kiểm tra có player trong hình nón không
    bool CheckPlayerInCone()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Vector2 dirToPlayer = (player.transform.position - transform.position);
            float distance = dirToPlayer.magnitude;
            if (distance > coneLength)
                continue;

            // Chuyển hướng coneDirection sang world space
            Vector2 coneDirWorld = transform.TransformDirection(coneDirection.normalized);
            float angle = Vector2.Angle(coneDirWorld, dirToPlayer.normalized);
            if (angle <= coneAngle * 0.5f)
            {
                // Trả về true nếu phát hiện player trong vùng tấn công
                return true;
            }
        }
        // Không thay đổi giá trị của canAttack ở đây, chỉ trả về false nếu không phát hiện player
        return false;
    }

    // Vẽ hình nón trong Scene view để debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Vector3 coneDirWorld = transform.TransformDirection(coneDirection.normalized);

        // Vẽ đường trung tâm
        Gizmos.DrawLine(origin, origin + coneDirWorld * coneLength);

        // Vẽ hai cạnh biên
        float halfAngle = coneAngle * 0.5f;
        Quaternion leftRot = Quaternion.AngleAxis(-halfAngle, Vector3.forward);
        Quaternion rightRot = Quaternion.AngleAxis(halfAngle, Vector3.forward);
        Vector3 leftDir = leftRot * coneDirWorld;
        Vector3 rightDir = rightRot * coneDirWorld;
        Gizmos.DrawLine(origin, origin + leftDir * coneLength);
        Gizmos.DrawLine(origin, origin + rightDir * coneLength);

        // Vẽ cung tròn
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
}
