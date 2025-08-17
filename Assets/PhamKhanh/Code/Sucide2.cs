using UnityEngine;
using System.Collections;

public class Suicide2 : MonoBehaviour
{
    [Header("Player Detection")]
    public string playerTag = "Player";
    public float detectionRange = 5f;
    public float explosionRange = 1.2f;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolRange = 4f; // khoảng cách tuần tra từ vị trí ban đầu

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float damage = 50f;
    public float countdownTime = 2f; // thời gian đếm ngược

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 initialPosition;
    private bool chasingPlayer = false;
    private int patrolDirection = 1;
    private bool isCountingDown = false;

    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        initialPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player != null && !isCountingDown)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                chasingPlayer = true;
            }
            else if (distanceToPlayer > detectionRange * 1.2f)
            {
                chasingPlayer = false;
            }

            // Nếu đến gần player thì bắt đầu đếm ngược phát nổ
            if (distanceToPlayer <= explosionRange)
            {
                StartCoroutine(CountdownToExplode());
            }
        }
    }

    void FixedUpdate()
    {
        if (!isCountingDown) // Khi đang đếm ngược thì đứng yên
        {
            if (chasingPlayer && player != null)
            {
                // Xoay quái hướng về Player
                Vector2 direction = (player.position - transform.position).normalized;
                if (direction.x != 0)
                    transform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                rb.linearVelocity = direction * chaseSpeed;
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Patrol()
    {
        if (transform.position.x > initialPosition.x + patrolRange)
        {
            patrolDirection = -1;
            Flip();
        }
        else if (transform.position.x < initialPosition.x - patrolRange)
        {
            patrolDirection = 1;
            Flip();
        }

        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    IEnumerator CountdownToExplode()
    {
        if (isCountingDown) yield break;
        isCountingDown = true;

        Debug.Log("Bắt đầu đếm ngược phát nổ!");

        for (int i = 0; i < 6; i++)
        {
            if (sr != null)
                sr.color = (i % 2 == 0) ? Color.red : Color.white;
            yield return new WaitForSeconds(countdownTime / 6f);
        }

        Explode();
    }

    void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // TODO: Thêm gây sát thương cho player ở đây nếu có hệ thống máu
        Destroy(gameObject);
    }
}
