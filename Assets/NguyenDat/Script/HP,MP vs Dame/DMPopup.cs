using TMPro;
using UnityEngine;

public class DMPopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float moveUpSpeed = 1f;
    public float fadeOutSpeed = 1f;
    private Color originalColor;

    private void Start()
    {
        originalColor = damageText.color;
    }

    public void Setup(int damageAmount, Color color)
    {
        damageText.text = damageAmount.ToString();
        damageText.color = color;
        originalColor = color;
    }

    void Update()
    {
        // Di chuyển lên
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Làm mờ dần
        Color c = damageText.color;
        c.a -= fadeOutSpeed * Time.deltaTime;
        damageText.color = c;

        if (c.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
