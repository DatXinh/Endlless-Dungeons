using UnityEngine;

public class SuicideEnemy : MonoBehaviour
{
    public float detectionRange = 5f;
    public float speed = 3f;
    public float explosionRadius = 1f;
    public int damage = 50;
    public GameObject explosionEffect;

    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Di chuyển
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            // Quay mặt về hướng người chơi (sprite nhìn lên, nên cộng 90 độ)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
