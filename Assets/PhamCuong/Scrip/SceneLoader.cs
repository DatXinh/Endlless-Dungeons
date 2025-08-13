using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Hàm này sẽ được gọi từ nút
    public void LoadSceneByName(string sceneName)
    {
        // Đảm bảo timeScale trở lại bình thường nếu đang pause
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
