using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static string nextSceneName;

    void Start()
    {
        // Gọi chuyển scene sau 5 giây
        Invoke("LoadNextScene", 5f);
        Time.timeScale = 1f; // Đảm bảo thời gian không bị dừng
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Tên scene tiếp theo chưa được gán!");
        }
    }
}
