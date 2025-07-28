using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDontDestroyOnLoad : MonoBehaviour
{
    private static PlayerDontDestroyOnLoad instance;
    public GameObject child;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Đặt lại vị trí mỗi khi load scene mới
        transform.position = Vector3.zero; // (0,0,0)
        // Nếu có child, đặt lại vị trí của nó
        if (child != null)
        {
            child.transform.position = Vector3.zero; // (0,0,0)
        }
    }

    private void OnDestroy()
    {
        // Gỡ đăng ký nếu object bị huỷ
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
