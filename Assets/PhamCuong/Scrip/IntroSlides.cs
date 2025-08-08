//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class IntroSlides : MonoBehaviour
//{
//    public Image slideImage;               // UI Image để hiển thị ảnh
//    public Sprite[] slides;                // Các Sprite ảnh
//    public AudioClip[] slideAudioClips;    // Các đoạn nhạc ứng với từng ảnh
//    public float fadeDuration = 1f;
//    public float displayDuration = 4f;
//    public string sceneToLoad = "Home";    // Scene sau intro

//    private AudioSource audioSource;

//    void Start()
//    {
//        audioSource = gameObject.AddComponent<AudioSource>();
//        StartCoroutine(PlaySlides());
//    }

//    IEnumerator PlaySlides()
//    {
//        slideImage.color = new Color(1, 1, 1, 0); // Ẩn ảnh ban đầu

//        for (int i = 0; i < slides.Length; i++)
//        {
//            slideImage.sprite = slides[i];

//            // Phát nhạc tương ứng
//            if (i < slideAudioClips.Length && slideAudioClips[i] != null)
//            {
//                audioSource.clip = slideAudioClips[i];
//                audioSource.Play();
//            }

//            // Fade in
//            yield return StartCoroutine(FadeImage(0f, 1f));

//            // Chờ hiển thị ảnh
//            yield return new WaitForSeconds(displayDuration);

//            // Fade out
//            yield return StartCoroutine(FadeImage(1f, 0f));
//        }

//        // Kết thúc, chuyển scene
//        SceneManager.LoadScene(sceneToLoad);
//    }

//    IEnumerator FadeImage(float startAlpha, float endAlpha)
//    {
//        float time = 0f;
//        Color color = slideImage.color;

//        while (time < fadeDuration)
//        {
//            float t = time / fadeDuration;
//            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
//            slideImage.color = color;
//            time += Time.deltaTime;
//            yield return null;
//        }

//        color.a = endAlpha;
//        slideImage.color = color;
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSlides : MonoBehaviour
{
    public Image slideImage;               // UI Image để hiển thị ảnh
    public Sprite[] slides;                // Các Sprite ảnh
    public AudioClip[] slideAudioClips;    // Các đoạn nhạc ứng với từng ảnh
    public float fadeDuration = 1f;
    public float displayDuration = 4f;
    public string sceneToLoad = "Home";    // Scene sau intro

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(PlaySlides());
    }

    IEnumerator PlaySlides()
    {
        slideImage.color = new Color(1, 1, 1, 0); // Ẩn ảnh ban đầu

        for (int i = 0; i < slides.Length; i++)
        {
            slideImage.sprite = slides[i];

            // Phát nhạc tương ứng
            if (i < slideAudioClips.Length && slideAudioClips[i] != null)
            {
                audioSource.clip = slideAudioClips[i];
                audioSource.Play();
            }

            // Fade in
            yield return StartCoroutine(FadeImage(0f, 1f));

            // Chờ hiển thị ảnh
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeImage(1f, 0f));
        }

        // Kết thúc, chuyển scene
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
