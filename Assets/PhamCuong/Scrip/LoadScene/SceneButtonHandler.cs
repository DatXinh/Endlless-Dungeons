using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class SceneButtonHandler : MonoBehaviour
{
    public string targetSceneName;

    // 👉 Nút Play chính (bắt đầu game / tiếp tục run dở)
    public void OnClickPlay()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("⚠ Bạn cần đăng nhập trước khi vào game!");
            var authManager = FindFirstObjectByType<FirebaseAuthManager>();
            if (authManager != null) authManager.ShowLoginUI();
            return;
        }

        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        // Kiểm tra xem có run đang dở không
        dbRef.Child("users").Child(user.UserId).Child("currentRun").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists && task.Result.Child("scene").Exists)
            {
                // 👉 Có dữ liệu run dở
                string sceneName = task.Result.Child("scene").Value.ToString();
                Debug.Log($"▶ Tiếp tục run ở scene: {sceneName}");

                SceneLoadManager.nextSceneName = sceneName;
                SceneManager.LoadScene("LoadScene");
            }
            else
            {
                // 👉 Không có run → bắt đầu mới từ 1-1
                Debug.Log("▶ Không có run dở, bắt đầu từ 1-1");

                SceneLoadManager.nextSceneName = "1-1";
                SceneManager.LoadScene("LoadScene");
            }
        });
    }

    // 👉 Nút load scene bình thường (menu, setting, home…)
    public void OnClickLoadScene()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("⚠ Bạn cần đăng nhập trước khi vào game!");
            var authManager = FindFirstObjectByType<FirebaseAuthManager>();
            if (authManager != null) authManager.ShowLoginUI();
            return;
        }

        SceneLoadManager.nextSceneName = targetSceneName;
        SceneManager.LoadScene("LoadScene");

        // ✅ Sửa lại phần check scene
        if (targetSceneName != "Home")
        {
            if (PlayerDontDestroyOnLoad.instance != null)
                PlayerDontDestroyOnLoad.instance.StartCountingTime();
        }
        else
        {
            if (FirebaseUserDataManager.Instance != null)
            {
                FirebaseUserDataManager.Instance.ClearCurrentRun(user);
                FirebaseUserDataManager.Instance.SaveHardData(user);
            }
        }
    }

    // 👉 Nút Endless mode (loop)
    public void OnClickLoadSceneWithLoop()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogWarning("⚠ Bạn cần đăng nhập trước khi vào game!");
            var authManager = FindFirstObjectByType<FirebaseAuthManager>();
            if (authManager != null) authManager.ShowLoginUI();
            return;
        }

        SceneLoadManager.nextSceneName = targetSceneName;
        LoopManager.Instance.SetGameMode(LoopManager.GameMode.Endless);

        if (PlayerDontDestroyOnLoad.instance != null)
            PlayerDontDestroyOnLoad.instance.StartCountingTime();

        SceneManager.LoadScene("LoadScene");
    }
}
