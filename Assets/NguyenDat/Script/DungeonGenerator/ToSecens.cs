using UnityEngine;
using UnityEngine.SceneManagement;

public class ToSecens : MonoBehaviour
{
    public string sceneName;
    public void toScene()
    {
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.IsEndlessMode() && SceneManager.GetActiveScene().name == "3-Boss")
            {
                sceneName = "1-1";
                LoopManager.Instance.IncreaseLoop();
            }
        }
        // Gán tên scene muốn chuyển đến
        SceneLoadManager.nextSceneName = sceneName;
        // Chuyển sang scene loading
        SceneManager.LoadScene("LoadScene");
        
    }
}
