using UnityEngine;
using System.Collections;

public class Suicide2 : MonoBehaviour
{
    public float detectionRange = 5f;
    public float speed = 3f;
    public float explodeDistance = 1.2f;
    public float countdownTime = 2f;
    public int damage = 50;
    public GameObject explosionEffect;

    private Transform player;
    private bool isChasing = false;
    private bool isCountingDown = false;

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

        if (isChasing && !isCountingDown)
        {
            // Di chuyển đến người chơi
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            // Quay mặt
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

            // Bắt đầu countdown nếu đến gần
            if (distance <= explodeDistance)
            {
                StartCoroutine(CountdownToExplode());
            }
        }
    }

    IEnumerator CountdownToExplode()
    {
        isCountingDown = true;

        Debug.Log("Bắt đầu đếm ngược phát nổ!");

        // Ví dụ nhấp nháy màu
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++)
        {
            sr.color = (i % 2 == 0) ? Color.red : Color.white;
            yield return new WaitForSeconds(countdownTime / 6f);
        }

        // Phát nổ bất chấp khoảng cách
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Gây sát thương nếu người chơi đang trong bán kính nổ
        if (player != null)
        {
            float finalDistance = Vector2.Distance(transform.position, player.position);
            if (finalDistance <= explodeDistance)
            {
                
            }
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Không xử lý trigger để tránh nổ sớm
    }
}
