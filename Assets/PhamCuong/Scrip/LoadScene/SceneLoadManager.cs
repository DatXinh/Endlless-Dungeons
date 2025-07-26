using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static string nextSceneName;

    void Start()
    {
        // Gọi chuyển scene sau 5 giây
        Invoke("LoadNextScene", 5f);
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
