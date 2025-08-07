using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public enum GameMode { Normal, Endless }
    public GameMode currentGameMode = GameMode.Normal;

    [Header("Loop Info")]
    public int currentLoop = 0;

    void Awake()
    {
        // Đảm bảo chỉ có 1 instance tồn tại
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Gọi khi hoàn thành một vòng
    public void IncreaseLoop()
    {
        currentLoop++;
        Debug.Log("Loop increased to: " + currentLoop);
    }

    // Gọi khi bắt đầu lại
    public void ResetLoop()
    {
        currentLoop = 0;
        Debug.Log("Loop reset to 0.");
    }

    // Gọi để đổi chế độ chơi
    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        Debug.Log("Game mode set to: " + currentGameMode);
    }

    public bool IsEndlessMode()
    {
        return currentGameMode == GameMode.Endless;
    }
}
