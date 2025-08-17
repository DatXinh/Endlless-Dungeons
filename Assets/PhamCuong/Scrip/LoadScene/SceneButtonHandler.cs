using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonHandler : MonoBehaviour
{
    public string targetSceneName;

    public void OnClickLoadScene()
    {
        // Gán tên scene muốn chuyển đến
        SceneLoadManager.nextSceneName = targetSceneName;
        // Chuyển sang scene loading
        SceneManager.LoadScene("LoadScene");
        if (targetSceneName != "Home")
        {
            PlayerDontDestroyOnLoad.instance.StartCountingTime();
        }
    }
    public void OnClickLoadSceneWithLoop()
    {
        // Gán tên scene muốn chuyển đến
        SceneLoadManager.nextSceneName = targetSceneName;
        // Đặt chế độ chơi là Endless
        LoopManager.Instance.SetGameMode(LoopManager.GameMode.Endless);
        PlayerDontDestroyOnLoad.instance.StartCountingTime();
        // Chuyển sang scene loading
        SceneManager.LoadScene("LoadScene");
    }
}
