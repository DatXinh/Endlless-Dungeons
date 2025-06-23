using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    [Header("Hiệu ứng")]
    public float moveUpSpeed = 1.5f;
    public float fadeSpeed = 2f;
    public float lifeTime = 1f;

    private Color originalColor;
    private float timer;

    void Start()
    {
        if (damageText == null)
        {
            Debug.LogWarning("Thiếu reference damageText!");
            return;
        }

        originalColor = damageText.color;
        timer = 0f;

        // Tự huỷ sau lifeTime giây (phòng hờ nếu không mờ hết)
        Destroy(gameObject, lifeTime + 0.5f);
    }

    void Update()
    {
        // Bay lên
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Mờ dần
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    public void SetDamage(float amount, bool isCrit)
    {
        if (damageText == null) return;

        damageText.text = amount.ToString("F0");

        if (isCrit)
        {
            damageText.color = Color.red;
            damageText.fontSize = 40;
        }
        else
        {
            damageText.color = Color.yellow;
            damageText.fontSize = 30;
        }

        originalColor = damageText.color;
    }
}
