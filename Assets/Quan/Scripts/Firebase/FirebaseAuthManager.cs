using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    public FirebaseUser currentUser; // Người dùng hiện tại

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // Hàm đăng ký
    public void Register(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("❌ Đăng ký thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User; // ✅ Lấy FirebaseUser từ AuthResult
                Debug.Log($"✅ Đăng ký thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");
            });
    }

    // Hàm đăng nhập
    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("❌ Đăng nhập thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User; // ✅ Lấy FirebaseUser từ AuthResult
                Debug.Log($"✅ Đăng nhập thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");
            });
    }

    // Lấy UID hiện tại (để lưu dữ liệu)
    public string GetCurrentUserId()
    {
        return currentUser != null ? currentUser.UserId : null;
    }

    // Đăng xuất
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("🚪 Đã đăng xuất.");
    }
}
