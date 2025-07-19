using UnityEngine;
using UnityEngine.SceneManagement;

public class ToSecens : MonoBehaviour
{
    public string sceneName;
    public void toScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Tên scene không được đặt.");
        }
    }
}
