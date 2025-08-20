using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseUserDataManager : MonoBehaviour
{
    public static FirebaseUserDataManager Instance;
    private DatabaseReference dbRef;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ✅ chỉ định DatabaseURL
            var app = Firebase.FirebaseApp.DefaultInstance;
            dbRef = FirebaseDatabase.GetInstance(app,
                "https://endless-dungeons-default-rtdb.firebaseio.com/").RootReference;

            Debug.Log("✅ FirebaseUserDataManager Initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ================== HARD DATA ==================
    public void SaveNewUser(FirebaseUser user)
    {
        string email = string.IsNullOrEmpty(user.Email) ? "unknown" : user.Email;

        UserProfileData hardData = new UserProfileData(
            email, 100, 50, 0, 0, new string[] { "Sword" }
        );

        string json = JsonUtility.ToJson(hardData);

        dbRef.Child("users").Child(user.UserId).Child("hardData")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("💾 HardData mới đã được lưu.");
                else
                    Debug.LogError("❌ Lỗi khi lưu HardData: " + task.Exception);
            });
    }

    // ================== LOAD DATA ==================
    public void LoadUserData(string userId, string dataType = "hardData")
    {
        dbRef.Child("users").Child(userId).Child(dataType).GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("❌ Lỗi load user data: " + task.Exception);
                }
                else if (task.Result.Exists)
                {
                    string json = task.Result.GetRawJsonValue();

                    if (dataType == "currentRun")
                    {
                        CurrentRunData runData = JsonUtility.FromJson<CurrentRunData>(json);
                        Debug.Log($"📥 Loaded currentRun: HP={runData.hp}, MP={runData.mp}, Coin={runData.coin}, Time={runData.time}, Weapons={string.Join(", ", runData.weapons)}, Scene={runData.scene}");

                        // 👉 áp dữ liệu vào player
                        var player = PlayerDontDestroyOnLoad.instance;
                        ApplyCurrentRunToPlayer(runData, player);

                        // 👉 teleport về scene đã lưu qua màn LoadScene trung gian
                        if (!string.IsNullOrEmpty(runData.scene))
                        {
                            SceneLoadManager.nextSceneName = runData.scene;
                            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
                            Time.timeScale = 1f; // tránh bị pause
                        }
                    }
                    else
                    {
                        UserProfileData userData = JsonUtility.FromJson<UserProfileData>(json);
                        Debug.Log($"📥 Loaded hardData: HP={userData.hp}, MP={userData.mp}, Coin={userData.coin}, Time={userData.time}, Weapons={string.Join(", ", userData.weapons)}");
                    }
                }
                else
                {
                    Debug.Log($"⚠ Không tìm thấy {dataType} cho user {userId}");
                }
            });
    }

    // ================== HARD DATA ==================
    public void SaveHardData(FirebaseUser user)
    {
        string email = string.IsNullOrEmpty(user.Email) ? "unknown" : user.Email;

        UserProfileData hardData = new UserProfileData(
            email, 100, 50, 0, 0, new string[] { "Sword" }
        );

        string json = JsonUtility.ToJson(hardData);

        dbRef.Child("users").Child(user.UserId).Child("hardData")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("💾 HardData đã được reset!");
            });
    }

    // ================== SOFT DATA ==================
    public void SaveCurrentRun(FirebaseUser user, PlayerHP hp, PLayerMP mp, PlayerInteractor interactor, int timePlayed, string sceneName)
    {
        if (user == null)
        {
            Debug.LogWarning("⚠ SaveCurrentRun thất bại: user null");
            return;
        }
        if (hp == null || mp == null || interactor == null)
        {
            Debug.LogWarning("⚠ SaveCurrentRun thất bại: thiếu component player");
            return;
        }

        string[] weaponNames = new string[2];
        for (int i = 0; i < 2; i++)
        {
            var weapon = interactor.GetWeapon(i);
            weaponNames[i] = weapon != null ? weapon.name.Replace("(Clone)", "").Trim() : "";
        }

        CurrentRunData runData = new CurrentRunData(
            hp.currentHP,
            mp.currentMP,
            interactor.Coins,
            timePlayed,
            weaponNames,
            sceneName
        );

        string json = JsonUtility.ToJson(runData);

        dbRef.Child("users").Child(user.UserId).Child("currentRun")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"💾 CurrentRun saved! Scene={sceneName}, HP={hp.currentHP}, MP={mp.currentMP}, Coin={interactor.Coins}, Time={timePlayed}, Weapons={string.Join(", ", weaponNames)}");
                }
                else
                {
                    Debug.LogError("❌ Lỗi khi lưu CurrentRun: " + task.Exception);
                }
            });
    }

    public void ClearCurrentRun(FirebaseUser user)
    {
        if (user == null)
        {
            Debug.LogWarning("⚠ ClearCurrentRun thất bại: user null");
            return;
        }

        dbRef.Child("users").Child(user.UserId).Child("currentRun")
            .RemoveValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("🗑 CurrentRun đã bị xóa.");
                else
                    Debug.LogError("❌ Lỗi khi xóa CurrentRun: " + task.Exception);
            });
    }

    // ================== APPLY DATA ==================
    public void ApplyCurrentRunToPlayer(CurrentRunData runData, PlayerDontDestroyOnLoad player)
    {
        if (player == null || runData == null)
        {
            Debug.LogWarning("⚠ Không thể apply run data vì null");
            return;
        }

        // HP & MP
        player.playerHP.SetHP(runData.hp);
        player.playerMP.SetMP(runData.mp);

        // Coins
        player.playerInteractor.Coins = runData.coin;
        player.playerInteractor.setCoinNumber();

        // Weapons
        UserProfileData tempData = new UserProfileData(
            "temp", runData.hp, runData.mp, runData.coin, runData.time, runData.weapons
        );
        player.playerInteractor.SetPlayerData(tempData);

        // Time
        player.SetPlayTime(runData.time);

        Debug.Log($"✅ Player đã hồi phục dữ liệu run: HP={runData.hp}, MP={runData.mp}, Coin={runData.coin}, Time={runData.time}, Weapons={string.Join(", ", runData.weapons)}");
    }
}

// ================== DATA CLASS ==================
[System.Serializable]
public class UserProfileData
{
    public string email;
    public int hp;
    public int mp;
    public int coin;
    public int time;
    public string[] weapons;

    public UserProfileData(string email, int hp, int mp, int coin, int time, string[] weapons)
    {
        this.email = email;
        this.hp = hp;
        this.mp = mp;
        this.coin = coin;
        this.time = time;
        this.weapons = weapons;
    }
}

[System.Serializable]
public class CurrentRunData
{
    public int hp;
    public int mp;
    public int coin;
    public int time;
    public string[] weapons;
    public string scene;

    public CurrentRunData(int hp, int mp, int coin, int time, string[] weapons, string scene)
    {
        this.hp = hp;
        this.mp = mp;
        this.coin = coin;
        this.time = time;
        this.weapons = weapons;
        this.scene = scene;
    }
}
