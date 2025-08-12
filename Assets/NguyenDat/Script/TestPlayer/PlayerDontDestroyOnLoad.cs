using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;

public class PlayerDontDestroyOnLoad : MonoBehaviour
{
    public static PlayerDontDestroyOnLoad instance;
    public GameObject child;

    [Header("UI")]
    public TextMeshProUGUI sceneNameText;
    public TextMeshProUGUI playTimeText;

    public Camera mainCamera;

    private float playTime = 0f;
    public bool isCountingTime = false; // biến mới để kiểm soát đếm thời gian

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

    private void Update()
    {
        if (isCountingTime) // chỉ đếm khi biến này = true
        {
            playTime += Time.deltaTime;
            if (playTimeText != null)
            {
                int minutes = Mathf.FloorToInt(playTime / 60f);
                int seconds = Mathf.FloorToInt(playTime % 60f);
                playTimeText.text = $"Thời gian chơi: {minutes:D2}:{seconds:D2}";
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Đặt lại vị trí
        transform.position = Vector3.zero;
        if (child != null)
        {
            child.transform.position = Vector3.zero;
        }

        // Cập nhật tên màn chơi
        if (sceneNameText != null)
        {
            sceneNameText.text = $"Màn chơi: {scene.name}" + LoopManager.Instance.currentGameMode;
        }
        SetPhysicalCameraByScene(scene);
    }

    public void StartCountingTime() // hàm để bắt đầu đếm
    {
        isCountingTime = true;
    }

    public void StopCountingTime() // hàm để dừng đếm nếu cần
    {
        isCountingTime = false;
    }

    public void ResetPlayTime()
    {
        playTime = 0f;
        if (playTimeText != null)
        {
            playTimeText.text = "Thời gian chơi: 00:00";
        }
        isCountingTime = false; // reset luôn trạng thái
        LoopManager.Instance.ResetLoop();
        LoopManager.Instance.SetGameMode(LoopManager.GameMode.Normal);
    }

    public void SetPhysicalCameraByScene(Scene scene)
    {
        if (mainCamera != null)
        {
            // Ví dụ: chỉ bật Physical Camera ở cảnh "Level_1" và "Level_2"
            if (scene.name == "1-Boss" || scene.name == "2-Boss" || scene.name == "3-Boss")
            {
                mainCamera.usePhysicalProperties = false;
            }
            else
            {
                mainCamera.usePhysicalProperties = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
