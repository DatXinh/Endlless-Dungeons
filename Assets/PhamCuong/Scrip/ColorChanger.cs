using TMPro;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 1f; // tốc độ chuyển màu

    void Update()
    {
        // Tạo màu RGB dựa theo thời gian (0->1)
        float r = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f;
        float g = Mathf.Sin(Time.time * speed + 2f) * 0.5f + 0.5f;
        float b = Mathf.Sin(Time.time * speed + 4f) * 0.5f + 0.5f;
        text.color = new Color(r, g, b);
    }
}
