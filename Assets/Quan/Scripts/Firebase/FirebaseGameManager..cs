using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class FirebaseGameManager : MonoBehaviour
{
    public static FirebaseGameManager Instance;
    public GameObject playerPrefab; // Prefab chứa PlayerDontDestroyOnLoad
    private PlayerDontDestroyOnLoad player;
    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Gọi sau khi login hoặc register thành công
    /// </summary>
    public void InitAfterLogin()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        string uid = auth.CurrentUser.UserId;

        // Spawn player nếu chưa có
        if (player == null)
        {
            GameObject obj = Instantiate(playerPrefab);
            DontDestroyOnLoad(obj);
            player = obj.GetComponent<PlayerDontDestroyOnLoad>();
        }

        // Load dữ liệu từ Firebase
        dbRef.Child("users").Child(uid).Child("softData").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("❌ Lỗi load dữ liệu: " + task.Exception);
                return;
            }

            if (!task.Result.Exists)
            {
                // Chưa có dữ liệu => tạo hard data mặc định
                ApplyHardData();
                SaveSoftData("Home");
                SceneManager.LoadScene("Home");
            }
            else
            {
                var snap = task.Result;
                player.hp = int.Parse(snap.Child("hp").Value.ToString());
                player.mp = int.Parse(snap.Child("mp").Value.ToString());
                player.coin = int.Parse(snap.Child("coin").Value.ToString());
                player.weapon = snap.Child("weapon").Value.ToString();
                player.playTime = float.Parse(snap.Child("time").Value.ToString());

                string lastScene = snap.Child("scene").Value.ToString();
                SceneManager.LoadScene(lastScene);
            }
        });

        // Lắng nghe khi scene load xong
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Bỏ qua nếu là màn loading
        if (scene.name == "LoadScene")
            return;

        if (scene.name == "Home")
        {
            ApplyHardData();
            SaveSoftData("Home");
            Debug.Log("🏠 Reset về Home và lưu dữ liệu mặc định");
        }
        else
        {
            SaveSoftData(scene.name);
            Debug.Log("💾 Checkpoint lưu ở màn: " + scene.name);
        }
    }

    private void ApplyHardData()
    {
        player.hp = 100;
        player.mp = 50;
        player.coin = 0;
        player.weapon = "none";
        player.playTime = 0f;
    }

    public void SaveSoftData(string sceneName)
    {
        string uid = auth.CurrentUser.UserId;
        var data = new
        {
            scene = sceneName,
            hp = player.hp,
            mp = player.mp,
            coin = player.coin,
            weapon = player.weapon,
            time = player.playTime
        };

        dbRef.Child("users").Child(uid).Child("softData").SetRawJsonValueAsync(JsonUtility.ToJson(data));
    }
}
