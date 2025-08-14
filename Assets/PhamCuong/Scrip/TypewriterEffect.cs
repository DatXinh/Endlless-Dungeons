using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text textUI;         // Kéo component TMP_Text vào đây
    [TextArea] public string fullText; // Đoạn text bạn muốn hiện
    public float totalDuration = 2f; // Tổng thời gian để hiển thị hết

    void Start()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        textUI.text = "";
        int length = fullText.Length;

        // Tính thời gian delay giữa mỗi chữ
        float delayPerChar = totalDuration / length;

        for (int i = 0; i < length; i++)
        {
            textUI.text += fullText[i];
            yield return new WaitForSeconds(delayPerChar);
        }
    }
}
