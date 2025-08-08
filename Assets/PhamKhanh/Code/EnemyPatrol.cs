using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing }
    private EnemyState currentState = EnemyState.Patrolling;

    public Vector2 patrolRange = new Vector2(3f, 3f);
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    public Transform player;

    private Vector2 centerPoint;
    private Vector2 targetPoint;
    private float waitCounter;

    void Start()
    {
        centerPoint = transform.position;
        ChooseNewTarget();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();

                // Phát hiện player trong phạm vi
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                ChasePlayer();

                // Nếu player ra khỏi phạm vi -> quay lại tuần tra
                if (distanceToPlayer > detectionRange + 1f) // Thêm ngưỡng để tránh nhấp nháy
                {
                    currentState = EnemyState.Patrolling;
                    ChooseNewTarget();
                }

                break;
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                ChooseNewTarget();
                waitCounter = 0f;
            }
        }
    }

    void ChooseNewTarget()
    {
        float randomX = Random.Range(-patrolRange.x, patrolRange.x);
        float randomY = Random.Range(-patrolRange.y, patrolRange.y);
        targetPoint = centerPoint + new Vector2(randomX, randomY);
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.position) <= attackRange)
            {
                // Tấn công ở đây (nếu có animation hoặc gây sát thương)
                Debug.Log($"{gameObject.name} attacking player!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, patrolRange * 2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}


