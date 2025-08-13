using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    public TextMeshProUGUI hintText;  // Kéo Text vào
    public List<string> hints;        // Danh sách gợi ý
    public float totalTimeToWrite = 2f; // Tổng thời gian viết hết (2s)

    void Start()
    {
        if (hints.Count > 0)
        {
            // Chọn ngẫu nhiên 1 câu
            string randomHint = hints[Random.Range(0, hints.Count)];
            StartCoroutine(TypeText(randomHint));
        }
    }

    IEnumerator TypeText(string textToType)
    {
        hintText.text = "";
        float delay = totalTimeToWrite / textToType.Length; // Thời gian mỗi ký tự

        foreach (char letter in textToType)
        {
            hintText.text += letter;
            yield return new WaitForSeconds(delay);
        }
    }
}
