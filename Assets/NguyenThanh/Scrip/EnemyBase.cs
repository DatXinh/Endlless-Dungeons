using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float moveSpeed = 2f;
    protected Transform player;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;

    protected Vector3 startPos;
    protected bool isChasing = false;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        Debug.Log($"{gameObject.name} start pos = {startPos}");
    }

    protected virtual void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            ReturnToStart();
        }
    }

    public void StartChasing() => isChasing = true;
    public void StopChasing() => isChasing = false;

    protected virtual void ChasePlayer()
    {
        if (!player) return;

        Vector2 target = player.position;
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        animator.SetFloat("Move", moveSpeed);
        spriteRenderer.flipX = (target.x < transform.position.x);
    }

    protected virtual void ReturnToStart()
    {
        transform.position = Vector2.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

        bool isMoving = Vector2.Distance(transform.position, startPos) > 0.05f;
        animator.SetFloat("Move", isMoving ? moveSpeed : 0f);
        spriteRenderer.flipX = (startPos.x < transform.position.x);
    }
}
