using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static string nextSceneName;

    void Start()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Invoke("LoadNextScene", 5f);
        }
        Time.timeScale = 1f;
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
