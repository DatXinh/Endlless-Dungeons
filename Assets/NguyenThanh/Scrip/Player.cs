using UnityEngine;

public class Player : MonoBehaviour
{
   
    public Rigidbody2D rb;
    public Animator animator;
    public int tocDo = 4;
    private float traiPhai;
    private float lenXuong;
    private bool isFacingRight = true;
    public Joystick joystick; // Gán trong Inspector


    void Update()
    {
       
        traiPhai = joystick.Horizontal;
        lenXuong = joystick.Vertical;
        rb.linearVelocity = new Vector2(tocDo * traiPhai, tocDo * lenXuong);
        

        // Đảo hướng nhân vật
        if (traiPhai < 0 && isFacingRight)
        {
            Flip();
        }
        else if (traiPhai > 0 && !isFacingRight)
        {
            Flip();
        }

        // Cập nhật animation di chuyển
        animator.SetFloat("di_chuyen", Mathf.Abs(traiPhai) + Mathf.Abs(lenXuong));

       
    }

    // Hàm đổi hướng nhân vật
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
