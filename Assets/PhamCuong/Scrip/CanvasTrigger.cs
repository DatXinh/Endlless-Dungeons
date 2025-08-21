using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class CanvasTrigger : MonoBehaviour
{
    public GameObject canvasUI;

    private void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canvasUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseCanvas()
    {
        canvasUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // 👉 Nút Continue
    public void OnContinueButton()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("⚠ Chưa có user đăng nhập, không thể Continue!");
            return;
        }

        var app = Firebase.FirebaseApp.DefaultInstance;
        var dbRef = FirebaseDatabase.GetInstance(app,
            "https://endless-dungeons-default-rtdb.firebaseio.com/").RootReference;

        dbRef.Child("users").Child(user.UserId).Child("currentRun").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || !task.Result.Exists)
                {
                    Debug.LogWarning("⚠ Không có dữ liệu currentRun để Continue!");
                    return;
                }

                string json = task.Result.GetRawJsonValue();
                CurrentRunData runData = JsonUtility.FromJson<CurrentRunData>(json);

                Debug.Log($"▶ Continue game tại scene {runData.scene}");

                // 👉 Áp dữ liệu vào Player
                var player = PlayerDontDestroyOnLoad.instance;
                if (player != null)
                {
                    FirebaseUserDataManager.Instance.ApplyCurrentRunToPlayer(runData, player);
                }

                // 👉 Load scene qua màn trung gian
                SceneLoadManager.nextSceneName = runData.scene;
                SceneManager.LoadScene("LoadScene");
                Time.timeScale = 1f; // reset pause
            });

        CloseCanvas();
    }
}
