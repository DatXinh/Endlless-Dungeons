using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro; // Nếu dùng TextMeshPro

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;
    public FirebaseUser currentUser;

    [Header("Register UI")]
    public TMP_InputField regEmailInput;
    public TMP_InputField regPasswordInput;
    public TMP_InputField regConfirmPasswordInput;
    public GameObject registerCanvas;
    public GameObject loginCanvas;

    [Header("Login UI")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    // ===== NÚT ĐĂNG KÝ =====
    public void OnRegisterButton()
    {
        string email = regEmailInput.text.Trim();
        string password = regPasswordInput.text.Trim();
        string confirmPassword = regConfirmPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("❌ Email hoặc mật khẩu trống!");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogError("❌ Mật khẩu nhập lại không khớp!");
            return;
        }

        Register(email, password);
    }

    // ===== NÚT ĐĂNG NHẬP =====
    public void OnLoginButton()
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("❌ Email hoặc mật khẩu trống!");
            return;
        }

        Login(email, password);
    }

    private void Register(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("❌ Đăng ký thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                Debug.Log($"✅ Đăng ký thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");

                // Đăng nhập ngay sau khi đăng ký
                Login(email, password);
            });
    }

    private void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("❌ Đăng nhập thất bại: " + task.Exception);
                    return;
                }

                currentUser = task.Result.User;
                Debug.Log($"✅ Đăng nhập thành công! UID: {currentUser.UserId}, Email: {currentUser.Email}");

                // Gọi GameManager để load dữ liệu và scene
                FirebaseGameManager.Instance.InitAfterLogin();
            });
    }


    // ===== ĐĂNG XUẤT =====
    public void Logout()
    {
        auth.SignOut();
        currentUser = null;
        Debug.Log("🚪 Đã đăng xuất.");
    }

    // ===== CHUYỂN UI =====
    public void ShowRegisterUI()
    {
        loginCanvas.SetActive(false);
        registerCanvas.SetActive(true);
    }

    public void ShowLoginUI()
    {
        registerCanvas.SetActive(false);
        loginCanvas.SetActive(true);
    }
}
