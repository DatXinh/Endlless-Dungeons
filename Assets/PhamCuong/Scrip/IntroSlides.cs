using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSlides : MonoBehaviour
{
    public Image slideImage;         // Kéo Image UI vào đây
    public Sprite[] slides;          // Kéo các Sprite ảnh intro vào đây
    public float fadeDuration = 1f;
    public float displayDuration = 4f;
    public string sceneToLoad = "Home"; // Tên scene sau intro

    void Start()
    {
        StartCoroutine(PlaySlides());
    }

    IEnumerator PlaySlides()
    {
        slideImage.color = new Color(1, 1, 1, 0); // Ẩn ban đầu

        foreach (Sprite slide in slides)
        {
            slideImage.sprite = slide;

            // Fade in
            yield return StartCoroutine(FadeImage(0f, 1f));

            // Chờ thời gian hiển thị
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeImage(1f, 0f));
        }

        // Chuyển scene
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = slideImage.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            slideImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        slideImage.color = color;
    }
}
