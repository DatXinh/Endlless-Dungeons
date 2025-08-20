using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class SceneLoadManager : MonoBehaviour
{
    public static string nextSceneName;

    void Start()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // ⏳ chờ 2 giây ở màn Loading
            Invoke(nameof(LoadNextScene), 2f);
        }
        Time.timeScale = 1f;
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("❌ Tên scene tiếp theo chưa được gán!");
            return;
        }

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        // 👉 Chỉ save khi có user và không phải về Home
        if (user != null && nextSceneName != "Home")
        {
            var player = PlayerDontDestroyOnLoad.instance;

            if (player != null && FirebaseUserDataManager.Instance != null
                && player.playerHP != null && player.playerMP != null && player.playerInteractor != null)
            {
                FirebaseUserDataManager.Instance.SaveCurrentRun(
                    user,
                    player.playerHP,
                    player.playerMP,
                    player.playerInteractor,
                    player.GetTimePlayed(),
                    nextSceneName
                );
            }
            else
            {
                Debug.LogWarning("⚠ Không thể save run vì thiếu reference!");
            }
        }

        // 👉 Load scene thực tế
        SceneManager.LoadScene(nextSceneName);
    }
}
