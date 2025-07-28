using UnityEngine;
using UnityEngine.SceneManagement;

public class ToSecens : MonoBehaviour
{
    public string sceneName;
    public void toScene()
    {
        SceneLoadManager.nextSceneName = sceneName;
        SceneManager.LoadScene("LoadScene");
    }
}
