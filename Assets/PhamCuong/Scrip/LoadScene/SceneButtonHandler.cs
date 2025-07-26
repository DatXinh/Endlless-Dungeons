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
    }
}
