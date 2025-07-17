using System.Collections;
using UnityEngine;

public class HMCrytalFollowP : MonoBehaviour
{
    public float detectRadius = 3f;         // Khoảng cách phát hiện người chơi
    public float moveSpeed = 5f;            // Tốc độ bay về
    private Transform player;

    public Rigidbody2D rb2d; // Rigidbody2D component for physics interactions
    private bool isFollowing = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        StartCoroutine(SetRigidbodyStaticAfterDelay(0.25f));
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (!isFollowing && distance <= detectRadius)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            // Di chuyển dần về phía người chơi
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator SetRigidbodyStaticAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb2d != null)
        {
            rb2d.bodyType = RigidbodyType2D.Static;
        }
    }
}
