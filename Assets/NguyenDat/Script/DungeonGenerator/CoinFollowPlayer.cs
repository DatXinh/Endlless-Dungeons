using UnityEngine;

public class CoinFollowPlayer : MonoBehaviour
{
    public float moveSpeed = 7f;
    private Transform playerTransform;
    public PlayerInteractor playerInteractor;

    void Start()
    {
        // Tìm GameObject có tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Di chuyển về phía player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInteractor = other.GetComponentInChildren<PlayerInteractor>();
            if (playerInteractor != null)
            {
                playerInteractor.earnCoins(5);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Khi player vẫn ở trong vùng va chạm thì biến mất
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
