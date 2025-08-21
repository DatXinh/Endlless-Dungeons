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

    [Header("Player Components (auto gán nếu quên)")]
    public PlayerHP playerHP;
    public PLayerMP playerMP;
    public PlayerInteractor playerInteractor;

    private float playTime = 0f;
    private bool isCountingTime = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // ✅ Auto tìm component nếu quên gán trong Inspector
            if (playerHP == null) playerHP = GetComponentInChildren<PlayerHP>();
            if (playerMP == null) playerMP = GetComponentInChildren<PLayerMP>();
            if (playerInteractor == null) playerInteractor = GetComponentInChildren<PlayerInteractor>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isCountingTime)
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
        // Reset vị trí player
        transform.position = Vector3.zero;
        if (child != null) child.transform.position = Vector3.zero;

        // Hiển thị tên màn
        if (sceneNameText != null)
        {
            sceneNameText.text = $"Màn chơi: {scene.name} {LoopManager.Instance.currentGameMode}";
        }

        // Camera setting
        SetPhysicalCameraByScene(scene);

        // 👉 Nếu về Home thì reset playTime
        if (scene.name == "Home")
        {
            ResetPlayTime();
        }
        else
        {
            // Ở các màn gameplay thì bắt đầu đếm giờ
            StartCountingTime();
        }
    }

    // ================= TIME CONTROL =================
    public void StartCountingTime() => isCountingTime = true;
    public void StopCountingTime() => isCountingTime = false;

    public void ResetPlayTime()
    {
        playTime = 0f;
        if (playTimeText != null)
        {
            playTimeText.text = "Thời gian chơi: 00:00";
        }
        isCountingTime = false;

        // Reset vòng lặp game (nếu có)
        LoopManager.Instance.ResetLoop();
        LoopManager.Instance.SetGameMode(LoopManager.GameMode.Normal);
    }

    // ✅ Lấy thời gian chơi (cho Firebase save)
    public int GetTimePlayed()
    {
        return Mathf.FloorToInt(playTime);
    }

    // ✅ Set lại thời gian chơi (dùng khi Continue)
    public void SetPlayTime(int time)
    {
        playTime = time;
        if (playTimeText != null)
        {
            int minutes = Mathf.FloorToInt(playTime / 60f);
            int seconds = Mathf.FloorToInt(playTime % 60f);
            playTimeText.text = $"Thời gian chơi: {minutes:D2}:{seconds:D2}";
        }
    }

    // ================= CAMERA =================
    public void SetPhysicalCameraByScene(Scene scene)
    {
        if (mainCamera != null)
        {
            if (scene.name == "1-Boss" || scene.name == "2-Boss" || scene.name == "3-Boss")
                mainCamera.usePhysicalProperties = false;
            else
                mainCamera.usePhysicalProperties = true;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
