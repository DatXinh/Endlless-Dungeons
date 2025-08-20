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

            // ✅ Fix: chỉ định DatabaseURL thủ công
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
    // ✅ Tạo user mới với dữ liệu mặc định
    public void SaveNewUser(FirebaseUser user)
    {
        string email = string.IsNullOrEmpty(user.Email) ? "unknown" : user.Email;

        UserProfileData hardData = new UserProfileData(
            email,
            100, // hp mặc định
            50,  // mp mặc định
            0,   // coin
            0,   // time
            new string[] { "Sword" } // vũ khí mặc định
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

    // ✅ Load dữ liệu user (hardData hoặc currentRun)
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

    // ✅ Save dữ liệu mặc định (hardData) khi về Home
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
    // ✅ Save dữ liệu run hiện tại (softData)
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
            weaponNames[i] = weapon != null ? weapon.name : "";
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
                    Debug.Log($"💾 CurrentRun saved! Scene={sceneName}, HP={hp.currentHP}, MP={mp.currentMP}, Coin={interactor.Coins}, Time={timePlayed}");
                }
                else
                {
                    Debug.LogError("❌ Lỗi khi lưu CurrentRun: " + task.Exception);
                }
            });
    }

    // ✅ Xóa currentRun khi về Home hoặc chết
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
