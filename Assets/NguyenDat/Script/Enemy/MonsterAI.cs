using UnityEngine;

public class monsterAI : MonoBehaviour
{
    public float moveSpeed;
    public float detectDistance;
    public float checkInterval = 0.2f;
    public float rotateInterval = 2f; // Thời gian đổi hướng khi không phát hiện player

    public bool isMovable = true; // Biến kiểm tra có thể di chuyển hay không, mặc định true

    private static Transform playerTransform;
    private float checkTimer;
    private float rotateTimer;

    private Vector3 lastDirectionToPlayer = Vector3.zero;
    private float lastDistanceToPlayer = 0f;
    private bool canMove = false;

    private SpriteRenderer spriteRenderer;
    private int idleFacing = 1; // 1: phải, -1: trái

    void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (playerTransform == null || spriteRenderer == null)
            return;

        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;

            lastDirectionToPlayer = (playerTransform.position - transform.position).normalized;
            lastDistanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            canMove = (lastDistanceToPlayer <= detectDistance);
        }

        if (isMovable && canMove)
        {
            // Nếu phát hiện player thì di chuyển về phía player
            transform.position += lastDirectionToPlayer * moveSpeed * Time.deltaTime;

            // Lật sprite theo hướng di chuyển về phía player
            if (lastDirectionToPlayer.x != 0)
            {
                spriteRenderer.flipX = lastDirectionToPlayer.x > 0;
            }
        }
        else if (!canMove && isMovable)
        {
            // Nếu không phát hiện player và vẫn được phép di chuyển thì quay trái/phải mỗi rotateInterval giây
            rotateTimer -= Time.deltaTime;
            if (rotateTimer <= 0f)
            {
                rotateTimer = rotateInterval;
                idleFacing *= -1; // Đổi hướng
            }
            // Lật sprite theo hướng idleFacing
            spriteRenderer.flipX = idleFacing > 0;
        }
        // Nếu isMovable == false thì không quay trái/phải, giữ nguyên hướng hiện tại
    }
}
