using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("✅ Firebase sẵn sàng!");
            }
            else
            {
                Debug.LogError("❌ Firebase lỗi: " + task.Result);
            }
        });
    }
}
