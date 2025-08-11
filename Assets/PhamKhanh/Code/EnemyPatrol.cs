using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Vector2 patrolRange = new Vector2(3, 3);
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public float detectionRange = 6f;
    public float attackRange = 4f;
    public float retreatRange = 2f;
    public Transform player;

    [Header("Attack Settings")]
    public MonoBehaviour shootScript;

    private Vector3 startPos;
    private Vector3 patrolTarget;
    private float waitCounter;

    private void Start()
    {
        // Tìm player tự động
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        startPos = transform.position;
        SetNewPatrolPoint();
        waitCounter = waitTime;

        if (shootScript != null)
            shootScript.enabled = false;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            HandleChaseAndAttack(distance);
        }
        else
        {
            Patrol();
            if (shootScript != null)
                shootScript.enabled = false;
        }
    }

    void HandleChaseAndAttack(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance > attackRange)
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            if (shootScript != null)
                shootScript.enabled = false;
        }
        else if (distance < retreatRange)
        {
            transform.position -= (Vector3)(direction * moveSpeed * Time.deltaTime);
            if (shootScript != null)
                shootScript.enabled = false;
        }
        else
        {
            if (shootScript != null)
                shootScript.enabled = true;
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            if (waitCounter <= 0)
            {
                SetNewPatrolPoint();
                waitCounter = waitTime;
            }
            else
            {
                waitCounter -= Time.deltaTime;
            }
        }
    }

    void SetNewPatrolPoint()
    {
        float randX = Random.Range(-patrolRange.x, patrolRange.x);
        float randY = Random.Range(-patrolRange.y, patrolRange.y);
        patrolTarget = new Vector3(startPos.x + randX, startPos.y + randY, transform.position.z);
    }
}
