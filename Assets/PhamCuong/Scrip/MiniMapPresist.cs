using UnityEngine;
using UnityEngine.SceneManagement;
public class MiniMapPresist : MonoBehaviour
{
    private static MiniMapPresist instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // Tránh tạo trùng khi load lại scene
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded; // đăng ký sự kiện
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentScene = scene.name;

        // Danh sách các scene KHÔNG cần minimap
        if (currentScene == "Star Game" || currentScene == "Home")
        {
            SetMinimapVisible(false);
        }
        else
        {
            SetMinimapVisible(true);
        }
    }

    void SetMinimapVisible(bool visible)
    {
        // Ẩn hoặc hiện camera minimap
        Transform minimapCamera = transform.Find("MinimapCamera");
        if (minimapCamera) minimapCamera.gameObject.SetActive(visible);

        // Ẩn hoặc hiện RawImage
        
        Transform rawImage = transform.Find("MinimapDisplay");
        if (rawImage) rawImage.gameObject.SetActive(visible);
        //border
        Transform border = transform.Find("Border");
        if (border) border.gameObject.SetActive(visible);
    }
}
