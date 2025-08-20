using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseDatabaseTest : MonoBehaviour
{
    DatabaseReference dbRef;

    void Start()
    {
        // Khởi tạo Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase sẵn sàng!");
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                SaveTestData();
            }
            else
            {
                Debug.LogError("❌ Firebase lỗi: " + task.Result);
            }
        });
    }

    void SaveTestData()
    {
        dbRef.Child("users").Child("testUser").Child("hp").SetValueAsync(2000);
        Debug.Log("💾 Đã lưu HP = 2000 vào Firebase");
    }
}
