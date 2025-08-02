using UnityEngine;

public class DropText : MonoBehaviour
{
    public float dropSpeed = 800f;
    public float bounceHeight = 20f;
    public float bounceDuration = 0.2f;
    public float delayBeforeDrop = 0f;  // ← NEW

    private float targetY;
    private float dropStartTime;
    private bool isDropping = false;
    private bool isBouncing = false;
    private float bounceTimer = 0f;

    void Start()
    {
        targetY = transform.position.y;
        transform.position += new Vector3(0, 1000f, 0); // Đẩy chữ lên cao
        dropStartTime = Time.time + delayBeforeDrop;   // ← NEW
    }

    void Update()
    {
        if (!isDropping && Time.time >= dropStartTime)
        {
            isDropping = true;
        }

        if (isDropping && !isBouncing)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, dropSpeed * Time.deltaTime);
            transform.position = pos;

            if (Mathf.Abs(pos.y - targetY) < 0.1f)
            {
                transform.position = new Vector3(pos.x, targetY, pos.z);
                isBouncing = true;
                bounceTimer = 0f;
            }
        }
        else if (isBouncing)
        {
            bounceTimer += Time.deltaTime;
            float t = bounceTimer / bounceDuration;
            float bounceY = Mathf.Sin(t * Mathf.PI) * bounceHeight;

            transform.position = new Vector3(transform.position.x, targetY + bounceY, transform.position.z);

            if (t >= 1f)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                isBouncing = false;
                this.enabled = false;
            }
        }
    }
}
