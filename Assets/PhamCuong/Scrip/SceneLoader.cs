using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string homeSceneName = "Home";
    public string startGameSceneName = "Start Game";

    public void LoadSceneSmart()
    {
        // Đảm bảo game không bị pause
        Time.timeScale = 1f;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == homeSceneName)
        {
            // Khi đang ở Home thì load sang Start Game
            SceneManager.LoadScene(startGameSceneName);
        }
        else
        {
            // Reset để LoadSceneManager không tự load lại
            SceneLoadManager.nextSceneName = null;
            CancelInvoke(); // Ngăn các Invoke đang chờ (nếu script này cùng GameObject)

            // Load về Home
            SceneManager.LoadScene(homeSceneName);
        }
    }

}
