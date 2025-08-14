using UnityEngine;
using System.Collections;

public class SuicidePatrol : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Exploding }
    private EnemyState currentState = EnemyState.Patrolling;

    [Header("Patrol Settings")]
    public Vector2 patrolRange = new Vector2(3f, 3f);
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float explodeRange = 1.2f;
    public float countdownTime = 2f;

    [Header("Explosion")]
    public int damage = 50;
    public GameObject explosionEffect;

    [Header("References")]
    public Transform player;

    private Vector2 centerPoint;
    private Vector2 targetPoint;
    private float waitCounter;
    private bool hasExploded = false;

    void Start()
    {
        centerPoint = transform.position;
        ChooseNewTarget();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();

                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                ChasePlayer();

                if (distanceToPlayer <= explodeRange)
                {
                    currentState = EnemyState.Exploding;
                    StartCoroutine(CountdownToExplode());
                }
                break;

            case EnemyState.Exploding:
                // Không làm gì, đợi nổ
                break;
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Quay hướng
        Vector2 dir = (targetPoint - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

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

            Vector2 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
    }

    IEnumerator CountdownToExplode()
    {
        Debug.Log($"{gameObject.name} bắt đầu đếm ngược!");

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int flashCount = 6;

        for (int i = 0; i < flashCount; i++)
        {
            if (sr != null)
                sr.color = (i % 2 == 0) ? Color.red : Color.white;

            yield return new WaitForSeconds(countdownTime / flashCount);
        }

        // Nổ
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Gây sát thương nếu player trong vùng nổ
        if (player != null && Vector2.Distance(transform.position, player.position) <= explodeRange + 0.5f)
        {
           
        }

        hasExploded = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, patrolRange * 2);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}

