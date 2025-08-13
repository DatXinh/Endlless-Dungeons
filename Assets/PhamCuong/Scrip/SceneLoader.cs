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
            SceneManager.LoadScene(startGameSceneName);
        }
        else
        {
            SceneManager.LoadScene(homeSceneName);
        }
    }
}
