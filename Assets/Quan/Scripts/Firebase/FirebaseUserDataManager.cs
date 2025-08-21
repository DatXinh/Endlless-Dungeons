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
                        Debug.Log($"📥 Loaded currentRun: HP={runData.hp}, MP={runData.mp}, Coin={runData.coin}, " +
                                  $"Time={runData.time}, Weapons={string.Join(", ", runData.weapons)}, Scene={runData.scene}");

                        var player = PlayerDontDestroyOnLoad.instance;
                        ApplyCurrentRunToPlayer(runData, player);

                        if (!string.IsNullOrEmpty(runData.scene))
                        {
                            SceneLoadManager.nextSceneName = runData.scene;
                            UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
                            Time.timeScale = 1f;
                        }
                    }
                    else
                    {
                        UserProfileData userData = JsonUtility.FromJson<UserProfileData>(json);
                        Debug.Log($"📥 Loaded hardData: HP={userData.hp}, MP={userData.mp}, Coin={userData.coin}, " +
                                  $"Time={userData.time}, Weapons={string.Join(", ", userData.weapons)}");

                        var player = PlayerDontDestroyOnLoad.instance;
                        ApplyHardDataToPlayer(userData, player);
                    }
                }
                else
                {
                    Debug.Log($"⚠ Không tìm thấy {dataType} cho user {userId}");
                }
            });
    }

    // ================== SOFT DATA ==================
    public void SaveCurrentRun(FirebaseUser user, PlayerHP hp, PLayerMP mp, PlayerInteractor interactor, int timePlayed, string sceneName)
    {
        if (user == null) return;
        if (hp == null || mp == null || interactor == null) return;

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
                    Debug.Log($"💾 CurrentRun saved! Scene={sceneName}, HP={hp.currentHP}, MP={mp.currentMP}, Coin={interactor.Coins}");
                }
                else
                {
                    Debug.LogError("❌ Lỗi khi lưu CurrentRun: " + task.Exception);
                }
            });
    }

    public void ClearCurrentRun(FirebaseUser user)
    {
        if (user == null) return;

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

    // 👉 HÀM MỚI: Tạo run mới từ scene đầu tiên
    public void CreateNewRun(FirebaseUser user, string startScene)
    {
        if (user == null) return;

        CurrentRunData runData = new CurrentRunData(
            100,  // HP mặc định
            50,   // MP mặc định
            0,    // Coin mặc định
            0,    // Thời gian
            new string[] { "", "" }, // Vũ khí rỗng
            startScene
        );

        string json = JsonUtility.ToJson(runData);

        dbRef.Child("users").Child(user.UserId).Child("currentRun")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("✨ Tạo run mới ở scene: " + startScene);
                else
                    Debug.LogError("❌ Lỗi khi tạo run mới: " + task.Exception);
            });
    }

    // ================== END GAME LOG ==================
    public void SaveEndGameLog(FirebaseUser user, CurrentRunData runData, string status)
    {
        if (user == null || runData == null) return;

        string logId = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        EndGameLog log = new EndGameLog(
            runData.hp,
            runData.mp,
            runData.coin,
            runData.time,
            runData.weapons,
            runData.scene,
            status
        );

        string json = JsonUtility.ToJson(log);

        dbRef.Child("users").Child(user.UserId).Child("endGameLogs").Child(logId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log($"💾 EndGameLog saved! Status={status}, Scene={runData.scene}");
                else
                    Debug.LogError("❌ Lỗi khi lưu EndGameLog: " + task.Exception);
            });
    }

    public void SaveLastEndLog(FirebaseUser user, CurrentRunData runData, string status)
    {
        if (user == null || runData == null) return;

        var log = new EndRunLog
        {
            status = status,
            scene = runData.scene,
            hp = runData.hp,
            mp = runData.mp,
            coin = runData.coin,
            time = runData.time,
            weapons = runData.weapons,
            timestampMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        string json = JsonUtility.ToJson(log);

        dbRef.Child("users").Child(user.UserId).Child("lastEndLog")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsCompleted) Debug.Log("💾 lastEndLog saved (" + status + ")");
                else Debug.LogError("❌ SaveLastEndLog error: " + t.Exception);
            });
    }

    public void ClearLastEndLog(FirebaseUser user)
    {
        if (user == null) return;

        dbRef.Child("users").Child(user.UserId).Child("lastEndLog")
            .RemoveValueAsync()
            .ContinueWithOnMainThread(t =>
            {
                if (t.IsCompleted) Debug.Log("🗑 lastEndLog removed");
                else Debug.LogError("❌ ClearLastEndLog error: " + t.Exception);
            });
    }

    // ================== APPLY DATA ==================
    public void ApplyCurrentRunToPlayer(CurrentRunData runData, PlayerDontDestroyOnLoad player)
    {
        if (player == null || runData == null) return;

        player.playerHP.SetHP(runData.hp);
        player.playerMP.SetMP(runData.mp);

        player.playerInteractor.Coins = runData.coin;
        player.playerInteractor.setCoinNumber();

        for (int i = 0; i < runData.weapons.Length; i++)
        {
            if (!string.IsNullOrEmpty(runData.weapons[i]))
                player.playerInteractor.EquipWeaponByName(runData.weapons[i], i);
        }

        player.SetPlayTime(runData.time);

        Debug.Log($"✅ Player đã hồi phục dữ liệu run: HP={runData.hp}, MP={runData.mp}, Coin={runData.coin}");
    }

    public void ApplyHardDataToPlayer(UserProfileData data, PlayerDontDestroyOnLoad player)
    {
        if (player == null || data == null) return;

        player.playerHP.SetHP(data.hp);
        player.playerMP.SetMP(data.mp);

        player.playerInteractor.Coins = data.coin;
        player.playerInteractor.setCoinNumber();

        for (int i = 0; i < data.weapons.Length; i++)
        {
            if (!string.IsNullOrEmpty(data.weapons[i]))
                player.playerInteractor.EquipWeaponByName(data.weapons[i], i);
        }

        player.SetPlayTime(data.time);

        Debug.Log($"✅ Player đã hồi phục dữ liệu hardData: HP={data.hp}, MP={data.mp}, Coin={data.coin}");
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

[System.Serializable]
public class EndGameLog
{
    public int hp;
    public int mp;
    public int coin;
    public int time;
    public string[] weapons;
    public string scene;
    public string status;

    public EndGameLog(int hp, int mp, int coin, int time, string[] weapons, string scene, string status)
    {
        this.hp = hp;
        this.mp = mp;
        this.coin = coin;
        this.time = time;
        this.weapons = weapons;
        this.scene = scene;
        this.status = status;
    }
}

[System.Serializable]
public class EndRunLog
{
    public string status;
    public string scene;
    public int hp;
    public int mp;
    public int coin;
    public int time;
    public string[] weapons;
    public long timestampMs;
}
