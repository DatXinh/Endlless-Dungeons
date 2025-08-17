using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;                     // Tốc độ di chuyển
    public Rigidbody2D rb;                           // Gắn Rigidbody2D từ Editor

    private Vector2 movement;

    void Update()
    {
        // Lấy input từ bàn phím (WASD hoặc phím mũi tên)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Di chuyển nhân vật dùng Rigidbody để có va chạm vật lý
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
