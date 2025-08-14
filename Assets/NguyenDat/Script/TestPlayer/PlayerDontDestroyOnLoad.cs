using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;

public class PlayerDontDestroyOnLoad : MonoBehaviour
{
    public static PlayerDontDestroyOnLoad instance;
    public GameObject child;

    // ==== DỮ LIỆU LƯU TRẠNG THÁI PLAYER ====
    public int hp;
    public int mp;
    public int coin;
    public string weapon;
    public float playTime;

    [Header("UI")]
    public TextMeshProUGUI sceneNameText;
    public TextMeshProUGUI playTimeText;

    public Camera mainCamera;

    private bool isCountingTime = false; // biến mới để kiểm soát đếm thời gian

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
        transform.position = Vector3.zero;
        if (child != null)
            child.transform.position = Vector3.zero;

        if (sceneNameText != null)
            sceneNameText.text = $"Màn chơi: {scene.name}" + LoopManager.Instance.currentGameMode;

        SetPhysicalCameraByScene(scene);
    }

    public void StartCountingTime()
    {
        isCountingTime = true;
    }

    public void StopCountingTime()
    {
        isCountingTime = false;
    }

    public void ResetPlayTime()
    {
        playTime = 0f;
        if (playTimeText != null)
            playTimeText.text = "Thời gian chơi: 00:00";

        isCountingTime = false;
        LoopManager.Instance.ResetLoop();
        LoopManager.Instance.SetGameMode(LoopManager.GameMode.Normal);
    }

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
