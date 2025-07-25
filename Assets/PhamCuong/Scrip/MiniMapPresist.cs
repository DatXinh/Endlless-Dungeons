using UnityEngine;
using UnityEngine.SceneManagement;
public class MiniMapPresist : MonoBehaviour
{
    private static MiniMapPresist instance;
    public GameObject minimapCamera;
    public GameObject rawImage;
    public GameObject border;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            if (instance.gameObject != null)
            {
                Destroy(gameObject); // Bản cũ vẫn còn
                return;
            }
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Tránh đăng ký nhiều lần
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        string currentScene = scene.name;

        // Danh sách các scene KHÔNG cần minimap
        if (currentScene == "Star Game" || currentScene == "LoadScene" || currentScene == "1-Boss" || currentScene == "2-Boss" || currentScene == "3-Boss" || currentScene == "1-Shop" || currentScene == "2-Shop" || currentScene == "3-Shop")
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
        if (instance == null) return; // tránh lỗi khi instance chưa được khởi tạo
        //Transform minimapCamera = transform.Find("MinimapCamera");

        if (minimapCamera) minimapCamera.SetActive(visible);
        
     
        
        //Transform rawImage = transform.Find("MinimapDisplay");
        if (rawImage) rawImage.SetActive(visible);
        
        //Transform border = transform.Find("Border");
        if (border) border.SetActive(visible);
    }
}
