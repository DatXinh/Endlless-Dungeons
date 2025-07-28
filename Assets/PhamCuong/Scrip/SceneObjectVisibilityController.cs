using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneObjectVisibilityController : MonoBehaviour
{
    [Header("Chỉ hiện các object này ở các scene sau:")]
    [SerializeField] private string[] scenesToShow;

    [Header("Các GameObject cần ẩn/hiện:")]
    [SerializeField] private List<GameObject> objectsToToggle;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Nếu là object xuyên scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;
        bool shouldShow = false;

        foreach (string s in scenesToShow)
        {
            if (s == currentScene)
            {
                shouldShow = true;
                break;
            }
        }

        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
                obj.SetActive(shouldShow);
        }

        Debug.Log($"[SceneObjectVisibilityController] Scene {currentScene} → {(shouldShow ? "HIỆN" : "ẨN")} {objectsToToggle.Count} object.");
    }
}
