using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnScene : MonoBehaviour
{
    [Header("Giữ object này ở các scene này:")]
    [SerializeField] private string[] scenesToKeep;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Giữ lại giữa các scene
        SceneManager.sceneLoaded += OnSceneLoaded; // Lắng nghe mỗi lần load scene
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Gỡ sự kiện để tránh lỗi
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;
        bool shouldKeep = false;

        foreach (string s in scenesToKeep)
        {
            if (s == currentScene)
            {
                shouldKeep = true;
                break;
            }
        }

        if (!shouldKeep)
        {
            Debug.Log($"[DestroyOnScene] Huỷ {gameObject.name} vì scene hiện tại là {currentScene}.");
            Destroy(gameObject);
        }
    }
}
