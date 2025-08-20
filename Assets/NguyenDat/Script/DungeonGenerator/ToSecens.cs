using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class ToSecens : MonoBehaviour
{
    public string sceneName;

    public void toScene()
    {
        // Trường hợp Endless mode (reset lại từ 1-1 sau khi xong 3-Boss)
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.IsEndlessMode() && SceneManager.GetActiveScene().name == "3-Boss")
            {
                sceneName = "1-1";
                LoopManager.Instance.IncreaseLoop();
            }
        }

        // ✅ Save dữ liệu trước khi chuyển scene
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null && PlayerDontDestroyOnLoad.instance != null)
        {
            var player = PlayerDontDestroyOnLoad.instance;
            FirebaseUserDataManager.Instance.SaveCurrentRun(
                user,
                player.playerHP,
                player.playerMP,
                player.playerInteractor,
                player.GetTimePlayed(),
                sceneName
            );
        }

        // Gán tên scene muốn chuyển đến
        SceneLoadManager.nextSceneName = sceneName;
        // Chuyển sang scene loading
        SceneManager.LoadScene("LoadScene");
    }
}
